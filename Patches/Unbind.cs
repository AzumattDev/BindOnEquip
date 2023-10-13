using BindOnEquip;
using BindOnEquip.Utility;
using HarmonyLib;

[HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.OnSelectedItem))]
static class InventoryGuiOnSelectedItemPatch
{
    static bool Prefix(InventoryGui __instance, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos, InventoryGrid.Modifier mod)
    {
        // item will be the thing that we are trying to unbind
        if (__instance.m_dragGo == null) return true;
        if (item == null) return true;
        if (__instance.m_dragItem == null) return true;
        // If the m_dragItem is a repair kit, then we want to repair the item
        if (__instance.m_dragItem.m_shared.m_name != "$item_azu_unbinder") return true;
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