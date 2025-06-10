namespace SuperNewRoles.Roles;

internal class Workperson
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    private class BeginCrewmatePatch
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer.IsRole(RoleId.Workperson))
            {
                PlayerControl.LocalPlayer.GenerateAndAssignTasks(CustomOptionHolder.WorkpersonCommonTask.GetInt(), CustomOptionHolder.WorkpersonShortTask.GetInt(), CustomOptionHolder.WorkpersonLongTask.GetInt());
            }
        }
    }
}