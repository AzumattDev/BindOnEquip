using System;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using BindOnEquip.Managers;
using BindOnEquip.Patches;
using UnityEngine;

namespace BindOnEquip.Utility;

public static class Functions
{
    public static void SetAllItemData(this ItemDrop.ItemData data, string bind, string uid, string playername,
        string bindtime, string isbound)
    {
        data.Data()[BindOnEquipPlugin.ItemDataKeys.BindOnEquip] = bind;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.SteamID] = uid;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName] = playername;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.BindTime] = bindtime;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] = isbound;
    }

    public static void DefaultSetAllItemData(this ItemDrop.ItemData data, string bind, string uid, string playername,
        string bindtime, string isbound)
    {
        data.Data()[BindOnEquipPlugin.ItemDataKeys.BindOnEquip] = bind;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.SteamID] = uid;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName] = playername;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.BindTime] = bindtime;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] = isbound;
    }

    public static bool IsCustomDataNull(this ItemDrop.ItemData data)
    {
        return (string.IsNullOrWhiteSpace(data.Data()[BindOnEquipPlugin.ItemDataKeys.BindOnEquip]) || data.Data()[BindOnEquipPlugin.ItemDataKeys.BindOnEquip] == "default") &&
               string.IsNullOrWhiteSpace(data.Data()[BindOnEquipPlugin.ItemDataKeys.SteamID]) &&
               string.IsNullOrWhiteSpace(data.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName]) &&
               string.IsNullOrWhiteSpace(data.Data()[BindOnEquipPlugin.ItemDataKeys.BindTime]);
    }

    public static bool CompareItemData(this ItemDrop.ItemData data, string uid, string playername)
    {
        BindOnEquipPlugin.BindOnEquipLogger.LogWarning(
            $"Comparing data: {data.Data()[BindOnEquipPlugin.ItemDataKeys.SteamID]} == {uid} && {data.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName]} == {playername}");
        return data.Data()[BindOnEquipPlugin.ItemDataKeys.SteamID] == uid &&
               data.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName] == playername;
    }

    public static bool IsIncludedItemType(this ItemDrop.ItemData.SharedData sharedData)
    {
        return BindOnEquipPlugin.IncludedCategories.Value.HasFlagFast(
            (BindOnEquipPlugin.ItemCategories)Enum.Parse(typeof(BindOnEquipPlugin.ItemCategories),
                sharedData.m_itemType.ToString()));
    }

    internal static void PatchConfigManager()
    {
        Assembly? bepinexConfigManager = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "ConfigurationManager");

        Type? configManagerType = bepinexConfigManager?.GetType("ConfigurationManager.ConfigurationManager");
        ConfigurationManagerPatch._configManager = configManagerType == null
            ? null
            : BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent(configManagerType);

        void reloadConfigDisplay() =>
            configManagerType?.GetMethod("BuildSettingList")!.Invoke(ConfigurationManagerPatch._configManager,
                Array.Empty<object>());
    }

    internal static void DrawCategoriesTable(ConfigEntryBase cfg)
    {
        ConfigurationManagerPatch._disabledToggleStyle = new GUIStyle(GUI.skin.button)
        {
            focused =
            {
                textColor = Color.red,
            },
            normal =
            {
                textColor = Color.gray
            },
            active =
            {
                textColor = Color.red
            },
            onHover = { textColor = Color.red },
            onActive = { textColor = Color.red },
            onFocused = { textColor = Color.red },
            onNormal = { textColor = Color.red },
        };
        ConfigurationManagerPatch._enabledToggleStyle = new GUIStyle(GUI.skin.button)
        {
            stretchWidth = true,
            normal =
            {
                textColor = Color.green
            },
            focused =
            {
                textColor = Color.green
            },
            active =
            {
                textColor = Color.green
            },
            onHover =
            {
                textColor = Color.green
            },
            onActive =
            {
                textColor = Color.green
            },
            onFocused =
            {
                textColor = Color.green
            },
            onNormal =
            {
                textColor = Color.green
            },
        };

        bool locked = cfg.Description.Tags
            .Select(a =>
                a.GetType().Name == "ConfigurationManagerAttributes"
                    ? (bool?)a.GetType().GetField("ReadOnly")?.GetValue(a)
                    : null).FirstOrDefault(v => v != null) ?? false;

        bool wasUpdated = false;

        int RightColumnWidth =
            (int)(ConfigurationManagerPatch._configManager?.GetType()
                .GetProperty("RightColumnWidth", BindingFlags.Instance | BindingFlags.NonPublic)!.GetGetMethod(true)
                .Invoke(ConfigurationManagerPatch._configManager, Array.Empty<object>()) ?? 130);
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace(); // Because there are only 19 enum values that show, make the first one stretch.
        // Build GUI toggle buttons for all categories in BindOnEquipPlugin.ItemCategories enum. This is a bit field enum, so we can use bitwise operations to check if a category is enabled. 2 toggles across
        for (int i = 0; i < Enum.GetValues(typeof(BindOnEquipPlugin.ItemCategories)).Length; ++i)
        {
            if (i % 2 == 0)
            {
                GUILayout.BeginHorizontal();
            }

            BindOnEquipPlugin.ItemCategories category =
                (BindOnEquipPlugin.ItemCategories)Enum.GetValues(typeof(BindOnEquipPlugin.ItemCategories))
                    .GetValue(i);
            if (category == BindOnEquipPlugin.ItemCategories.None) continue;
            bool enabled =
                (cfg.BoxedValue as BindOnEquipPlugin.ItemCategories? ?? BindOnEquipPlugin.ItemCategories.None)
                .HasFlagFast(category);
            if (GUILayout.Button(category.ToString(),
                    enabled
                        ? ConfigurationManagerPatch._enabledToggleStyle
                        : ConfigurationManagerPatch._disabledToggleStyle) && !locked)
            {
                cfg.BoxedValue = (BindOnEquipPlugin.ItemCategories)cfg.BoxedValue ^ category;
            }

            if (i % 2 == 1 || i == Enum.GetValues(typeof(BindOnEquipPlugin.ItemCategories)).Length - 1)
            {
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Disable All", ConfigurationManagerPatch._disabledToggleStyle) && !locked)
        {
            foreach (object? category in Enum.GetValues(typeof(BindOnEquipPlugin.ItemCategories)))
            {
                cfg.BoxedValue = (cfg.BoxedValue as BindOnEquipPlugin.ItemCategories?) &
                                 ~(BindOnEquipPlugin.ItemCategories)category;
            }
        }
    }
}