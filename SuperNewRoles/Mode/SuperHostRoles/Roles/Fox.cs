using SuperNewRoles.Roles;

namespace SuperNewRoles.Mode.SuperHostRoles.Roles;

internal class Fox
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive), new System.Type[] { typeof(PlayerControl), typeof(RoleBehaviour), typeof(bool) })]
    private class SetHudActivePatch
    {
        public static void Postfix(HudManager __instance)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (PlayerControl.LocalPlayer.IsRole(RoleId.Fox))
            {
                __instance.ReportButton.ToggleVisible(visible: RoleClass.Fox.UseReport);
            }
        }
    }
}