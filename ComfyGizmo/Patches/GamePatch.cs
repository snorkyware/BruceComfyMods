using HarmonyLib;

namespace ComfyGizmo
{
    [HarmonyPatch(typeof(Game))]
    internal static class GamePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Game.Start))]
        private static void StartPostfix()
        {
            RotationManager.Initialize();
        }
    }
}
