namespace SuperNewRoles.Modules;
public static class DebugModeManager
{
    public static bool IsDebugMode;
    public static void UpdateDebugModeState()
    {
        IsDebugMode = ConfigRoles.DebugMode.Value;
    }
}