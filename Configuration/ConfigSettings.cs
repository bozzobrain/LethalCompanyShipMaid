using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;

namespace ShipMaid.Configuration
{
	public static class ConfigSettings
	{
		public static ConfigSetupList ClosetLocationOverride = new ConfigSetupList()
		{
			pluginName = "ShipMaid",
			SettingName = "ClosetLocationOverride",
			SettingValue = "Whoopie,Key,Flashlight,StunGrenade",
			SettingDescription = "List of items separated by comma that will be automatically placed in the storage container on ship cleanup.",
		};

		public static Dictionary<string, ConfigEntryBase> currentConfigEntries = new Dictionary<string, ConfigEntryBase>();

		public static ConfigSetupString ItemGrouping = new ConfigSetupString()
		{
			pluginName = "ShipMaid",
			SettingName = "ItemGrouping",
			SettingValue = "Tight",
			SettingDescription = "[Loose] Spread items accross the ship from left to right -or- [Tight] Pack the items to the side of the ship with the suit rack.",
		};

		public static ConfigSetupGrabbableObjectPositions ItemPlacementOverrideLocation = new()
		{
			pluginName = "ShipMaid",
			SettingName = "ItemPlacementOverrideLocation",
			SettingValue = "name,0.00,0.00,0.00",
			SettingDescription = "Name and location of the item location (if UseItemTypePlacementOverrides is enabled)",
		};

		public static ConfigSetupVector3 ItemPlacementOverrideOffsetPosition = new()
		{
			pluginName = "ShipMaid",
			SettingName = "ItemPlacementOverrideOffsetPosition",
			SettingValue = "0.10,0.10,0.10",
			SettingDescription = "Vector3 variation of items within a stack of items (if UseOneHandedPlacementOverrides or UseTwoHandedPlacementOverrides is enabled)",
		};

		public static ConfigSetupFloat ItemPlacementOverrideOffsetRotation = new()
		{
			pluginName = "ShipMaid",
			SettingName = "ItemPlacementOverrideOffsetRotation",
			SettingValue = "10",
			SettingDescription = "Float rotation variation of items within a stack of items (if UseOneHandedPlacementOverrides or UseTwoHandedPlacementOverrides is enabled)",
		};

		public static ConfigSetupVector3 OneHandedItemPlacementOverrideLocation = new()
		{
			pluginName = "ShipMaid",
			SettingName = "OneHandedItemPlacementOverrideLocation",
			SettingValue = "0.00,0.00,0.00",
			SettingDescription = "Vector3 location of the One-handed item location (if UseOneHandedPlacementOverrides is enabled)",
		};

		public static ConfigSetupString OrganizationTechnique = new ConfigSetupString()
		{
			pluginName = "ShipMaid",
			SettingName = "OrganizationMethod",
			SettingValue = "Value",
			SettingDescription = "Choose organization method, spread items of a type by [Value] or [Stack] perfectly by item type.",
		};

		public static ConfigSetupString OrganizeShotgunByAmmo = new ConfigSetupString()
		{
			pluginName = "ShipMaid",
			SettingName = "OrganizeShotgunByAmmo",
			SettingValue = "Disabled",
			SettingDescription = "If [Enabled], setting position overrides on shotguns will permit them to be organized by how much ammo is loaded",
		};

		public static ConfigSetupShotgunPositions ShotgunPlacementOverrideLocation = new()
		{
			pluginName = "ShipMaid",
			SettingName = "ShotgunPlacementOverrideLocation",
			SettingValue = "name,0.00,0.00,0.00,0",
			SettingDescription = "Name,locationX,locationY,locationZ,shellCount of the shotgun (if OrganizeShotgunByAmmo is enabled)",
		};

		public static ConfigSetupList SortingLocationBlacklist = new ConfigSetupList()
		{
			pluginName = "ShipMaid",
			SettingName = "SortingDisabledList",
			SettingValue = "",
			SettingDescription = "List of items separated by comma that will be ignored on sorting.",
		};

