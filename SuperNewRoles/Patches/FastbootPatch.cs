namespace SuperNewRoles.Patches;

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
internal class SplashLogoAnimatorPatch
{
    private static bool IsAgarthaLoaded;
    public static void Prefix(SplashManager __instance)
    {
        if (!IsAgarthaLoaded)
        {
            SuperNewRolesPlugin.AgarthaLoad();
            IsAgarthaLoaded = true;
        }
        if (ConfigRoles.DebugMode.Value)
        {
            __instance.sceneChanger.AllowFinishLoadingScene();
            __instance.startedSceneLoad = true;
        }
    }
}