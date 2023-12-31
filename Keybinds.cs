using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using static UnityEngine.InputSystem.InputAction;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

namespace ShipMaid
{
    [HarmonyPatch]
    internal static class Keybinds
    {
        public static PlayerControllerB localPlayerController;

        private static InputAction shipMaidCleanupShip;
        private static InputAction shipMaidCleanupCloset;

		[HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        public static void OnLocalPlayerConnect(PlayerControllerB __instance)
        {
            localPlayerController = __instance;
            shipMaidCleanupShip = new InputAction(null, 0, ConfigSettings.activateShipMaidKey.Value, "Press", null, null);
            shipMaidCleanupCloset = new InputAction(null, 0, ConfigSettings.activateShipMaidClosetKey.Value, "Press", null, null);
			if (localPlayerController.gameObject.activeSelf)
            {
                SubscribeToEvents();
            }
        }

        private static void SubscribeToEvents()
        {
            if (shipMaidCleanupShip != null)
            {
                shipMaidCleanupShip.Enable();
                shipMaidCleanupCloset.Enable();

				shipMaidCleanupShip.performed += OnShipMaidShipCleanupCalled;
                shipMaidCleanupCloset.performed += OnShipMaidClosetCleanupCalled;

			}
        }

        [HarmonyPatch(typeof(PlayerControllerB), "OnEnable")]
        [HarmonyPostfix]
        public static void OnEnable(PlayerControllerB __instance)
        {
            if ((Object)(object)__instance == (Object)(object)localPlayerController)
            {
                SubscribeToEvents();
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "OnDisable")]
        [HarmonyPostfix]
        public static void OnDisable(PlayerControllerB __instance)
		{
			if (shipMaidCleanupShip != null && !((Object)(object)__instance != (Object)(object)localPlayerController))
			{
				shipMaidCleanupShip.performed -= OnShipMaidShipCleanupCalled;
				shipMaidCleanupShip.Disable();
			}
			if (shipMaidCleanupCloset != null && !((Object)(object)__instance != (Object)(object)localPlayerController))
			{
				shipMaidCleanupCloset.performed -= OnShipMaidClosetCleanupCalled;
				shipMaidCleanupCloset.Disable();
			}
		}

		private static void OnShipMaidShipCleanupCalled(CallbackContext context)
		{
			if ((Object)(object)localPlayerController == null || !localPlayerController.isPlayerControlled || localPlayerController.inTerminalMenu || localPlayerController.IsServer && !localPlayerController.isHostPlayerObject)
			{
				return;
			}
			ShipMaid.Log("Cleanup Ship");

			ShipMaidFunctions.OrganizeShipLoot();
		}
		private static void OnShipMaidClosetCleanupCalled(CallbackContext context)
		{
			if ((Object)(object)localPlayerController == null || !localPlayerController.isPlayerControlled || localPlayerController.inTerminalMenu || localPlayerController.IsServer && !localPlayerController.isHostPlayerObject)
			{
				return;
			}
			ShipMaid.Log("Cleanup Closet");

			ShipMaidFunctions.OrganizeStorageCloset();
		}
	}

}
