using BepInEx.Configuration;
using ShipMaid.ObjectHelpers;
using Steamworks.Ugc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ShipMaid.Configuration
{
	public class ConfigSetupGrabbableObjectPositions
	{
		public ConfigEntry<string> Key;

		public string KeyDisplayName;

		public string pluginName;

		public string SettingDescription;

		public string SettingName;

		public string SettingValue;

		public ConfigSetupGrabbableObjectPositions()
		{
		}

		public static ConfigEntry<string> CreateKey(ConfigSetupGrabbableObjectPositions c)
		{
			return ShipMaid.instance.Config.Bind(c.pluginName, c.SettingName, c.SettingValue, c.SettingDescription);
		}

		public void AddOrUpdateObjectPositionSetting(GrabbableObjectPositionHelper objectPositionHelper)
		{
			List<GrabbableObjectPositionHelper> newSetting = new();
			string newSettingValue = string.Empty;
			var objList = GetObjectPositionList(this.Key.Value);
			// If this object already exists in list
			if (objList.Where(o => o.objName == objectPositionHelper.objName).Any())
			{
				// Remove the old
				objList.Remove(objList.Where(o => o.objName == objectPositionHelper.objName).FirstOrDefault());
				// Add the new
				objList.Add(objectPositionHelper);
			}
			// This means that there is the dummy setting in the list
			else if (objList.Where(o => o.objName == "name").Any())
			{
				// Remove the dummy
				objList.Remove(objList.Where(o => o.objName == "name").FirstOrDefault());
				// Add the new
				objList.Add(objectPositionHelper);
			}
			// This means that there are entries in the list and the new entry is not listed
			else
			{
				// Add the new
				objList.Add(objectPositionHelper);
			}
			for (int i = 0; i < objList.Count; i++)
			{
				GrabbableObjectPositionHelper obj = objList[i];
				newSettingValue += MakeObjectSettingString(obj);
				if (i < objList.Count - 1)
				{
					newSettingValue += ",";
				}
			}
			if (newSettingValue.Length > 0)
			{
				Key.Value = newSettingValue;
			}
		}

		public Dictionary<string, ConfigEntryBase> Bind(Dictionary<string, ConfigEntryBase> toDictionary)
		{
			Key = CreateKey(this);
			toDictionary.Add(Key.Definition.Key, (ConfigEntryBase)(object)Key);
			return toDictionary;
		}

		public List<GrabbableObjectPositionHelper> GetObjectPositionList(string configSetting)
		{
			List<GrabbableObjectPositionHelper> resultPositions = new();

			Regex ItemMatches = new Regex(@"((?<name>[A-Za-z\d\(\)]+)[,](?<posx>[-]*[\d]+[.][\d]+)[,](?<posy>[-]*[\d]+[.][\d]+)[,](?<posz>[-]*[\d]+[.][\d]+)[,]*)");
			var result = ItemMatches.Matches(configSetting);
			foreach (Match ItemMatch in result)
			{
				string name = ItemMatch.Groups["name"].Value;
				string S_x = ItemMatch.Groups["posx"].ToString();
				string S_y = ItemMatch.Groups["posy"].ToString();
				string S_z = ItemMatch.Groups["posz"].ToString();
				if (float.TryParse(S_x, out float x) && float.TryParse(S_y, out float y) && float.TryParse(S_z, out float z))
				{
					GrabbableObjectPositionHelper goph = new(name, x, y, z);
					resultPositions.Add(goph);
					//ShipMaid.Log($"Parsed config setting GrabbleObjectPositionHelper {goph.objName},{goph.PlacementPosition.x},{goph.PlacementPosition.y},{goph.PlacementPosition.z}");
				}
			}

			return resultPositions;
		}

		public string MakeObjectSettingString(GrabbableObjectPositionHelper goph)
		{
			return goph.objName + "," + goph.PlacementPosition.x + "," + goph.PlacementPosition.y + "," + goph.PlacementPosition.z;
		}
	}
}