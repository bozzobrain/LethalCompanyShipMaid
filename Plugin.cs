﻿using BepInEx;
using HarmonyLib;
using ShipMaid.Configuration;
using System.Reflection;

namespace ShipMaid
{
	[BepInPlugin(GUID, NAME, VERSION)]
	internal class ShipMaid : BaseUnityPlugin
	{
		public static ShipMaid instance;
		private const string GUID = "ShipMaid";
		private const string NAME = "ShipMaid";
		private const string VERSION = "3.1.1";

		public static void Log(string message)
		{
			instance.Logger.LogInfo((object)message);
		}

		public static void LogError(string message)
		{
			instance.Logger.LogError((object)message);
		}

		private void Awake()
		{
			instance = this;
			ConfigSettings.BindConfigSettings();

			// Plugin startup logic
			Logger.LogInfo($"Plugin {GUID} is loaded!");

			Harmony harmony = new Harmony(GUID);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}