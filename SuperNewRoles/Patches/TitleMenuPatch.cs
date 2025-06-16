using System;
using UnityEngine;
using UnityEngine.UI;

namespace SuperNewRoles.Patches;

[HarmonyPatch(typeof(PlayerParticles), nameof(PlayerParticles.Start))]
public class MainMenuStartPatcha
{
    private static void Postfix(PlayerParticles __instance)
    {
        //とりあえず僕の誕生日終わるまで出しとく
        if (DateTime.UtcNow < new DateTime(2023, 11, 4, 15, 0, 0) &&
            !AprilFoolsMode.ShouldHorseAround())
            return;
        foreach (var item in __instance.pool.activeChildren)
        {
            PlayerMaterial.SetColors(ModHelpers.GetRandomIndex(Palette.PlayerColors.ToList()), item.TryCast<PlayerParticle>().myRend);
        }
    }
}