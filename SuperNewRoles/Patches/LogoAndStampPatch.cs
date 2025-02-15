using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using SuperNewRoles.CustomCosmetics;
using SuperNewRoles.Mode;
using SuperNewRoles.Replay;
using TMPro;
using UnityEngine;

namespace SuperNewRoles.Patches;

[HarmonyPatch]
public static class CredentialsPatch
{
    public static string baseCredentials => $@"<size=130%>{SuperNewRolesPlugin.ColorModName}</size> v{SuperNewRolesPlugin.ThisVersion}";

    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    private static class VersionShowerPatch
    {
        public static string modColor = "#a6d289";

        private static void Postfix(VersionShower __instance)
        {
            if (UnityEngine.Object.FindObjectOfType<MainMenuManager>() == null)
                return;
            var credentials = UnityEngine.Object.Instantiate(__instance.text);
            credentials.transform.position = new Vector3(2, -0.15f, 0);
            credentials.transform.localScale = Vector3.one * 2;
            //ブランチ名表示
            string credentialsText = "";
            credentialsText += ModTranslation.GetString("creditsMain");
            credentials.SetText(credentialsText);

            credentials.alignment = TextAlignmentOptions.Center;
            credentials.fontSize *= 0.9f;
            //_ = AutoUpdate.checkForUpdate(credentials);

            var version = UnityEngine.Object.Instantiate(credentials);
            version.transform.position = new Vector3(2, -0.5f, 0);
            version.transform.localScale = Vector3.one * 1.5f;
            version.SetText($"{SuperNewRolesPlugin.ModName} v{SuperNewRolesPlugin.VersionString}");

            //            credentials.transform.SetParent(amongUsLogo.transform);
            //            version.transform.SetParent(amongUsLogo.transform);
        }
    }

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class HudManagerPatch
    {
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            __instance.text.SetOutlineThickness(0.01f);
            if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
            {
                var text = $"{baseCredentials}";
                try
                {
                    if (ModHelpers.IsDebugMode()) text += $"\n{ModTranslation.GetString("DebugModeOn")}";
                    if (!ModeHandler.IsMode(ModeId.Default, ModeId.HideAndSeek))
                        text += $"\n{ModTranslation.GetString("SettingMode")}:{ModeHandler.GetThisModeIntro()}";
                }
                catch { }
                __instance.text.text = $"{text}\n<size=80%><color=#FFB793>Sunday Edition</color></size>";
                __instance.text.alignment = TextAlignmentOptions.TopRight;
                position.Alignment = AspectPosition.EdgeAlignments.RightTop;
                position.DistanceFromEdge = new Vector3(2.7f, 0.1f, 0);
            }
            else
            {
                __instance.text.text = $@"{baseCredentials}
{ModTranslation.GetString("creditsFull")}
<size=80%><color=#FFB793>Sunday Edition</color>
by mxyx-club</size>";
                position.Alignment = AspectPosition.EdgeAlignments.LeftTop;
                __instance.text.alignment = TextAlignmentOptions.TopLeft;
                position.DistanceFromEdge = new(0.4f, 0.06f);
            }
            position.AdjustPosition();
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class LogoPatch
    {
        public static SpriteRenderer renderer;
        public static Sprite bannerSprite;
        // ☆ス☆ー☆パ☆ー☆な☆感☆じ☆の
        // ☆バ☆ナ☆ー☆ス☆プ☆ラ☆イ☆ト
        public static Sprite SuperNakanzinoBannerSprite;
        public static Sprite horseBannerSprite;

        private static IEnumerator ViewBoosterCoro(MainMenuManager __instance)
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (Downloaded)
                {
                    if (__instance != null)
                    {
                        ViewBoosterPatch(__instance);
                    }
                    break;
                }
            }
        }
        public static string DevsData = "";
        public static string SupporterData = "";
        public static string TransData = "";

        public static async Task<HttpStatusCode> FetchBoosters()
        {
            if (!Downloaded)
            {
                Downloaded = true;
                HttpClient http = new();
                http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true, OnlyIfCached = false };
                var response = await http.GetAsync(new Uri($"https://raw.githubusercontent.com/{SuperNewRolesPlugin.ModUrl}/master/CreditsData.json"), HttpCompletionOption.ResponseContentRead);
                try
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        SuperNewRolesPlugin.Logger.LogInfo("NOTOK!!!");
                        return response.StatusCode;
                    };
                    if (response.Content == null)
                    {
                        System.Console.WriteLine("Server returned no data: " + response.StatusCode.ToString());
                        return HttpStatusCode.ExpectationFailed;
                    }
                    string json = await response.Content.ReadAsStringAsync();
                    JToken jobj = JObject.Parse(json);

