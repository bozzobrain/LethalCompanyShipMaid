using BepInEx.Configuration;
using System.Collections.Generic;

namespace ShipMaid.Configuration
{
	public class ConfigSetupString
	{
		public ConfigSetupString()
		{
		}

		public string pluginName;
		public string SettingName;
		public string SettingValue;
		public string SettingDescription;

		public Dictionary<string, ConfigEntryBase> Bind(Dictionary<string, ConfigEntryBase> toDictionary)
		{
			Key = CreateKey(this);
			toDictionary.Add(Key.Definition.Key, (ConfigEntryBase)(object)Key);
			return toDictionary;
		}

		public ConfigEntry<string> Key;
		public string KeyDisplayName;

		public static ConfigEntry<string> CreateKey(ConfigSetupString c)
		{
			return ShipMaid.instance.Config.Bind(c.pluginName, c.SettingName, c.SettingValue, c.SettingDescription);
		}
	}
}