using System;
using System.Text;
using BindOnEquip.Managers;
using BindOnEquip.Utility;
using HarmonyLib;
using UnityEngine;

namespace BindOnEquip.Patches;

[HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip), typeof(ItemDrop.ItemData), typeof(int),
    typeof(bool))]
static class ItemDropItemDataGetTooltipPatch
{
    static void Postfix(ItemDrop.ItemData item, ref string __result)
    {
        if (item?.m_dropPrefab is { } prefab)
        {
            if (item.m_shared.IsIncludedItemType())
            {
                string bindOnEquip = item.Data()[BindOnEquipPlugin.ItemDataKeys.BindOnEquip];
                string isBound = item.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound];
                StringBuilder sb = new StringBuilder($"{Environment.NewLine}");

                if ((bindOnEquip == "default" || isBound != "true") && isBound != "true")
                {
                    sb.Append($"{Environment.NewLine}<color=orange>$item_binds_on_equip</color>");
                }
                else if (isBound == "true")
                {
                    sb.Append(
                        $"{Environment.NewLine}<color=#FF0000>$item_isbound: {item.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName]}</color>");
                }

                if (sb.Length > Environment.NewLine.Length)
                {
                    __result += Localization.instance.Localize(sb.ToString());
                }
            }
        }
    }
}

[HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.Pickup))]
static class ItemDropInteractPatch
{
    static bool Prefix(ItemDrop __instance)
    {
        if (Player.m_localPlayer == null || Player.m_localPlayer.m_isLoading) return true;

        var item = __instance.m_itemData;

        if (item != null && item.m_shared.IsIncludedItemType())
        {
            if (item.IsCustomDataNull())
            {
                BindOnEquipPlugin.BindOnEquipLogger.LogDebug("Custom data is null, setting default values");
                string id = PrivilegeManager.GetCurrentPlatform() == PrivilegeManager.Platform.Steam
                    ? Steamworks.SteamUser.GetSteamID().ToString()
                    : PrivilegeManager.GetNetworkUserId();
                item.SetAllItemData("true", id, Game.instance.GetPlayerProfile().m_playerName, DateTime.Now.ToString(),
                    "true");
            }

            if (item.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] == "true")
            {
                BindOnEquipPlugin.BindOnEquipLogger.LogDebug($"Item {item.m_shared.m_name} is bound, comparing data");
                string id = PrivilegeManager.GetCurrentPlatform() == PrivilegeManager.Platform.Steam
                    ? Steamworks.SteamUser.GetSteamID().ToString()
                    : PrivilegeManager.GetNetworkUserId();

                if (!item.CompareItemData(id, Game.instance.GetPlayerProfile().m_playerName))
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center,
                        Localization.instance.Localize("$item_binds_on_equip_denymessage"));
                    return false;
                }
            }
        }

        return true;
    }
}

[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
static class HumanoidEquipItemPatch
{
    static bool Prefix(ref Humanoid __instance, ref bool __result, ItemDrop.ItemData? item,
        bool triggerEquipEffects = true)
    {
        if (Player.m_localPlayer == null || !__instance.IsPlayer() || Player.m_localPlayer.m_isLoading) return true;

        if (item != null && item.m_shared.IsIncludedItemType())
        {
            if (item.IsCustomDataNull())
            {
                BindOnEquipPlugin.BindOnEquipLogger.LogDebug("Custom data is null, setting default values");
                string id = PrivilegeManager.GetCurrentPlatform() == PrivilegeManager.Platform.Steam
                    ? Steamworks.SteamUser.GetSteamID().ToString()
                    : PrivilegeManager.GetNetworkUserId();
                item.SetAllItemData("true", id, Game.instance.GetPlayerProfile().m_playerName, DateTime.Now.ToString(),
                    "true");
            }
            else if (item.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] == "true")
            {
                BindOnEquipPlugin.BindOnEquipLogger.LogDebug($"Item {item.m_shared.m_name} is bound, comparing data");
                string id = PrivilegeManager.GetCurrentPlatform() == PrivilegeManager.Platform.Steam
                    ? Steamworks.SteamUser.GetSteamID().ToString()
                    : PrivilegeManager.GetNetworkUserId();

                if (!item.CompareItemData(id, Game.instance.GetPlayerProfile().m_playerName))
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center,
                        Localization.instance.Localize("$item_binds_on_equip_denymessage"));
                    return false;
                }
            }
        }

        return true;
    }
}

[HarmonyPatch(typeof(InventoryGrid), nameof(InventoryGrid.OnRightClick))]
static class InventoryGridOnRightClickPatch
{
    static bool Prefix(InventoryGrid __instance, UIInputHandler element)
    {
        Vector2i buttonPos = __instance.GetButtonPos(element.gameObject);
        ItemDrop.ItemData item = __instance.m_inventory.GetItemAt(buttonPos.x, buttonPos.y);
        if (__instance.m_onRightClick == null)
            return false;
        if (Player.m_localPlayer == null || Player.m_localPlayer.m_isLoading) return true;
        if (item != null && item.m_shared.IsIncludedItemType())
        {
            if (item.IsCustomDataNull())
            {
                BindOnEquipPlugin.BindOnEquipLogger.LogDebug("Custom data is null, setting default values");
                string id = PrivilegeManager.GetCurrentPlatform() == PrivilegeManager.Platform.Steam
                    ? Steamworks.SteamUser.GetSteamID().ToString()
                    : PrivilegeManager.GetNetworkUserId();
                item.SetAllItemData("true", id, Game.instance.GetPlayerProfile().m_playerName, DateTime.Now.ToString(),
                    "true");
            }
            else if (item.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] == "true")
            {
                BindOnEquipPlugin.BindOnEquipLogger.LogDebug($"Item {item.m_shared.m_name} is bound, comparing data");
                string id = PrivilegeManager.GetCurrentPlatform() == PrivilegeManager.Platform.Steam
                    ? Steamworks.SteamUser.GetSteamID().ToString()
                    : PrivilegeManager.GetNetworkUserId();

                if (!item.CompareItemData(id, Game.instance.GetPlayerProfile().m_playerName))
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center,
                        Localization.instance.Localize("$item_binds_on_equip_denymessage"));
                    return false;
                }
            }
        }

        return true;
    }
}

[HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
static class ObjectDBAwakePatch
{
    static void Postfix(ObjectDB __instance)
    {
        foreach (GameObject itemObject in __instance.m_items)
        {
            ItemDrop itemDrop = itemObject.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                ItemDrop.ItemData itemData = itemDrop.m_itemData;
                if (itemData.m_shared.IsIncludedItemType()) continue;
                if (itemData.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] != "true")
                {
                    itemData.DefaultSetAllItemData("default", "", "", "", "");
                }
            }
        }
    }
}