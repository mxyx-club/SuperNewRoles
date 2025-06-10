using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuperNewRoles.Roles;

internal class Doctor
{
    public static void FixedUpdate()
    {
        if (RoleClass.Doctor.IsChargingNow && Vector2.Distance(GameObject.Find("panel_vitals").transform.position, CachedPlayer.LocalPlayer.transform.position) <= 1.2f)
        {
            RoleClass.Doctor.BatteryZeroTime -= Time.fixedDeltaTime;
            RoleClass.Doctor.Battery = (int)(RoleClass.Doctor.BatteryZeroTime * (100f / RoleClass.Doctor.ChargeTime));
            if (RoleClass.Doctor.BatteryZeroTime <= 0)
            {
                RoleClass.Doctor.Battery = 100;
                RoleClass.Doctor.IsChargingNow = false;
                RoleClass.Doctor.BatteryZeroTime = RoleClass.Doctor.UseTime;
            }
        }
    }
    [Harmony]
    public class VitalsPatch
    {
        //static float vitalsTimer = 0f;
        private static TextMeshPro TimeRemaining;
        private static List<TextMeshPro> hackerTexts = new();

        public static void ResetData()
        {
            //vitalsTimer = 0f;
            if (TimeRemaining != null)
            {
                UObject.Destroy(TimeRemaining);
                TimeRemaining = null;
            }
        }

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
        private class VitalsMinigameStartPatch
        {
            private static void Postfix(VitalsMinigame __instance)
            {
                if (PlayerControl.LocalPlayer.IsRole(RoleId.Doctor))
                {
                    hackerTexts = new();
                    foreach (VitalsPanel panel in __instance.vitals)
                    {
                        TextMeshPro text = UObject.Instantiate(__instance.SabText, panel.transform);
                        hackerTexts.Add(text);
                        UObject.DestroyImmediate(text.GetComponent<AlphaBlink>());
                        text.gameObject.SetActive(false);
                        text.transform.localScale = Vector3.one * 0.75f;
                        text.transform.localPosition = new(-0.75f, -0.23f, 0f);

                    }
                }
            }
        }
        [HarmonyPatch(typeof(Minigame), nameof(Minigame.Close), new Type[] { })]
        private class VitalsMinigameClosePatch
        {
            public static void Prefix(Minigame __instance)
            {
                if (UObject.FindObjectOfType<VitalsMinigame>() && PlayerControl.LocalPlayer.IsRole(RoleId.Doctor))
                {
                    new LateTask(() => RoleClass.Doctor.MyPanelFlag = false, 0.5f, "Doctor flag");
                }
            }
        }
        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
        private class VitalsMinigameUpdatePatch
        {
            private static void Postfix(VitalsMinigame __instance)
            {
                if (PlayerControl.LocalPlayer.IsRole(RoleId.Doctor) && !RoleClass.Doctor.MyPanelFlag)
                {
                    for (int k = 0; k < __instance.vitals.Length; k++)
                    {
                        VitalsPanel vitalsPanel = __instance.vitals[k];
                        GameData.PlayerInfo player = GameData.Instance.AllPlayers[k];
                        if (vitalsPanel.IsDead)
                        {
                            DeadPlayer deadPlayer = DeadPlayer.deadPlayers?.FirstOrDefault(x => x.playerId == player?.PlayerId);
                            if (deadPlayer != null && deadPlayer.timeOfDeath != null && k < hackerTexts.Count && hackerTexts[k] != null)
                            {
                                float timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.timeOfDeath).TotalMilliseconds;
                                hackerTexts[k].gameObject.SetActive(true);
                                hackerTexts[k].text = Math.Round(timeSinceDeath / 1000) + "s";
                            }
                        }
                    }
                }
                else if (PlayerControl.LocalPlayer.IsRole(RoleId.Doctor) && RoleClass.Doctor.MyPanelFlag)
                {
                    if (!RoleClass.Doctor.IsChargingNow)
                    {
                        RoleClass.Doctor.BatteryZeroTime -= Time.deltaTime;
                        if (RoleClass.Doctor.BatteryZeroTime <= 0)
                        {
                            RoleClass.Doctor.Battery = 0;
                            RoleClass.Doctor.IsChargingNow = true;
                            RoleClass.Doctor.BatteryZeroTime = RoleClass.Doctor.ChargeTime;
                            __instance.Close();
                        }
                    }
                }
                else
                {
                    foreach (TextMeshPro text in hackerTexts)
                        if (text != null && text.gameObject != null)
                            text.gameObject.SetActive(false);
                }
            }
        }
    }
}