using BepInEx;
using HarmonyLib;
using ShipMaid.Configuration;
using ShipMaid.InputUtils;
using System.Reflection;

namespace ShipMaid
{
	[BepInPlugin(GUID, NAME, VERSION)]
	[BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
	internal class ShipMaid : BaseUnityPlugin
	{
		public static ShipMaid instance;
		public static ShipMaidFunctions smf = new();
		internal static InputUtilsKeybinds InputActionsInstance = new InputUtilsKeybinds();
		private const string GUID = "ShipMaid";
		private const string NAME = "ShipMaid";
		private const string VERSION = "4.0.8";

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