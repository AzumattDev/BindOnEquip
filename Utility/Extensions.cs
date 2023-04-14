namespace BindOnEquip.Utility;

public static class ItemCategoriesExtensions
{
    public static bool HasFlagFast(this BindOnEquipPlugin.ItemCategories value, BindOnEquipPlugin.ItemCategories flag)
    {
        return (value & flag) != 0;
    }
}