                    var devs = jobj["Devs"];
                    for (JToken current = devs.First; current != null; current = current.Next)
                    {
                        if (current.HasValues)
                        {
                            DevsData += $"{current["number"]?.ToString()} : {current["name"]?.ToString()}\n";
                        }
                    }

                    var Sponsers = jobj["Supporter"];
                    for (JToken current = Sponsers.First; current != null; current = current.Next)
                    {
                        if (current.HasValues)
                        {
                            SupporterData += current["name"]?.ToString() + "\n";
                        }
                    }

                    var Translator = jobj["Translate"];
                    for (JToken current = Translator.First; current != null; current = current.Next)
                    {
                        if (current.HasValues)
                        {
                            TransData += $"{current["name"]?.ToString()} <size=100%>({current["language"]?.ToString()})</size>\n";
                        }
                    }
                }
                catch (Exception e)
                {
                    SuperNewRolesPlugin.Logger.LogError(e);
                }
            }
            return HttpStatusCode.OK;
        }
        public static GameObject CreditsPopup;

        private static void ViewBoosterPatch(MainMenuManager __instance)
        {
            var template = __instance.transform.FindChild("StatsPopup");
            var obj = UnityEngine.Object.Instantiate(template, template.transform.parent).gameObject;
            obj.name = "CreditsPopup";
            obj.GetComponent<StatsPopup>().SelectableButtons.ToList().ForEach(button => UnityEngine.Object.Destroy(button.gameObject));
            CreditsPopup = obj;
            UnityEngine.Object.Destroy(obj.GetComponent<StatsPopup>());

            CreditsPopup.transform.FindChild("Background").localScale = new Vector3(1.5f, 1f, 1f);
            CreditsPopup.transform.FindChild("CloseButton").localPosition = new Vector3(-3.75f, 2.65f, 0);

            var textobj = CreditsPopup.transform.FindChild("Title_TMP");
            UnityEngine.Object.Destroy(textobj.GetComponent<TextTranslatorTMP>());
            textobj.GetComponent<TextMeshPro>().text = "<size=200%>Credit for SNR</size>";
            textobj.localScale = new Vector3(1.5f, 1.5f, 1f);

            var statsTextTransform = CreditsPopup.transform.FindChild("StatsText_TMP"); // Findの使用回数を減らす為に中身のないStatsTextを複製
            statsTextTransform.gameObject.name = "CreditText_TMP";
            const string titleFormat = $"<size=200%><align={"left"}>{{0}}</align></size>";
            const string textFormat = $"<size=150%><align={"left"}>{{0}}</align></size>";

            var developerTitleText = UnityEngine.Object.Instantiate(statsTextTransform, CreditsPopup.transform);
            developerTitleText.gameObject.name = "DeveloperText";
            developerTitleText.GetComponent<TextMeshPro>().text = string.Format(titleFormat, ModTranslation.GetString("Developer"));
            developerTitleText.position = new Vector3(0.1f, -1.15f, -12f);
            developerTitleText.localPosition = new Vector3(0.1f, -1.15f, -2f);
            developerTitleText.localScale = new Vector3(1.5f, 1.5f, 1f);

            var devText = UnityEngine.Object.Instantiate(developerTitleText, CreditsPopup.transform);
            devText.position = new Vector3(-0.2f, -1.1f, -12f);
            devText.localPosition = new Vector3(-0.2f, -1.1f, -2f);
            devText.localScale = new Vector3(1.25f, 1.25f, 1f);
            devText.GetComponent<TextMeshPro>().text = string.Format(textFormat, DevsData);

            var transTitleText = UnityEngine.Object.Instantiate(statsTextTransform, CreditsPopup.transform);
            transTitleText.gameObject.name = "TranslatorText";
            transTitleText.GetComponent<TextMeshPro>().text = string.Format(titleFormat, ModTranslation.GetString("Translator"));
            transTitleText.position = new Vector3(0.1f, -4.15f, -12f);
            transTitleText.localPosition = new Vector3(0.1f, -4.15f, -2f);
            transTitleText.localScale = new Vector3(1.5f, 1.5f, 1f);

            var transText = UnityEngine.Object.Instantiate(transTitleText, CreditsPopup.transform);
            transText.position = new Vector3(-0.2f, -4.1f, -12f);
            transText.localPosition = new Vector3(-0.2f, -4.1f, -2f);
            transText.localScale = new Vector3(1.25f, 1.25f, 1f);
            transText.GetComponent<TextMeshPro>().text = string.Format(textFormat, TransData);

            // サポーターは現在不在

            UnityEngine.Object.Destroy(statsTextTransform.gameObject); // 用済みなオブジェクトを削除
        }

        private static bool Downloaded = false;
        public static MainMenuManager instance;

        public static void Postfix(MainMenuManager __instance)
        {
            DownLoadCustomCosmetics.CosmeticsLoad();
            AprilFoolsManager.SetRandomModMode();

            __instance.gameModeButtons.GetComponent<AspectPosition>().DistanceFromEdge = new(0, 0, -5);
            if (AprilFoolsManager.IsApril(2024))
            {
                __instance.accountButtons.GetComponent<AspectPosition>().DistanceFromEdge = new(0, 0, -5);
            }

            __instance.StartCoroutine(Blacklist.FetchBlacklist().WrapToIl2Cpp());
            AmongUsClient.Instance.StartCoroutine(CustomRegulation.FetchRegulation().WrapToIl2Cpp());
            /*if (ConfigRoles.IsUpdated)
            {
                __instance.StartCoroutine(ShowAnnouncementPopUp(__instance).WrapToIl2Cpp());
            }*/


            instance = __instance;

            AmongUsClient.Instance.StartCoroutine(ViewBoosterCoro(__instance).WrapToIl2Cpp());

            //ViewBoosterPatch(__instance);

            FastDestroyableSingleton<ModManager>.Instance.ShowModStamp();

            var amongUsLogo = GameObject.Find("bannerLogo_AmongUs");
            if (amongUsLogo != null)
            {
                amongUsLogo.transform.localScale *= 0.6f;
                amongUsLogo.transform.position += Vector3.up * 0.25f;
            }

            var snrLogo = new GameObject("bannerLogo");
            snrLogo.transform.position = new(2, AprilFoolsManager.getCurrentBannerYPos(), AprilFoolsManager.IsApril(2024) ? -6 : 0);
            snrLogo.transform.localScale = Vector3.one * 0.95f;
            //snrLogo.transform.localScale = Vector3.one;
            renderer = snrLogo.AddComponent<SpriteRenderer>();

            LoadSprites();
            renderer.sprite = bannerRendSprite;
            __instance.howToPlayButton.transform.localPosition = new(-1.925f, -1.75f, 0);
            PassiveButton FreePlayButton = __instance.howToPlayButton.transform.parent.FindChild("FreePlayButton").GetComponent<PassiveButton>();
            FreePlayButton.transform.localPosition = new(-0.05f, -1.75f, 0);
            ReplayManager.CreateReplayButton(__instance, FreePlayButton);
        }

        public static void LoadSprites()
        {
            if (bannerSprite == null) bannerSprite = AssetManager.GetAsset<Sprite>("banner.png");
            if (SuperNakanzinoBannerSprite == null) SuperNakanzinoBannerSprite = AssetManager.GetAsset<Sprite>("banner_April.png");
            if (horseBannerSprite == null) horseBannerSprite = AssetManager.GetAsset<Sprite>("SuperHorseRoles.png");
        }

        public static Sprite bannerRendSprite
        {
            get
            {
                //if (HorseModeOption.enableHorseMode) return horseBannerSprite;
                Sprite aprilBannerSprite = AprilFoolsManager.getCurrentBanner();
                //if (AprilFoolsManager.IsApril(2023)
                //    return SuperNakanzinoBannerSprite;
                return aprilBannerSprite != null ? aprilBannerSprite : bannerSprite;
            }
        }

        public static void UpdateSprite()
        {
            LoadSprites();
            if (renderer != null)
            {
                float fadeDuration = 1f;
                AmongUsClient.Instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) =>
                {
                    renderer.color = new Color(1, 1, 1, 1 - p);
                    if (p == 1)
                    {
                        renderer.sprite = bannerRendSprite;
                        AmongUsClient.Instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) =>
                        {
                            renderer.color = new Color(1, 1, 1, p);
                        })));
                    }
                })));
            }
        }
    }
}