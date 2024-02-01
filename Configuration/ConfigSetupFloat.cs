using BepInEx.Configuration;
using Steamworks.Ugc;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ShipMaid.Configuration
{
	public class ConfigSetupFloat
	{
		public ConfigEntry<string> Key;

		public string KeyDisplayName;

		public string pluginName;

		public string SettingDescription;

		public string SettingName;

		public string SettingValue;

		public ConfigSetupFloat()
		{
		}

		public static ConfigEntry<string> CreateKey(ConfigSetupFloat c)
		{
			return ShipMaid.instance.Config.Bind(c.pluginName, c.SettingName, c.SettingValue, c.SettingDescription);
		}

		public Dictionary<string, ConfigEntryBase> Bind(Dictionary<string, ConfigEntryBase> toDictionary)
		{
			Key = CreateKey(this);
			toDictionary.Add(Key.Definition.Key, (ConfigEntryBase)(object)Key);
			return toDictionary;
		}

		public bool GetFloat(string configSetting, out float resultFloat)
		{
			resultFloat = new();

			Regex ItemMatches = new Regex(@"((?<float>[-]*[\d]+[.][\d]+))");
			var result = ItemMatches.Matches(configSetting);
			foreach (Match ItemMatch in result)
			{
				string s_float = ItemMatch.Groups["float"].ToString();
				if (float.TryParse(s_float, out float x) )
				{
					resultFloat = x;
					//ShipMaid.Log($"Parsed config setting Vector3 {resultVector.x},{resultVector.y},{resultVector.z}");
					return true;
				}
			}

			return false;
		}

		public void SetFloat(float settingFloat)
		{
			Key.Value = settingFloat.ToString() ;
		}
	}
}