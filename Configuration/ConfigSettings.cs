using BepInEx.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
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

		public static ConfigSetupBool OrganizeShotgunByAmmo = new ConfigSetupBool()
		{
			pluginName = "ShipMaid",
			SettingName = "OrganizeShotgunByAmmo",
			SettingValue = false,
			SettingDescription = "If [true], setting position overrides on shotguns will permit them to be organized by how much ammo is loaded",
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

		public static ConfigSetupBool UseItemTypePlacementOverrides = new ConfigSetupBool()
		{
			pluginName = "ShipMaid",
			SettingName = "UseItemTypePlacementOverrides",
			SettingValue = false,
			SettingDescription = "If [true], pressing J (or what ever keybind from SetObjectTypePositionKey) will set an objects item type location for organization",
		};

		public static ConfigSetupBool UseOneHandedPlacementOverrides = new ConfigSetupBool()
		{
			pluginName = "ShipMaid",
			SettingName = "UseOneHandedPlacementOverrides",
			SettingValue = false,
			SettingDescription = "If [true], pressing J (or what ever keybind from SetObjectTypePositionKey) with a one handed object will set all one handed objects location for organization",
		};

		public static ConfigSetupBool UseOnlyTerminal = new ConfigSetupBool()
		{
			pluginName = "ShipMaid",
			SettingName = "UseOnlyTerminal",
			SettingValue = false,
			SettingDescription = "If [true], the keybinding will be disabled and only the terminal will be used for cleanup commands",
		};

		public static ConfigSetupBool UseTwoHandedPlacementOverrides = new ConfigSetupBool()
		{
			pluginName = "ShipMaid",
			SettingName = "UseTwoHandedPlacementOverrides",
			SettingValue = false,
			SettingDescription = "If [true], pressing J (or what ever keybind from SetObjectTypePositionKey) with a two handed object will set all two handed objects location for organization",
		};

		public static void BindConfigSettings()
		{
			ShipMaid.Log("BindingConfigs");
			MaybeMigrateConfigFile();
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

		public static void MaybeMigrateConfigFile()
		{
			try
			{
				// Check for file present
				//ShipMaid.Log("Cleaning old config entries");
				ConfigFile config = ShipMaid.instance.Config;
				string configFilePath = config.ConfigFilePath;
				if (!File.Exists(configFilePath))
				{
					return;
				}

				// Read file into string and array
				string configFileText = "";
				string[] configFileLines = File.ReadAllLines(configFilePath);
				int linesInFile = configFileLines.Length;
				for (int i = 0; i < configFileLines.Length; i++)
				{
					if (configFileLines[i].Contains(" Enabled") || configFileLines[i].Contains(" Disabled"))
					{
						ShipMaid.Log($"Found line to replace {configFileLines[i]}");
						configFileLines[i] = configFileLines[i].Replace("Enabled", "true");
						configFileLines[i] = configFileLines[i].Replace("Disabled", "false");
						if (i > 2 && configFileLines[i - 2].Contains("String"))
						{
							configFileLines[i - 2] = configFileLines[i - 2].Replace("String", "Boolean");
						}
						ShipMaid.Log($"Changed string to {configFileLines[i]}");
					}
				}
				for (int i = 0; i < configFileLines.Length; i++)
				{
					configFileText += configFileLines[i] + "\n";
				}

				File.WriteAllText("debug.txt", configFileText);
				File.WriteAllText(configFilePath, configFileText);
				config.Reload();
			}
			catch
			{
			}
		}

		public static void TryRemoveOldConfigSettings()
		{
			HashSet<string> sectionsHashSet = new HashSet<string>();
			HashSet<string> keysHashSet = new HashSet<string>();
			// Add all config settings to hash sets
			foreach (ConfigEntryBase value in currentConfigEntries.Values)
			{
				sectionsHashSet.Add(value.Definition.Section);
				keysHashSet.Add(value.Definition.Key);
			}

			// Try file modifications
			try
			{
				// Check for file present
				//ShipMaid.Log("Cleaning old config entries");
				ConfigFile config = ShipMaid.instance.Config;
				string configFilePath = config.ConfigFilePath;
				if (!File.Exists(configFilePath))
				{
					return;
				}

				// Read file into string and array
				string configFileText = File.ReadAllText(configFilePath);
				string[] configFileLines = File.ReadAllLines(configFilePath);
				string sectionTextFromFile = "";
				for (int i = 0; i < configFileLines.Length; i++)
				{
					configFileLines[i] = configFileLines[i].Replace("\n", "");
					if (configFileLines[i].Length <= 0)
					{
						continue;
					}
					// THIS IS A SECTION
					if (configFileLines[i].StartsWith("["))
					{
						// If the parsed section in the file is not in the code settings (remove it)
						if (sectionTextFromFile != "" && !sectionsHashSet.Contains(sectionTextFromFile))
						{
							sectionTextFromFile = "[" + sectionTextFromFile + "]";
							int num = configFileText.IndexOf(sectionTextFromFile);
							int num2 = configFileText.IndexOf(configFileLines[i]);
							configFileText = configFileText.Remove(num, num2 - num);
						}
						// Remove the section braces and store line
						sectionTextFromFile = configFileLines[i].Replace("[", "").Replace("]", "").Trim();
					}
					else
					{
						if (!(sectionTextFromFile != ""))
						{
							continue;
						}
						// This is a config setting description - Starts a new config entry
						if (i <= configFileLines.Length - 4 && configFileLines[i].StartsWith("##"))
						{
							int j;
							for (j = 1; i + j < configFileLines.Length && configFileLines[i + j].Length > 3; j++)
							{
							}
							if (sectionsHashSet.Contains(sectionTextFromFile))
							{
								int num3 = configFileLines[i + j - 1].IndexOf("=");
								string item = configFileLines[i + j - 1].Substring(0, num3 - 1);
								if (!keysHashSet.Contains(item))
								{
									int num4 = configFileText.IndexOf(configFileLines[i]);
									int num5 = configFileText.IndexOf(configFileLines[i + j - 1]) + configFileLines[i + j - 1].Length;
									configFileText = configFileText.Remove(num4, num5 - num4);
								}
							}
							i += j - 1;
						}
						else if (configFileLines[i].Length > 3)
						{
							configFileText = configFileText.Replace(configFileLines[i], "");
						}
					}
				}
				// Found a section in the config file that isnt in the config settings (remove it)
				if (!sectionsHashSet.Contains(sectionTextFromFile))
				{
					sectionTextFromFile = "[" + sectionTextFromFile + "]";
					int num6 = configFileText.IndexOf(sectionTextFromFile);
					configFileText = configFileText.Remove(num6, configFileText.Length - num6);
				}
				while (configFileText.Contains("\n\n\n"))
				{
					configFileText = configFileText.Replace("\n\n\n", "\n\n");
				}
				File.WriteAllText(configFilePath, configFileText);
				config.Reload();
			}
			catch
			{
			}
		}
	}
}