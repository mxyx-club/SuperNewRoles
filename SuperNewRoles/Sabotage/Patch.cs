using SuperNewRoles.Mode;

namespace SuperNewRoles.Sabotage;

internal class Patch
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.OpenMeetingRoom))]
    private class OpenMeetingPatch
    {
        public static void Prefix(HudManager __instance)
        {
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                p.resetChange();
            }
        }
    }
    [HarmonyPatch(typeof(InfectedOverlay), nameof(InfectedOverlay.Update))]
    private class SetUpCustomButton
    {
        public static void Postfix(InfectedOverlay __instance)
        {
            SabotageManager.InfectedOverlayInstance = __instance;
        }
    }
    [HarmonyPatch(typeof(InfectedOverlay), nameof(InfectedOverlay.Start))]
    private class SetUpCustomSabotageButton
    {
        public static void Postfix(InfectedOverlay __instance)
        {
            if (ModeHandler.IsMode(ModeId.Default))
            {
                CognitiveDeficit.Main.Create(__instance);
            }
        }
    }
}