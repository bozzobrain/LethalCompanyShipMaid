using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ShipMaid.Configuration
{
    public static class ConfigSettings
    {



        public static Dictionary<string, ConfigEntryBase> currentConfigEntries = new Dictionary<string, ConfigEntryBase>();

        public static ConfigSetupInputAction ShipMaidShipCleanupInputAction = new ConfigSetupInputAction()
        {
            pluginName = "ShipMaid",
            ActionName = "CleanupShipKey",
            KeyboardMapping = "<Keyboard>/m",
            ActionDescription = "Activate ship maid ship keybind.",

        };

        public static ConfigSetupInputAction ShipMaidClosetCleanupInputAction = new ConfigSetupInputAction()
        {
            pluginName = "ShipMaid",
            ActionName = "CleanupClosetKey",
            KeyboardMapping = "<Keyboard>/n",
            ActionDescription = "Activate ship maid closet keybind.",
        };
        public static ConfigSetupString OrganizationTechnique = new ConfigSetupString()
        {
            pluginName = "ShipMaid",
            SettingName = "OrganizationMethod",
            SettingValue = "Value",
            SettingDescription = "Choose organization method, spread by [Value] or [Stack] perfectly.",
        };
        public static ConfigSetupString TwoHandedItemLocation = new ConfigSetupString()
        {
            pluginName = "ShipMaid",
            SettingName = "TwoHandedItemLocation",
            SettingValue = "Front",
            SettingDescription = "Choose location for two handed objects, [Front] of ship, or [Back] of ship. The opposite location will have the single handed items",
        };
        public static ConfigSetupString ItemGrouping = new ConfigSetupString()
        {
            pluginName = "ShipMaid",
            SettingName = "ItemGrouping",
            SettingValue = "Tight",
            SettingDescription = "[Loose] or [Tight]",
        };



		public static ConfigSetupList ClosetLocationOverride = new ConfigSetupList()
		{
			pluginName = "ShipMaid",
			SettingName = "ClosetLocationOverride",
			SettingValue = "Whoopie,Key,Flashlight,StunGrenade",
			SettingDescription = "List of items separated by comma that will be automatically placed in the storage container on ship cleanup.",
		};

		public static void BindConfigSettings()
        {
            ShipMaid.Log("BindingConfigs");
			currentConfigEntries = ShipMaidShipCleanupInputAction.Bind(currentConfigEntries);
			currentConfigEntries = ShipMaidClosetCleanupInputAction.Bind(currentConfigEntries);
			currentConfigEntries = OrganizationTechnique.Bind(currentConfigEntries);
			currentConfigEntries = TwoHandedItemLocation.Bind(currentConfigEntries);
			currentConfigEntries = ItemGrouping.Bind(currentConfigEntries);
			currentConfigEntries = ClosetLocationOverride.Bind(currentConfigEntries);

			//currentConfigEntries.Add(ShipMaidShipCleanupInputAction.Key.Definition.Key, (ConfigEntryBase)(object)ShipMaidShipCleanupInputAction.Key);
			//         currentConfigEntries.Add(ShipMaidClosetCleanupInputAction.Key.Definition.Key, (ConfigEntryBase)(object)ShipMaidClosetCleanupInputAction.Key);
			TryRemoveOldConfigSettings();
        }

       

        public static void TryRemoveOldConfigSettings()
        {
            HashSet<string> hashSet = new HashSet<string>();
            HashSet<string> hashSet2 = new HashSet<string>();
            foreach (ConfigEntryBase value in currentConfigEntries.Values)
            {
                hashSet.Add(value.Definition.Section);
                hashSet2.Add(value.Definition.Key);
            }
            try
            {
                ShipMaid.Log("Cleaning old config entries");
                ConfigFile config = ShipMaid.instance.Config;
                string configFilePath = config.ConfigFilePath;
                if (!File.Exists(configFilePath))
                {
                    return;
                }
                string text = File.ReadAllText(configFilePath);
                string[] array = File.ReadAllLines(configFilePath);
                string text2 = "";
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = array[i].Replace("\n", "");
                    if (array[i].Length <= 0)
                    {
                        continue;
                    }
                    if (array[i].StartsWith("["))
                    {
                        if (text2 != "" && !hashSet.Contains(text2))
                        {
                            text2 = "[" + text2 + "]";
                            int num = text.IndexOf(text2);
                            int num2 = text.IndexOf(array[i]);
                            text = text.Remove(num, num2 - num);
                        }
                        text2 = array[i].Replace("[", "").Replace("]", "").Trim();
                    }
                    else
                    {
                        if (!(text2 != ""))
                        {
                            continue;
                        }
                        if (i <= array.Length - 4 && array[i].StartsWith("##"))
                        {
                            int j;
                            for (j = 1; i + j < array.Length && array[i + j].Length > 3; j++)
                            {
                            }
                            if (hashSet.Contains(text2))
                            {
                                int num3 = array[i + j - 1].IndexOf("=");
                                string item = array[i + j - 1].Substring(0, num3 - 1);
                                if (!hashSet2.Contains(item))
                                {
                                    int num4 = text.IndexOf(array[i]);
                                    int num5 = text.IndexOf(array[i + j - 1]) + array[i + j - 1].Length;
                                    text = text.Remove(num4, num5 - num4);
                                }
                            }
                            i += j - 1;
                        }
                        else if (array[i].Length > 3)
                        {
                            text = text.Replace(array[i], "");
                        }
                    }
                }
                if (!hashSet.Contains(text2))
                {
                    text2 = "[" + text2 + "]";
                    int num6 = text.IndexOf(text2);
                    text = text.Remove(num6, text.Length - num6);
                }
                while (text.Contains("\n\n\n"))
                {
                    text = text.Replace("\n\n\n", "\n\n");
                }
                File.WriteAllText(configFilePath, text);
                config.Reload();
            }
            catch
            {
            }
        }
    }
}