		public static ConfigSetupString TwoHandedItemLocation = new ConfigSetupString()
		{
			pluginName = "ShipMaid",
			SettingName = "TwoHandedItemLocation",
			SettingValue = "Front",
			SettingDescription = "Choose location for two handed objects, [Front] of ship, or [Back] of ship. The opposite location will have the single handed items",
		};

		public static ConfigSetupVector3 TwoHandedItemPlacementOverrideLocation = new()
		{
			pluginName = "ShipMaid",
			SettingName = "TwoHandedItemPlacementOverrideLocation",
			SettingValue = "0.00,0.00,0.00",
			SettingDescription = "Vector3 location of the Two-handed item location (if UseTwoHandedPlacementOverrides is enabled)",
		};

		public static ConfigSetupString UseItemTypePlacementOverrides = new ConfigSetupString()
		{
			pluginName = "ShipMaid",
			SettingName = "UseItemTypePlacementOverrides",
			SettingValue = "Disabled",
			SettingDescription = "If [Enabled], pressing J (or what ever keybind from SetObjectTypePositionKey) will set an objects item type location for organization",
		};

		public static ConfigSetupString UseOneHandedPlacementOverrides = new ConfigSetupString()
		{
			pluginName = "ShipMaid",
			SettingName = "UseOneHandedPlacementOverrides",
			SettingValue = "Disabled",
			SettingDescription = "If [Enabled], pressing J (or what ever keybind from SetObjectTypePositionKey) with a one handed object will set all one handed objects location for organization",
		};

		public static ConfigSetupString UseOnlyTerminal = new ConfigSetupString()
		{
			pluginName = "ShipMaid",
			SettingName = "UseOnlyTerminal",
			SettingValue = "Disabled",
			SettingDescription = "If [Enabled], the keybinding will be disabled and only the terminal will be used for cleanup commands",
		};

		public static ConfigSetupString UseTwoHandedPlacementOverrides = new ConfigSetupString()
		{
			pluginName = "ShipMaid",
			SettingName = "UseTwoHandedPlacementOverrides",
			SettingValue = "Disabled",
			SettingDescription = "If [Enabled], pressing J (or what ever keybind from SetObjectTypePositionKey) with a two handed object will set all two handed objects location for organization",
		};

		public static void BindConfigSettings()
		{
			ShipMaid.Log("BindingConfigs");
			currentConfigEntries = UseOnlyTerminal.Bind(currentConfigEntries);
			currentConfigEntries = OrganizationTechnique.Bind(currentConfigEntries);
			currentConfigEntries = TwoHandedItemLocation.Bind(currentConfigEntries);
			currentConfigEntries = ItemGrouping.Bind(currentConfigEntries);
			currentConfigEntries = ClosetLocationOverride.Bind(currentConfigEntries);
			currentConfigEntries = SortingLocationBlacklist.Bind(currentConfigEntries);
			currentConfigEntries = UseItemTypePlacementOverrides.Bind(currentConfigEntries);
			currentConfigEntries = ItemPlacementOverrideLocation.Bind(currentConfigEntries);
			currentConfigEntries = UseOneHandedPlacementOverrides.Bind(currentConfigEntries);
			currentConfigEntries = OneHandedItemPlacementOverrideLocation.Bind(currentConfigEntries);
			currentConfigEntries = UseTwoHandedPlacementOverrides.Bind(currentConfigEntries);
			currentConfigEntries = TwoHandedItemPlacementOverrideLocation.Bind(currentConfigEntries);
			currentConfigEntries = ItemPlacementOverrideOffsetPosition.Bind(currentConfigEntries);
			currentConfigEntries = ItemPlacementOverrideOffsetRotation.Bind(currentConfigEntries);
			currentConfigEntries = OrganizeShotgunByAmmo.Bind(currentConfigEntries);
			currentConfigEntries = ShotgunPlacementOverrideLocation.Bind(currentConfigEntries);

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
				//ShipMaid.Log("Cleaning old config entries");
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