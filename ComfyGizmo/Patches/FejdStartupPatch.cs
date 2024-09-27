using HarmonyLib;

namespace ComfyGizmo
{
    [HarmonyPatch(typeof(FejdStartup))]
    internal static class FejdStartupPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(FejdStartup.Awake))]
        private static void AwakePostfix()
        {
            HammerTableManager.Initialize();
        }
    }
}
