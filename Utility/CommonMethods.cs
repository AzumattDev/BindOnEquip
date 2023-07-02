using System;
using ItemDataManager;

namespace BindOnEquip.Utility;

static class CommonMethods
{
    public static bool CheckItemData(ItemDrop.ItemData item, bool showdenymessage = true, bool isEquipAction = true)
    {
        if (Player.m_localPlayer == null || Player.m_localPlayer.m_isLoading) return true;

        if (item != null && item.m_shared.IsIncludedItemType())
        {
            if (item.IsCustomDataNull())
            {
#if DEBUG
                BindOnEquipPlugin.BindOnEquipLogger.LogDebug("Custom data is null, setting default values");
#endif
                if (isEquipAction)
                    BindToCurrentPlayer(item);
                else
                    item.DefaultSetAllItemData();
            }
            else if (item.Data()[BindOnEquipPlugin.ItemDataKeys.IsBound] == "true")
            {
#if DEBUG
                BindOnEquipPlugin.BindOnEquipLogger.LogDebug($"Item {item.m_shared.m_name} is bound, comparing data");
#endif
                string id = GetPlatformUserId();

                if (!item.CompareItemData(id, Game.instance.GetPlayerProfile().m_playerName))
                {
                    if (showdenymessage)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center,
                            Localization.instance.Localize("$item_binds_on_equip_denymessage"));
                    }

                    return false;
                }
            }
        }

        return true;
    }

    public static void BindToCurrentPlayer(ItemDrop.ItemData item)
    {
        string id = GetPlatformUserId();
        item.DefaultSetAllItemData("true", id, Game.instance.GetPlayerProfile().m_playerName, DateTime.Now.ToString(), "true");
    }

    public static string GetPlatformUserId()
    {
        return PrivilegeManager.GetCurrentPlatform() == PrivilegeManager.Platform.Steam
            ? Steamworks.SteamUser.GetSteamID().ToString()
            : PrivilegeManager.GetNetworkUserId();
    }
}