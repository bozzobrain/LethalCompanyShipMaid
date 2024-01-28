using BepInEx.Configuration;
using Steamworks.Ugc;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ShipMaid.Configuration
{
	public class ConfigSetupVector3
	{
		public ConfigEntry<string> Key;

		public string KeyDisplayName;

		public string pluginName;

		public string SettingDescription;

		public string SettingName;

		public string SettingValue;

		public ConfigSetupVector3()
		{
		}

		public static ConfigEntry<string> CreateKey(ConfigSetupVector3 c)
		{
			return ShipMaid.instance.Config.Bind(c.pluginName, c.SettingName, c.SettingValue, c.SettingDescription);
		}

		public Dictionary<string, ConfigEntryBase> Bind(Dictionary<string, ConfigEntryBase> toDictionary)
		{
			Key = CreateKey(this);
			toDictionary.Add(Key.Definition.Key, (ConfigEntryBase)(object)Key);
			return toDictionary;
		}

		public bool GetVector3(string configSetting, out Vector3 resultVector)
		{
			resultVector = new();

			Regex ItemMatches = new Regex(@"((?<posx>[-]*[\d]+[.][\d]+))[,](?<posy>[-]*[\d]+[.][\d]+)[,](?<posz>[-]*[\d]+[.][\d]+)");
			var result = ItemMatches.Matches(configSetting);
			foreach (Match ItemMatch in result)
			{
				string S_x = ItemMatch.Groups["posx"].ToString();
				string S_y = ItemMatch.Groups["posy"].ToString();
				string S_z = ItemMatch.Groups["posz"].ToString();
				if (float.TryParse(S_x, out float x) && float.TryParse(S_y, out float y) && float.TryParse(S_z, out float z))
				{
					resultVector = new(x, y, z);
					//ShipMaid.Log($"Parsed config setting Vector3 {resultVector.x},{resultVector.y},{resultVector.z}");
					return true;
				}
			}

			return false;
		}

		public void SetVector3(Vector3 settingVector)
		{
			Key.Value = settingVector.x.ToString() + "," + settingVector.y.ToString() + "," + settingVector.z.ToString();
		}
	}
}