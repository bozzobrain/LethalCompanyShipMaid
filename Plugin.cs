﻿using BepInEx;
using BepInEx.Logging;
using Dissonance;
using HarmonyLib;
using System;
using System.Reflection;

namespace ShipMaid
{

	[BepInPlugin(GUID, NAME, VERSION)]
	internal class ShipMaid : BaseUnityPlugin
	{
		private const string GUID = "ShipMaid";
		private const string NAME = "ShipMaid";
		private const string VERSION = "1.1.0";

		public static ShipMaid instance;
		private void Awake()
		{
			instance = this;
			ConfigSettings.BindConfigSettings();

			// Plugin startup logic
			Logger.LogInfo($"Plugin {GUID} is loaded!");

			Harmony harmony = new Harmony(GUID);
			harmony.PatchAll(Assembly.GetExecutingAssembly());


		}
		public static void Log(string message)
		{
			instance.Logger.LogInfo((object)message);
		}
	}
}
