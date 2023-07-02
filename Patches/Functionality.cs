using System;
using System.Text;
using BindOnEquip.Utility;
using HarmonyLib;
using ItemDataManager;
using UnityEngine;

namespace BindOnEquip.Patches;

[HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip), typeof(ItemDrop.ItemData), typeof(int),
    typeof(bool))]
static class ItemDropItemDataGetTooltipPatch
{
    static void Postfix(ItemDrop.ItemData item, int qualityLevel, bool crafting, ref string __result)
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
                else if (isBound == "true" && !crafting)
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
        return CommonMethods.CheckItemData(__instance.m_itemData, isEquipAction: false);
    }
}

[HarmonyPatch(typeof(Inventory), nameof(Inventory.CanAddItem), typeof(ItemDrop.ItemData), typeof(int))]
static class InventoryCanAddItempatch
{
    static bool Prefix(Inventory __instance, ItemDrop.ItemData item, int stack = -1)
    {
        return CommonMethods.CheckItemData(item, false, false);
    }
}

[HarmonyPatch(typeof(Inventory), nameof(Inventory.AddItem), typeof(ItemDrop.ItemData))]
static class InventoryAddItemPatch
{
    static bool Prefix(Inventory __instance, ItemDrop.ItemData item)
    {
        return CommonMethods.CheckItemData(item, true, false);
    }
}

[HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.OnSelectedItem))]
static class InventoryGuiOnSelectedItemPatch
{
    static bool Prefix(InventoryGui __instance, InventoryGrid grid,
        ItemDrop.ItemData item,
        Vector2i pos,
        InventoryGrid.Modifier mod)
    {
        return CommonMethods.CheckItemData(__instance.m_dragItem, true, false);
    }
}

[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
static class HumanoidEquipItemPatch
{
    static bool Prefix(ref Humanoid __instance, ref bool __result, ItemDrop.ItemData? item,
        bool triggerEquipEffects = true)
    {
        if (__instance.IsPlayer())
        {
            if (item != null)
            {
                return CommonMethods.CheckItemData(item);
            }

            return true;
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
        return CommonMethods.CheckItemData(item);
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
                if (!itemData.m_shared.IsIncludedItemType()) continue;
                if (itemData.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] != "true")
                {
                    itemData.DefaultSetAllItemData();
                }
            }
        }
    }
}