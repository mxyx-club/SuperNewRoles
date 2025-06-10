using SuperNewRoles.Buttons;
using SuperNewRoles.Replay;
using SuperNewRoles.Roles;

namespace SuperNewRoles.Patches;

internal class HudManagerPatch
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdatePatch
    {
        public static void Prefix(HudManager __instance)
        {
            GameSettingsScale.GameSettingsScalePatch(__instance);
        }
        public static void Postfix(HudManager __instance)
        {
            WallHack.WallHackUpdate();
            if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started) return;
            ReplayManager.HudUpdate();
            Mode.Zombie.FixedUpdate.ZombieTimerUpdate(__instance);
            CustomButton.HudUpdate();
            ButtonTime.Update();
            Tuna.HudUpdate();
            Arsonist.HudUpdate();
            Shielder.HudUpdate();
            Roles.Attribute.Jumbo.FixedUpdate();
            Zoom.HudUpdate(__instance);
        }
    }
}