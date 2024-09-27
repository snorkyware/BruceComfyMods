﻿using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace ComfyLib
{
    public static class ConfigFileExtensions
    {
        private static readonly Dictionary<string, int> _sectionToSettingOrder = new();

        private static int GetSettingOrder(string section)
        {
            if (!_sectionToSettingOrder.TryGetValue(section, out var order)) order = 0;

            _sectionToSettingOrder[section] = order - 1;
            return order;
        }

        public static ConfigEntry<T> BindInOrder<T>(
            this ConfigFile config,
            string section,
            string key,
            T defaultValue,
            string description,
            AcceptableValueBase acceptableValues,
            bool browsable = true,
            bool hideDefaultButton = false,
            bool hideSettingName = false,
            bool isAdvanced = false,
            bool readOnly = false)
        {
            return config.Bind(
                section,
                key,
                defaultValue,
                new ConfigDescription(
                    description,
                    acceptableValues,
                    new ConfigurationManagerAttributes
                    {
                        Browsable = browsable,
                        CustomDrawer = default,
                        HideDefaultButton = hideDefaultButton,
                        HideSettingName = hideSettingName,
                        IsAdvanced = isAdvanced,
                        Order = GetSettingOrder(section),
                        ReadOnly = readOnly
                    }));
        }

        public static ConfigEntry<T> BindInOrder<T>(
            this ConfigFile config,
            string section,
            string key,
            T defaultValue,
            string description,
            Action<ConfigEntryBase> customDrawer = null,
            bool browsable = true,
            bool hideDefaultButton = false,
            bool hideSettingName = false,
            bool isAdvanced = false,
            bool readOnly = false)
        {
            return config.Bind(
                section,
                key,
                defaultValue,
                new ConfigDescription(
                    description,
                    default,
                    new ConfigurationManagerAttributes
                    {
                        Browsable = browsable,
                        CustomDrawer = customDrawer,
                        HideDefaultButton = hideDefaultButton,
                        HideSettingName = hideSettingName,
                        IsAdvanced = isAdvanced,
                        Order = GetSettingOrder(section),
                        ReadOnly = readOnly
                    }));
        }

        public static void OnSettingChanged<T>(this ConfigEntry<T> configEntry, Action settingChangedHandler)
        {
            configEntry.SettingChanged += (_, _) => settingChangedHandler();
        }

        public static void OnSettingChanged<T>(this ConfigEntry<T> configEntry, Action<T> settingChangedHandler)
        {
            configEntry.SettingChanged +=
                (_, eventArgs) =>
                    settingChangedHandler((T)((SettingChangedEventArgs)eventArgs).ChangedSetting.BoxedValue);
        }

        public static void OnSettingChanged<T>(
            this ConfigEntry<T> configEntry, Action<ConfigEntry<T>> settingChangedHandler)
        {
            configEntry.SettingChanged +=
                (_, eventArgs) =>
                    settingChangedHandler(
                        (ConfigEntry<T>)((SettingChangedEventArgs)eventArgs).ChangedSetting.BoxedValue);
        }

        internal sealed class ConfigurationManagerAttributes
        {
            public bool? Browsable;
            public Action<ConfigEntryBase> CustomDrawer;
            public bool? HideDefaultButton;
            public bool? HideSettingName;
            public bool? IsAdvanced;
            public int? Order;
            public bool? ReadOnly;
        }
    }
}
