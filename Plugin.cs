using BepInEx;
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
		private const string VERSION = "1.0.0";

		internal static ManualLogSource Log;
		private void Awake()
		{
			Log = Logger;
			// Plugin startup logic
			Log.LogInfo($"Plugin {GUID} is loaded!");

			Harmony harmony = new Harmony(GUID);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

		}
	}
}
