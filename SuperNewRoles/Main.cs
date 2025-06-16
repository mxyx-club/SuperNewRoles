global using System.Linq;
global using System.Reflection;
global using AmongUs.Data;
global using HarmonyLib;
global using Hazel;
global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.Injection;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;
global using InnerNet;
global using SuperNewRoles.Modules;
global using TMPro;
global using static SuperNewRoles.Logger;
global using ISystem = Il2CppSystem.Collections.Generic;
global using UObject = UnityEngine.Object;
global using URandom = UnityEngine.Random;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using SuperNewRoles.CustomObject;
using SuperNewRoles.Roles.Role;
using SuperNewRoles.Roles.RoleBases;
using SuperNewRoles.WaveCannonObj;
using UnityEngine;

namespace SuperNewRoles;

[BepInAutoPlugin("jp.ykundesu.supernewroles", "SuperNewRoles")]
[BepInIncompatibility("com.emptybottle.townofhost")]
[BepInIncompatibility("me.eisbison.theotherroles")]
[BepInIncompatibility("me.yukieiji.extremeroles")]
[BepInIncompatibility("com.tugaru.TownOfPlus")]
[BepInProcess("Among Us.exe")]
public partial class SuperNewRolesPlugin : BasePlugin
{
    public static string ModName => Name;
    public const string VersionSuffix = "";
    public static Version version => System.Version.Parse(Version);

    public const bool IsSecretBranch = false; // プルリク時にtrueなら指摘してください
    public const bool IsHideText = false; // プルリク時にtrueなら指摘してください

    public static Assembly assembly => _assembly ??= Assembly.GetExecutingAssembly();
    private static Assembly _assembly;

    public static string ColorModName => AprilFoolsManager.getCurrentModNameOnColor();

    public static Version ThisVersion = System.Version.Parse(Version);
    public static BepInEx.Logging.ManualLogSource Logger;
    public static Sprite ModStamp;
    public static int optionsPage = 1;
    public static int optionsMaxPage;
    public Harmony Harmony { get; } = new(Id);
    public static SuperNewRolesPlugin Instance;
    public static bool IsUpdate;
    public static string NewVersion = "";
    public static string thisname;
    public static string ThisPluginModName;
    //対応しているバージョン。nullなら全て。
    public static string[] SupportVanilaVersion = ["2024.3.5", "2024.6.4"];

    public override void Load()
    {
        if (ConsoleManager.ConsoleEnabled) System.Console.OutputEncoding = Encoding.UTF8;
        Logger = Log;
        Instance = this;

        Task LoadHarmonyPatchTask = Task.Run(() =>
        {
            Logger.LogInfo("Start Patch Harmony");
            Harmony.PatchAll();
            Logger.LogInfo("End Patch Harmony");
        });
        ModTranslation.LoadCsv();
        bool CreatedVersionPatch = false;

        //初期状態ではRoleInfoやOptionInfoなどが読み込まれていないため、
        //ここで読み込む
        Type RoleInfoType = typeof(RoleInfo);
        Type RoleBaseType = typeof(RoleBase);
        _ = Assembly.GetAssembly(RoleBaseType)
        .GetTypes()
        .Where(t =>
        {
            if (t.IsSubclassOf(RoleBaseType))
            {
                foreach (FieldInfo field in t.GetFields())
                {
                    if (field.IsStatic && field.FieldType == RoleInfoType)
                        field.GetValue(null);
                }
            }
            return false;
        });

        //SetNonVanilaVersionPatch();
        // All Load() Start
        OptionSaver.Load();
        ConfigRoles.Load();
        UpdateCPUProcessorAffinity();
        ContentManager.Load();
        //WebAccountManager.SetToken("XvSwpZ8CsQgEksBg");
        ChacheManager.Load();
        CustomCosmetics.CustomColors.Load();
        CustomOptionHolder.Load();
        LegacyOptionDataMigration.Load();
        // All Load() End

        Logger.LogInfo(ModTranslation.GetString("\n---------------\nSuperNewRoles\n" + ModTranslation.GetString("StartLogText") + "\n---------------"));

        ThisPluginModName = IL2CPPChainloader.Instance.Plugins.FirstOrDefault(x => x.Key == "jp.ykundesu.supernewroles").Value.Metadata.Name;

        //Register Il2cpp
        ClassInjector.RegisterTypeInIl2Cpp<CustomAnimation>();
        ClassInjector.RegisterTypeInIl2Cpp<WormHole>();
        ClassInjector.RegisterTypeInIl2Cpp<SluggerDeadbody>();
        ClassInjector.RegisterTypeInIl2Cpp<WaveCannonObject>();
        ClassInjector.RegisterTypeInIl2Cpp<RocketDeadbody>();
        ClassInjector.RegisterTypeInIl2Cpp<SpiderTrap>();
        ClassInjector.RegisterTypeInIl2Cpp<WCSantaHandler>();
        ClassInjector.RegisterTypeInIl2Cpp<PushedPlayerDeadbody>();
        ClassInjector.RegisterTypeInIl2Cpp<WaveCannonEffect>();

        Logger.LogInfo("Start Load Resource");
        string[] resourceNames = assembly.GetManifestResourceNames();
        foreach (string resourceName in resourceNames)
        {
            if (resourceName.EndsWith(".png") && resourceName.Contains("_"))
            {
                ModHelpers.LoadSpriteFromResources(resourceName, 115f);
            }
        }
        AssetManager.Load();

        Logger.LogInfo("Resource Loaded");

        Logger.LogInfo("Start WaitLoad");
        // ロードが終わってないなら待つ
        LoadHarmonyPatchTask.Wait();
    }

