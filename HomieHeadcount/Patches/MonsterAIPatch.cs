﻿using HarmonyLib;

using static HomieHeadcount.PluginConfig;
using static HomieHeadcount.HomieHeadcount;
using HomieHeadcount.Core;

namespace HomieHeadcount.Patches {
  [HarmonyPatch(typeof(MonsterAI))]
  static class MonsterAIPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MonsterAI.SetFollowTarget))]
    public static void SetFollowTargetPostfix(MonsterAI __instance) {
      if (!IsModEnabled.Value
          || !__instance
          || __instance.name.GetStableHashCode() != SkeletonAiNameHashCode
          || !__instance.m_follow
          || !__instance.m_follow.TryGetComponent(out Player player)
          || !__instance.TryGetComponent(out Tameable tameable)
          || player.GetPlayerID() != Player.m_localPlayer.GetPlayerID()) {

        return;
      }

      HomieCounter.Add(tameable);
      PanelManager.AddHomie(tameable);
    }
  }
}
