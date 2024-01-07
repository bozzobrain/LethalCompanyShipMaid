using BepInEx.Configuration;
using System.Collections.Generic;

namespace ShipMaid.Configuration
{
	public class ConfigSetupInputAction
	{
		public ConfigSetupInputAction()
		{
		}

		public string pluginName;
		public string ActionName;
		public string KeyboardMapping;
		public string ActionDescription;

		public void Bind()
		{
			Key = CreateKey(this);
			KeyDisplayName = GetDisplayName(Key.Value);
		}

		public Dictionary<string, ConfigEntryBase> Bind(Dictionary<string, ConfigEntryBase> toDictionary)
		{
			Key = CreateKey(this);
			toDictionary.Add(Key.Definition.Key, (ConfigEntryBase)(object)Key);
			return toDictionary;
		}

		public ConfigEntry<string> Key;
		public string KeyDisplayName;

		public static ConfigEntry<string> CreateKey(ConfigSetupInputAction c)
		{
			return ShipMaid.instance.Config.Bind(c.pluginName, c.ActionName, c.KeyboardMapping, c.ActionDescription);
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
	}
}