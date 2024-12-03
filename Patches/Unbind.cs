using BindOnEquip;
using BindOnEquip.Utility;
using HarmonyLib;
using ItemDataManager;

[HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.OnSelectedItem))]
static class InventoryGuiOnSelectedItemPatch
{
    const string UnbindItem = "$item_azu_unbinder";
    static bool Prefix(InventoryGui __instance, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos, InventoryGrid.Modifier mod)
    {
        // item will be the thing that we are trying to unbind
        if (__instance.m_dragGo == null) return true;
        if (item == null) return true;
        if (__instance.m_dragItem == null) return true;
        // If the m_dragItem is an unbinder, then we want to unbind the item
        if (__instance.m_dragItem.m_shared.m_name != UnbindItem) return true;
        if (item.m_shared.m_name == UnbindItem) return true;
        if (!item.IsBound()) return true;
        if (item.Data()[BindOnEquipPlugin.ItemDataKeys.PlayerName] != Game.instance.GetPlayerProfile().m_playerName || item.Data()[BindOnEquipPlugin.ItemDataKeys.SteamID] != CommonMethods.GetPlatformUserId()) return true;
        // Unbind the item
        BindOnEquipPlugin.BindOnEquipLogger.LogDebug("Unbinding item");
        item.DefaultSetAllItemData();
        // Remove the Unbinder from the inventory
        Functions.TrashItem(__instance, grid.GetInventory() == __instance.m_dragInventory
            ? grid.GetInventory()
            : __instance.m_dragInventory, __instance.m_dragItem, 1);
        return false;
    }
}