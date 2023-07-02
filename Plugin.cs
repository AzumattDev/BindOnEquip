using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BindOnEquip.Utility;
using HarmonyLib;
using JetBrains.Annotations;
using LocalizationManager;
using ServerSync;

namespace BindOnEquip
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class BindOnEquipPlugin : BaseUnityPlugin
    {
        internal const string ModName = "BindOnEquip";
        internal const string ModVersion = "1.2.0";
        internal const string Author = "Azumatt";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource BindOnEquipLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        public static class ItemDataKeys
        {
            public const string BindOnEquip = "BindOnEquip";
            public const string SteamID = "SteamID";
            public const string PlayerName = "PlayerName";
            public const string BindTime = "BindTime";
            public const string IsBound = "IsBound";
        }

        [Flags]
        public enum ItemCategories
        {
            None = 0,
            Tool = 1 << 0,
            OneHandedWeapon = 1 << 1,
            TwoHandedWeapon = 1 << 2,
            TwoHandedWeaponLeft = 1 << 3,
            Bow = 1 << 4,
            Shield = 1 << 5,
            Helmet = 1 << 6,
            Chest = 1 << 7,
            Legs = 1 << 8,
            Shoulder = 1 << 9,
            Ammo = 1 << 10,
            Torch = 1 << 11,
            Utility = 1 << 12
        }


        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public void Awake()
        {
            Localizer.Load();

            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On,
                "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            IncludedCategories = ItemCatConfig("1 - General", "Included Categories",
                (ItemCategories)Enum.GetValues(typeof(ItemCategories)).Cast<int>().Sum() & ~(ItemCategories.Ammo | ItemCategories.Torch | ItemCategories.Tool),
                "List of item categories that are affected by the bind on equip. What this means is items with these categories will use the bind on equip system. This is useful for items that are meant to be bound to a player, such as armor or weapons.");

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void Start()
        {
            AutoDoc();
        }

        private void AutoDoc()
        {
#if DEBUG
            // Store Regex to get all characters after a [
            Regex regex = new(@"\[(.*?)\]");

            // Strip using the regex above from Config[x].Description.Description
            string Strip(string x) => regex.Match(x).Groups[1].Value;
            StringBuilder sb = new();
            string lastSection = "";
            foreach (ConfigDefinition x in Config.Keys)
            {
                // skip first line
                if (x.Section != lastSection)
                {
                    lastSection = x.Section;
                    sb.Append($"{Environment.NewLine}`{x.Section}`{Environment.NewLine}");
                }

                sb.Append($"\n{x.Key} [{Strip(Config[x].Description.Description)}]" +
                          $"{Environment.NewLine}   * {Config[x].Description.Description.Replace("[Synced with Server]", "").Replace("[Not Synced with Server]", "")}" +
                          $"{Environment.NewLine}     * Default Value: {Config[x].GetSerializedValue()}{Environment.NewLine}");
            }

            File.WriteAllText(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, $"{ModName}_AutoDoc.md"),
                sb.ToString());
#endif
        }

        private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                BindOnEquipLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                BindOnEquipLogger.LogError($"There was an issue loading your {ConfigFileName}");
                BindOnEquipLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;
        internal static ConfigEntry<ItemCategories> IncludedCategories = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        ConfigEntry<T> ItemCatConfig<T>(string group, string name, T value, string desc,
            bool synchronizedSetting = true)
        {
            ConfigurationManagerAttributes attributes = new()
            {
                CustomDrawer = Functions.DrawCategoriesTable
            };
            return config(group, name, value, new ConfigDescription(desc, null, attributes), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public int? Order;
            [UsedImplicitly] public bool? Browsable;
            [UsedImplicitly] public string? Category;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer;
        }

        #endregion
    }
}