﻿using BepInEx.Configuration;

using UnityEngine;

namespace Gizmo {
  public static class PluginConfig {
    public static ConfigEntry<int> SnapDivisions { get; private set; }

    public static ConfigEntry<KeyboardShortcut> XRotationKey;
    public static ConfigEntry<KeyboardShortcut> ZRotationKey;
    public static ConfigEntry<KeyboardShortcut> ResetRotationKey;
    public static ConfigEntry<KeyboardShortcut> ResetAllRotationKey;

    public static ConfigEntry<bool> ShowGizmoPrefab;

    public static void BindConfig(ConfigFile config) {
      SnapDivisions =
          config.Bind(
              "Gizmo",
              "snapDivisions",
              16,
              new ConfigDescription(
                  "Number of snap angles per 180 degrees. Vanilla uses 8.",
                  new AcceptableValueRange<int>(2, 128)));

      XRotationKey =
          config.Bind(
              "Keys",
              "xRotationKey",
              new KeyboardShortcut(KeyCode.LeftShift),
              "Hold this key to rotate on the x-axis/plane (red circle).");

      ZRotationKey =
          config.Bind(
              "Keys",
              "zRotationKey",
              new KeyboardShortcut(KeyCode.LeftAlt),
              "Hold this key to rotate on the z-axis/plane (blue circle).");

      ResetRotationKey =
          config.Bind(
              "Keys",
              "resetRotationKey",
              new KeyboardShortcut(KeyCode.V),
              "Press this key to reset the selected axis to zero rotation.");

      ResetAllRotationKey =
          config.Bind(
              "Keys",
              "resetAllRotationKey",
              KeyboardShortcut.Empty,
              "Press this key to reset _all axis_ rotations to zero rotation.");

      ShowGizmoPrefab = config.Bind("UI", "showGizmoPrefab", true, "Show the Gizmo prefab in placement mode.");
    }
  }
}
