// START OF MIT CODE
using BepInEx;
using BepInEx.IL2CPP;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine.Analytics;
using UnityEngine.CrashReportHandler;

namespace GTFO_R6VanityBandaid
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class R6VanityBandaid : BasePlugin
    {
        public const string PLUGIN_GUID = "dev.aurirex.gtfo.r6vanitybandaid";
        public const string PLUGIN_NAME = "R6 Vanity Bandaid";
        public const string PLUGIN_VERSION = "1.0.0";

        private static R6VanityBandaid _instance;

        private HarmonyLib.Harmony _harmony = new HarmonyLib.Harmony(PLUGIN_GUID);

        private static Patches _patches;

        public override void Load()
        {
            _instance = this;

            CrashReportHandler.SetUserMetadata("Modded", "true");
            CrashReportHandler.enableCaptureExceptions = false;

            Analytics.enabled = false;

            _patches = new Patches();
            _patches.ApplyNative();

            _harmony.PatchAll(typeof(Patches));
            Log.LogMessage("Loaded and patched!");
        }

        // END OF MIT CODE
        // Snippet from https://github.com/Kasuromi/GTFO-API/
        public static unsafe void* GetIl2CppMethod<T>(string methodName, string returnTypeName, bool isGeneric, params string[] argTypes) where T : Il2CppObjectBase
        {
            void** ppMethod = (void**)IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<T>.NativeClassPtr, isGeneric, methodName, returnTypeName, argTypes).ToPointer();
            if ((long)ppMethod == 0) return ppMethod;

            return *ppMethod;
        }
        // START OF MIT CODE

        internal static void LogMsg(string v)
        {
            _instance.Log.LogMessage(v);
        }
    }
}
// END OF MIT CODE