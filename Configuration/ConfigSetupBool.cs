using BepInEx.Configuration;
using System.Collections.Generic;

namespace ShipMaid.Configuration
{
	public class ConfigSetupBool
	{
		public ConfigEntry<bool> Key;

		public string KeyDisplayName;

		public string pluginName;

		public string SettingDescription;

		public string SettingName;

		public bool SettingValue;

		public ConfigSetupBool()
		{
		}

		public static ConfigEntry<bool> CreateKey(ConfigSetupBool c)
		{
			return ShipMaid.instance.Config.Bind(c.pluginName, c.SettingName, c.SettingValue, c.SettingDescription);
		}

		public Dictionary<string, ConfigEntryBase> Bind(Dictionary<string, ConfigEntryBase> toDictionary)
		{
			Key = CreateKey(this);
			toDictionary.Add(Key.Definition.Key, (ConfigEntryBase)(object)Key);
			return toDictionary;
		}
	}
}