    // CPUの割当を0と1にする
    public static void UpdateCPUProcessorAffinity()
    {
        if (!ConfigRoles._isCPUProcessorAffinity.Value)
        {
            Logger.LogWarning("UpdateCPUProcessorAffinity: IsCPUProcessorAffinity is false");
            return;
        }
        Logger.LogInfo("Start UpdateCPUProcessorAffinity");
        if (Environment.ProcessorCount > 1)
        {
            int affinity = 1;
            for (int i = 1; i < 2; i++)
            {
                affinity |= 1 << i;
            }
            System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)affinity;
        }
        Logger.LogInfo("End UpdateCPUProcessorAffinity");
    }
    // https://github.com/yukieiji/ExtremeRoles/blob/master/ExtremeRoles/Patches/Manager/AuthManagerPatch.cs
    [HarmonyPatch(typeof(AuthManager), nameof(AuthManager.CoConnect))]
    public static class AuthManagerCoConnectPatch
    {
        public static bool Prefix(AuthManager __instance)
        {
            if (!ModHelpers.IsCustomServer() ||
                FastDestroyableSingleton<ServerManager>.Instance.CurrentRegion.Servers.Any(x => x.UseDtls))
                return true;
            if (__instance.connection != null)
                __instance.connection.Dispose();
            __instance.connection = null;
            return false;
        }
    }

    [HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
    private class GetBroadcastVersionPatch
    {
        public static void Postfix(ref int __result)
        {
            if (AmongUsClient.Instance.NetworkMode is NetworkModes.LocalGame or NetworkModes.FreePlay) return;
            __result += 25;
        }
    }
    [HarmonyPatch(typeof(Constants), nameof(Constants.IsVersionModded))]
    public static class ConstantsVersionModdedPatch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
    public static class AmBannedPatch
    {
        public static void Postfix(out bool __result) => __result = false;
    }
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class ChatControllerAwakePatch
    {
        public static void Prefix()
        {
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
        }
        public static void Postfix(ChatController __instance)
        {
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;

            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (!__instance.isActiveAndEnabled) return;
                __instance.Toggle();
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                __instance.SetVisible(false);
                new LateTask(() =>
                {
                    __instance.SetVisible(true);
                }, 0f, "AntiChatBug");
            }
            if (__instance.IsOpenOrOpening)
            {
                __instance.banButton.MenuButton.enabled = !__instance.IsAnimating;
            }
        }
    }
    public static void AgarthaLoad() => Agartha.AgarthaPlugin.Instance.Log.LogInfo("アガルタやで");
}