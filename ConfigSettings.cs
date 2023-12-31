using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ShipMaid
{
	public static class ConfigSettings
	{
		public static ConfigEntry<string> activateShipMaidKey;

		public static string activateShipMaidDisplayName;

		public static ConfigEntry<string> activateShipMaidClosetKey;

		public static string activateShipMaidClosetDisplayName;

		public static Dictionary<string, ConfigEntryBase> currentConfigEntries = new Dictionary<string, ConfigEntryBase>();

		public static void BindConfigSettings()
		{
			ShipMaid.Log("BindingConfigs");
			activateShipMaidKey = ((BaseUnityPlugin)ShipMaid.instance).Config.Bind<string>("ShipMaid", "CleanupShipKey", "<Keyboard>/m", "Activate ship maid ship keybind.");
			activateShipMaidClosetKey = ((BaseUnityPlugin)ShipMaid.instance).Config.Bind<string>("ShipMaid", "CleanupClosetKey", "<Keyboard>/n", "Activate ship maid closet keybind.");
			activateShipMaidDisplayName = GetDisplayName(activateShipMaidKey.Value);
			activateShipMaidClosetDisplayName = GetDisplayName(activateShipMaidClosetKey.Value);
			currentConfigEntries.Add(((ConfigEntryBase)activateShipMaidKey).Definition.Key, (ConfigEntryBase)(object)activateShipMaidKey);
			currentConfigEntries.Add(((ConfigEntryBase)activateShipMaidClosetKey).Definition.Key, (ConfigEntryBase)(object)activateShipMaidClosetKey);
			TryRemoveOldConfigSettings();
		}

		public static string GetDisplayName(string key)
		{
			key = key.Replace("<Keyboard>/", "");
			key = key.Replace("<Mouse>/", "");
			string text = key;
			text = text.Replace("leftAlt", "Alt");
			text = text.Replace("rightAlt", "Alt");
			text = text.Replace("leftCtrl", "Ctrl");
			text = text.Replace("rightCtrl", "Ctrl");
			text = text.Replace("leftShift", "Shift");
			text = text.Replace("rightShift", "Shift");
			text = text.Replace("leftButton", "LMB");
			text = text.Replace("rightButton", "RMB");
			return text.Replace("middleButton", "MMB");
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
				ConfigFile config = ((BaseUnityPlugin)ShipMaid.instance).Config;
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
