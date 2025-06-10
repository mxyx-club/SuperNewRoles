namespace SuperNewRoles.Modules;
public static class DebugModeManager
{
    public static bool IsDebugMode;
    public static void ClearAndReloads()
    {
        IsDebugMode = ConfigRoles.DebugMode.Value;
    }
}