using SuperNewRoles.Roles;

namespace SuperNewRoles.Mode.SuperHostRoles.Roles;

internal class Samurai
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive), new System.Type[] { typeof(PlayerControl), typeof(RoleBehaviour), typeof(bool) })]
    private class SetHudActivePatch
    {
        public static void Postfix(HudManager __instance)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (PlayerControl.LocalPlayer.IsRole(RoleId.Samurai))
            {
                __instance.SabotageButton.ToggleVisible(visible: RoleClass.Samurai.UseSabo);
                __instance.ImpostorVentButton.ToggleVisible(visible: RoleClass.Samurai.UseVent);
            }
        }
    }
}