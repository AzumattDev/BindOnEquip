# BindOnEquip (Character Specific)

A mod for Valheim that allows players to bind items to their account on a character-specific basis, making the items
unusable by others.

## Description

The BindOnEquip mod adds a feature to bind items to a specific player's account and character upon equipping, making the
items unusable by other players while maintaining character-specific progression. Items can still be dropped, but they
can only be equipped by the intended character on the bound account.

The item binding checks the SteamID of the account and the player name. If either of them do not match, the item cannot
be equipped. This ensures that items are only usable by the intended character on the bound account.

`Version checks with itself. If installed on the server, it will kick clients who do not have it installed.`

`This mod uses ServerSync, if installed on the server and all clients, it will sync all configs to client`

`This mod uses a file watcher. If the configuration file is not changed with BepInEx Configuration Manager, but changed in the file directly on the server, upon file save, it will sync the changes to all clients.`


## Installation Instructions


### Manual Installation

`Note: (Manual installation is likely how you have to do this on a server, make sure BepInEx is installed on the server correctly)`

1. **Download the latest release of BepInEx.**
2. **Extract the contents of the zip file to your game's root folder.**
3. **Download the latest release of BindOnEquip from Thunderstore.io.**
4. **Extract the contents of the zip file to the `BepInEx/plugins` folder.**
5. **Launch the game.**

### Installation through r2modman or Thunderstore Mod Manager

1. **Install [r2modman](https://valheim.thunderstore.io/package/ebkr/r2modman/) or [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager).**

   > For r2modman, you can also install it through the Thunderstore site.
   ![](https://i.imgur.com/s4X4rEs.png "r2modman Download")

   > For Thunderstore Mod Manager, you can also install it through the Overwolf app store
   ![](https://i.imgur.com/HQLZFp4.png "Thunderstore Mod Manager Download")
2. **Open the Mod Manager and search for "BindOnEquip" under the Online tab. `Note: You can also search for "Azumatt" to find all my mods.`**
   `The image below shows VikingShip as an example, but it was easier to reuse the image. Type BindOnEquip.`

![](https://i.imgur.com/5CR5XKu.png)

3. **Click the Download button to install the mod.**
4. **Launch the game.**


`Feel free to reach out to me on Discord if you need manual download assistance.`

## Configuration

#### 1 - General

**Lock Configuration [Synced with Server]**

* If on, the configuration is locked and can be changed by server admins only.
    * Default Value: On

**Ignored Categories [Synced with Server]**

* List of item categories that are not affected by the bind on equip. What this means is items with these categories
  will be ignored by the bind on equip system. This is useful for items that are not meant to be bound to a player, such
  as arrows or food.
    * Default Value: Material, Consumable, Ammo, Customization, Trophie, Torch, Misc, Tool, Fish, AmmoNonEquipable

## Author Information

### Azumatt

`DISCORD:` Azumatt#2625

`STEAM:` https://steamcommunity.com/id/azumatt/

For Questions or Comments, find me in the Odin Plus Team Discord or in mine:

[![https://i.imgur.com/XXP6HCU.png](https://i.imgur.com/XXP6HCU.png)](https://discord.gg/Pb6bVMnFb2)
<a href="https://discord.gg/pdHgy6Bsng"><img src="https://i.imgur.com/Xlcbmm9.png" href="https://discord.gg/pdHgy6Bsng" width="175" height="175"></a>
