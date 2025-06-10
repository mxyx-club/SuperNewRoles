using SuperNewRoles.Roles.Impostor;

namespace SuperNewRoles.Patches;
public static class MushroomPatch
{
    [HarmonyPatch(typeof(Mushroom), nameof(Mushroom.ResetState))]
    public static class MushroomResetStatePatch
    {
        public static void Postfix(Mushroom __instance)
        {
            Mushroomer.MushroomResetStatePatch(__instance);
        }
    }
}