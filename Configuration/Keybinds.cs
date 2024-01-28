using GameNetcodeStuff;
using HarmonyLib;
using ShipMaid.Configuration;
using ShipMaid.HelperFunctions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static ShipMaid.Networking.NetworkFunctions;
using static UnityEngine.InputSystem.InputAction;

namespace ShipMaid
{
	[HarmonyPatch]
	internal static class Keybinds
	{
		public static PlayerControllerB localPlayerController;

		private static InputAction shipMaidCleanupCloset;
		private static InputAction shipMaidCleanupShip;
		private static InputAction shipMaidDropAndSetObjectTypePosition;

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
			if (shipMaidDropAndSetObjectTypePosition != null && !((Object)(object)__instance != (Object)(object)localPlayerController))
			{
				shipMaidDropAndSetObjectTypePosition.performed -= OnShipMaidDropAndSetObjectTypePositionCalled;
				shipMaidDropAndSetObjectTypePosition.Disable();
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

		[HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
		[HarmonyPostfix]
		public static void OnLocalPlayerConnect(PlayerControllerB __instance)
		{
			localPlayerController = __instance;
			shipMaidCleanupShip = new InputAction(null, 0, ConfigSettings.ShipMaidShipCleanupInputAction.Key.Value, "Press", null, null);
			shipMaidCleanupCloset = new InputAction(null, 0, ConfigSettings.ShipMaidClosetCleanupInputAction.Key.Value, "Press", null, null);
			shipMaidDropAndSetObjectTypePosition = new InputAction(null, 0, ConfigSettings.ShipMaidSetObjectTypePositionInputAction.Key.Value, "Press", null, null);
			ShipMaidFunctions.initItemLocations();

			if (localPlayerController.gameObject.activeSelf)
			{
				SubscribeToEvents();
			}
			ShipMaid.Log("Local Player Connect");

			NetworkingObjectManager.NetworkManagerInit();

			ShipMaid.Log("Registering Manager");
		}

		private static void OnShipMaidClosetCleanupCalled(CallbackContext context)
		{
			if ((Object)(object)localPlayerController == null || !localPlayerController.isPlayerControlled || localPlayerController.inTerminalMenu || localPlayerController.IsServer && !localPlayerController.isHostPlayerObject)
			{
				return;
			}
			ShipMaid.Log("Cleanup Closet");

			LootOrganizingFunctions.OrganizeStorageCloset();
		}

		private static void OnShipMaidDropAndSetObjectTypePositionCalled(CallbackContext context)
		{
			if ((Object)(object)localPlayerController == null || !localPlayerController.isPlayerControlled || localPlayerController.inTerminalMenu || localPlayerController.IsServer && !localPlayerController.isHostPlayerObject)
			{
				return;
			}
			ShipMaid.Log("Set object cleanup position");

			ShipMaidFunctions.DropAndSetObjectTypePosition();
		}

		private static void OnShipMaidShipCleanupCalled(CallbackContext context)
		{
			if ((Object)(object)localPlayerController == null || !localPlayerController.isPlayerControlled || localPlayerController.inTerminalMenu || localPlayerController.IsServer && !localPlayerController.isHostPlayerObject)
			{
				return;
			}
			ShipMaid.Log("Cleanup Ship");

			LootOrganizingFunctions.OrganizeShipLoot();
		}

		private static void SubscribeToEvents()
		{
			if (shipMaidCleanupShip != null)
			{
				shipMaidCleanupShip.Enable();
				shipMaidCleanupShip.performed += OnShipMaidShipCleanupCalled;
			}
			if (shipMaidCleanupCloset != null)
			{
				shipMaidCleanupCloset.Enable();
				shipMaidCleanupCloset.performed += OnShipMaidClosetCleanupCalled;
			}
			if (shipMaidDropAndSetObjectTypePosition != null)
			{
				shipMaidDropAndSetObjectTypePosition.Enable();
				shipMaidDropAndSetObjectTypePosition.performed += OnShipMaidDropAndSetObjectTypePositionCalled;
			}
		}
	}
}