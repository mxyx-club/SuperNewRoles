using SuperNewRoles.CustomCosmetics;
using SuperNewRoles.Mode;
using UnityEngine;

namespace SuperNewRoles.Patches;

internal class GameStartPatch
{
    public static bool lastPublic;
    public static float lastTimer;

    /// <summary>公開部屋を封印するか</summary>
    private const bool PublicSeal = false;

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.MakePublic))]
    private class MakePublicPatch
    {
        public static bool Prefix(GameStartManager __instance)
        {
            if (!AmongUsClient.Instance.AmHost || ModHelpers.IsCustomServer()) return true;
            if (!PublicSeal) return true;

            string HostAmongUSVer = $"{Application.version}({Constants.GetPurchasingPlatformType()})";
            System.Version HostSNRVer = ShareGameVersion.GameStartManagerUpdatePatch.VersionPlayers[AmongUsClient.Instance.HostId].version; // HostのSNRVersion取得

            string reason = null; // 都度変える
            string error = string.Format(ModTranslation.GetString("PublicRoomError"), HostAmongUSVer, HostSNRVer, reason == null ? ModTranslation.GetString("CheckOfficialInformation") : reason);

            Error($"公開が無効に関わらず, 公開ボタンが押されました。", "MakePublicPatch");
            __instance.MakePublicButton.color = Palette.DisabledClear;
            __instance.privatePublicText.color = Palette.DisabledClear;

            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, error);
            return false;
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGame))]
    private class CoStartGamePatch
    {
        public static void Postfix()
        {
            if (lastPublic && AmongUsClient.Instance.AmHost)
            {
                Modules.MatchMaker.EndInviting();
            }
            if (CustomOption.IsValuesUpdated)
            {
                OptionSaver.WriteNowOptions();
                CustomOption.IsValuesUpdated = false;
            }
        }
    }
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    private class StartPatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            lastPublic = AmongUsClient.Instance.IsGamePublic;
            if (lastPublic && AmongUsClient.Instance.AmHost)
            {
                Modules.MatchMaker.CreateRoom();
            }
            lastTimer = 0;
        }
    }
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    private class AmongUsClientOnPlayerJoinedPatch
    {
        public static void Postfix()
        {
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined && lastPublic && AmongUsClient.Instance.AmHost)
            {
                Modules.MatchMaker.UpdatePlayerCount(true);
            }
        }
    }
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
    private class AmongUsClientOnPlayerLeftPatch
    {
        public static void Postfix()
        {
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined && lastPublic && AmongUsClient.Instance.AmHost)
            {
                Modules.MatchMaker.UpdatePlayerCount();
            }
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    private class UpdatePatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (lastPublic != AmongUsClient.Instance.IsGamePublic && AmongUsClient.Instance.AmHost)
            {
                if (AmongUsClient.Instance.IsGamePublic)
                {
                    Modules.MatchMaker.CreateRoom();
                }
                else
                {
                    Modules.MatchMaker.EndInviting();
                }
                lastPublic = AmongUsClient.Instance.IsGamePublic;
            }
            if (lastPublic && AmongUsClient.Instance.AmHost)
            {
                lastTimer += Time.deltaTime;
                if (lastTimer >= 12.5f)
                {
                    Modules.MatchMaker.KeepAlive();
                    lastTimer = 0;
                }
            }
        }
    }
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class LobbyCountDownTimer
    {
        public static void Postfix()
        {
            if (!GameStartManager._instance || !AmongUsClient.Instance.AmHost) return; // 以下ホストのみで動作

            if (CustomOptionHolder.ProhibitModColor.GetBool() || ModeHandler.IsMode(ModeId.SuperHostRoles, false))
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (!player) continue;
                    if (player.Data.DefaultOutfit.ColorId < CustomColors.DefaultPickAbleColors) continue;
                    player.CheckColor((byte)player.Data.DefaultOutfit.ColorId);
                }
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                FastDestroyableSingleton<GameStartManager>.Instance.ResetStartState();
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                FastDestroyableSingleton<GameStartManager>.Instance.countDownTimer = 0;
            }

            // 以下デバッグモード限定の機能
            if (!ModHelpers.IsDebugMode()) return;

            if (CustomOptionHolder.DebugModeFastStart.GetBool()) // デバッグモードでデバッグ即開始が有効
            {
                if (GameStartManager.InstanceExists && FastDestroyableSingleton<GameStartManager>.Instance.startState == GameStartManager.StartingStates.Countdown) // カウントダウン中
                {
                    FastDestroyableSingleton<GameStartManager>.Instance.countDownTimer = 0; //カウント0
                }
            }
        }
    }
}