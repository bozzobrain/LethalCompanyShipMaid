using ShipMaid.Networking;
using HarmonyLib;

namespace ShipMaid.Patchers
{
	// Because of reasons I'm patching to the terminal object lol
	internal class TerminalPatcher
	{
		[HarmonyPatch(typeof(Terminal), "Start")]
		private static class Patch
		{
			[HarmonyPrefix]
			private static void AddToTerminalObject(Terminal __instance)
			{
				__instance.gameObject.AddComponent<NetworkFunctions.NetworkingObjectManager>();
			}
		}
	}
}