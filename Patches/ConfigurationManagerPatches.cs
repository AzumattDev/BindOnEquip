using BindOnEquip.Utility;
using HarmonyLib;
using UnityEngine;

namespace BindOnEquip.Patches;

[HarmonyPatch(typeof(FejdStartup),nameof(FejdStartup.Awake))]
static class ConfigurationManagerPatch
{
    internal static object? _configManager;
    internal static GUIStyle _enabledToggleStyle = null!;
    internal static GUIStyle _disabledToggleStyle = null!;
    internal static GUIStyle _disabledToggleStyle2 = null!;
    
    static void Prefix(FejdStartup __instance)
    {
        Functions.PatchConfigManager(); // Patch the config manager late to load any fonts or anything we should need later.
    }
}