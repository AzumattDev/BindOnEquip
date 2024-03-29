using System;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using BindOnEquip.Patches;
using ItemDataManager;
using UnityEngine;

namespace BindOnEquip.Utility;

public static class Functions
{
    public static void DefaultSetAllItemData(this ItemDrop.ItemData data, string bind = "default", string uid = "", string playername = "", string bindtime = "", string isbound = "")
    {
        data.Data()[BindOnEquipPlugin.ItemDataKeys.BindOnEquip] = bind;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.SteamID] = uid;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName] = playername;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.BindTime] = bindtime;
        data.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] = isbound;
    }

    public static bool IsCustomDataNull(this ItemDrop.ItemData data)
    {
        return (string.IsNullOrWhiteSpace(data.Data()[BindOnEquipPlugin.ItemDataKeys.BindOnEquip]) ||
                data.Data()[BindOnEquipPlugin.ItemDataKeys.BindOnEquip] == "default") &&
               string.IsNullOrWhiteSpace(data.Data()[BindOnEquipPlugin.ItemDataKeys.SteamID]) &&
               string.IsNullOrWhiteSpace(data.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName]) &&
               string.IsNullOrWhiteSpace(data.Data()[BindOnEquipPlugin.ItemDataKeys.BindTime]);
    }

    public static bool CompareItemData(this ItemDrop.ItemData data, string uid, string playername)
    {
        return data.Data()[BindOnEquipPlugin.ItemDataKeys.SteamID] == uid &&
               data.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName] == playername;
    }
    
    public static bool IsBound(this ItemDrop.ItemData data)
    {
        return data.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] == "true";
    }

    public static BindOnEquipPlugin.ItemCategories? MapToItemCategories(ItemDrop.ItemData.ItemType itemType)
    {
        /* Having to do this dumb shit just because I need an enum that is smaller than the default ItemType one */
        switch (itemType)
        {
            case ItemDrop.ItemData.ItemType.Tool:
                return BindOnEquipPlugin.ItemCategories.Tool;
            case ItemDrop.ItemData.ItemType.OneHandedWeapon:
                return BindOnEquipPlugin.ItemCategories.OneHandedWeapon;
            case ItemDrop.ItemData.ItemType.None:
                return BindOnEquipPlugin.ItemCategories.None;
            case ItemDrop.ItemData.ItemType.Bow:
                return BindOnEquipPlugin.ItemCategories.Bow;
            case ItemDrop.ItemData.ItemType.Shield:
                return BindOnEquipPlugin.ItemCategories.Shield;
            case ItemDrop.ItemData.ItemType.Helmet:
                return BindOnEquipPlugin.ItemCategories.Helmet;
            case ItemDrop.ItemData.ItemType.Chest:
                return BindOnEquipPlugin.ItemCategories.Chest;
            case ItemDrop.ItemData.ItemType.Ammo:
                return BindOnEquipPlugin.ItemCategories.Ammo;
            case ItemDrop.ItemData.ItemType.Legs:
                return BindOnEquipPlugin.ItemCategories.Legs;
            case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
                return BindOnEquipPlugin.ItemCategories.TwoHandedWeapon;
            case ItemDrop.ItemData.ItemType.Torch:
                return BindOnEquipPlugin.ItemCategories.Torch;
            case ItemDrop.ItemData.ItemType.Utility:
                return BindOnEquipPlugin.ItemCategories.Utility;
            case ItemDrop.ItemData.ItemType.Shoulder:
                return BindOnEquipPlugin.ItemCategories.Shoulder;
            case ItemDrop.ItemData.ItemType.TwoHandedWeaponLeft:
                return BindOnEquipPlugin.ItemCategories.TwoHandedWeaponLeft;
            case ItemDrop.ItemData.ItemType.AmmoNonEquipable:
            case ItemDrop.ItemData.ItemType.Material:
            case ItemDrop.ItemData.ItemType.Consumable:
            case ItemDrop.ItemData.ItemType.Customization:
            case ItemDrop.ItemData.ItemType.Misc:
            case ItemDrop.ItemData.ItemType.Hands:
            case ItemDrop.ItemData.ItemType.Trophy:
            case ItemDrop.ItemData.ItemType.Attach_Atgeir:
            case ItemDrop.ItemData.ItemType.Fish:
            default:
                return null;
        }
    }

    public static bool IsIncludedItemType(this ItemDrop.ItemData.SharedData sharedData)
    {
        var mappedCategory = MapToItemCategories(sharedData.m_itemType);
        if (mappedCategory.HasValue)
        {
            return BindOnEquipPlugin.IncludedCategories.Value.HasFlagFast(mappedCategory.Value);
        }

        return false;
    }

    public static bool IsIncludedItem(this ItemDrop.ItemData itemn)
    {
        if (BindOnEquipPlugin.MappedItems.Contains(itemn?.m_dropPrefab?.name))
        {
            return true;
        }
        return false;
    }


    internal static void PatchConfigManager()
    {
        Assembly? bepinexConfigManager = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "ConfigurationManager");

        Type? configManagerType = bepinexConfigManager?.GetType("ConfigurationManager.ConfigurationManager");
        ConfigurationManagerPatch._configManager = configManagerType == null
            ? null
            : BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent(configManagerType);

        void reloadConfigDisplay() => configManagerType?.GetMethod("BuildSettingList")!.Invoke(ConfigurationManagerPatch._configManager, Array.Empty<object>());
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
                textColor = Color.black
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
        ConfigurationManagerPatch._disabledToggleStyle2 = new GUIStyle(GUI.skin.button)
        {
            focused =
            {
                textColor = Color.red,
            },
            normal =
            {
                textColor = Color.red
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
                textColor = Color.green,
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

        int RightColumnWidth = (int)(ConfigurationManagerPatch._configManager?.GetType()
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

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Disable All", ConfigurationManagerPatch._disabledToggleStyle2) && !locked)
        {
            foreach (object? category in Enum.GetValues(typeof(BindOnEquipPlugin.ItemCategories)))
            {
                cfg.BoxedValue = (cfg.BoxedValue as BindOnEquipPlugin.ItemCategories?) &
                                 ~(BindOnEquipPlugin.ItemCategories)category;
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }

    internal static void TrashItem(InventoryGui __instance, Inventory ___m_dragInventory, ItemDrop.ItemData ___m_dragItem, int ___m_dragAmount)
    {
        if (___m_dragAmount == ___m_dragItem.m_stack)
        {
            Player.m_localPlayer.RemoveEquipAction(___m_dragItem);
            Player.m_localPlayer.UnequipItem(___m_dragItem, false);
            ___m_dragInventory.RemoveItem(___m_dragItem);
        }
        else
        {
            ___m_dragInventory.RemoveItem(___m_dragItem, ___m_dragAmount);
        }

        __instance.SetupDragItem(null, null, 0);
        __instance.UpdateCraftingPanel(false);
    }
}