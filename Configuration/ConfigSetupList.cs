using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShipMaid.Configuration
{
	public class ConfigSetupList
	{
		public ConfigSetupList()
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

		public static ConfigEntry<string> CreateKey(ConfigSetupList c)
		{
			return ShipMaid.instance.Config.Bind(c.pluginName, c.SettingName, c.SettingValue, c.SettingDescription);
		}
		public List<string> GetStrings(string configSetting)
		{
			List<string> results = new List<string>();
			Regex ItemMatches = new Regex("((?<item>[A-Za-z]+)[,]*)");
			var result = ItemMatches.Matches(configSetting);
			foreach (Match ItemMatch in result)
			{
				string item = ItemMatch.Groups["item"].ToString();
				results.Add(ItemMatch.Groups["item"].ToString());
				ShipMaid.Log($"Got configuration item {item}");
			}

			return results;
		}
	}
}
