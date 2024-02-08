using BepInEx.Configuration;
using ShipMaid.ObjectHelpers;
using Steamworks.Ugc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ShipMaid.Configuration
{
	public class ConfigSetupShotgunPositions
	{
		public ConfigEntry<string> Key;

		public string KeyDisplayName;

		public string pluginName;

		public string SettingDescription;

		public string SettingName;

		public string SettingValue;

		public ConfigSetupShotgunPositions()
		{
		}

		public static ConfigEntry<string> CreateKey(ConfigSetupShotgunPositions c)
		{
			return ShipMaid.instance.Config.Bind(c.pluginName, c.SettingName, c.SettingValue, c.SettingDescription);
		}

		public void AddOrUpdateObjectPositionSetting(GrabbableObjectPositionHelperShotgun shotgunPositionHelper)
		{
			List<GrabbableObjectPositionHelperShotgun> newSetting = new();
			string newSettingValue = string.Empty;
			var objList = GetObjectPositionList(this.Key.Value);
			// If this object already exists in list
			if (objList.Where(o => o.objName == shotgunPositionHelper.objName && shotgunPositionHelper.AmmoQuantity == o.AmmoQuantity).Any())
			{
				// Remove the old
				objList.Remove(objList.Where(o => o.objName == shotgunPositionHelper.objName && shotgunPositionHelper.AmmoQuantity == o.AmmoQuantity).FirstOrDefault());
				// Add the new
				objList.Add(shotgunPositionHelper);
			}
			// This means that there is the dummy setting in the list
			else if (objList.Where(o => o.objName == "name").Any())
			{
				// Remove the dummy
				objList.Remove(objList.Where(o => o.objName == "name").FirstOrDefault());
				// Add the new
				objList.Add(shotgunPositionHelper);
			}
			// This means that there are entries in the list and the new entry is not listed
			else
			{
				// Add the new
				objList.Add(shotgunPositionHelper);
			}
			for (int i = 0; i < objList.Count; i++)
			{
				GrabbableObjectPositionHelperShotgun obj = objList[i];
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

		public List<GrabbableObjectPositionHelperShotgun> GetObjectPositionList(string configSetting)
		{
			List<GrabbableObjectPositionHelperShotgun> resultPositions = new();

			Regex ItemMatches = new Regex(@"((?<name>[A-Za-z\d\(\)]+)[,](?<posx>[-]*[\d]+[.][\d]+)[,](?<posy>[-]*[\d]+[.][\d]+)[,](?<posz>[-]*[\d]+[.][\d]+)[,]*)[,](?<ammo>[\d])");
			var result = ItemMatches.Matches(configSetting);
			foreach (Match ItemMatch in result)
			{
				string name = ItemMatch.Groups["name"].Value;
				string S_x = ItemMatch.Groups["posx"].ToString();
				string S_y = ItemMatch.Groups["posy"].ToString();
				string S_z = ItemMatch.Groups["posz"].ToString();
				string ammo = ItemMatch.Groups["ammo"].ToString();
				ShipMaid.LogError($"Parsed config setting GrabbableObjectPositionHelperShotgun {name},{S_x},{S_y},{S_z},{ammo}");

				if (float.TryParse(S_x, out float x) && float.TryParse(S_y, out float y) && float.TryParse(S_z, out float z) && int.TryParse(ammo, out int ammoQuantity))
				{
					GrabbableObjectPositionHelperShotgun goph = new(name, x, y, z, ammoQuantity);
					resultPositions.Add(goph);
					ShipMaid.LogError($"Parsed config setting GrabbableObjectPositionHelperShotgun {goph.objName},{goph.PlacementPosition.x},{goph.PlacementPosition.y},{goph.PlacementPosition.z},{goph.AmmoQuantity}");
				}
			}

			return resultPositions;
		}

		public string MakeObjectSettingString(GrabbableObjectPositionHelperShotgun goph)
		{
			return goph.objName + "," + goph.PlacementPosition.x + "," + goph.PlacementPosition.y + "," + goph.PlacementPosition.z + "," + goph.AmmoQuantity;
		}
	}
}