using GameNetcodeStuff;
using HarmonyLib;
using ShipMaid.Configuration;
using ShipMaid.HelperFunctions;
using ShipMaid.InputUtils;
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

		[HarmonyPatch(typeof(PlayerControllerB), "OnDisable")]
		[HarmonyPostfix]
		public static void OnDisable(PlayerControllerB __instance)
		{
			if (ShipMaid.InputActionsInstance.ShipCleanupKey != null && !((Object)(object)__instance != (Object)(object)localPlayerController))
			{
				ShipMaid.InputActionsInstance.ShipCleanupKey.performed -= OnShipMaidShipCleanupCalled;
				ShipMaid.InputActionsInstance.ShipCleanupKey.Disable();
			}
			if (ShipMaid.InputActionsInstance.StorageCleanupKey != null && !((Object)(object)__instance != (Object)(object)localPlayerController))
			{
				ShipMaid.InputActionsInstance.StorageCleanupKey.performed -= OnShipMaidClosetCleanupCalled;
				ShipMaid.InputActionsInstance.StorageCleanupKey.Disable();
			}
			if (ShipMaid.InputActionsInstance.LocationOverrideKey != null && !((Object)(object)__instance != (Object)(object)localPlayerController))
			{
				ShipMaid.InputActionsInstance.LocationOverrideKey.performed -= OnShipMaidDropAndSetObjectTypePositionCalled;
				ShipMaid.InputActionsInstance.LocationOverrideKey.Disable();
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
			if ((Object)(object)localPlayerController == null || !localPlayerController.isPlayerControlled || localPlayerController.isTypingChat || localPlayerController.inTerminalMenu || localPlayerController.IsServer && !localPlayerController.isHostPlayerObject || ConfigSettings.UseOnlyTerminal.Key.Value == "Enabled")
			{
				return;
			}
			if (!StartOfRound.Instance.shipHasLanded && !StartOfRound.Instance.inShipPhase)
			{
				ShipMaid.Log("Ship Has Not Landed and Not In Ship Phase, do not cleanup");

				return;
			}
			ShipMaid.Log("Cleanup Closet");

			LootOrganizingFunctions.OrganizeStorageCloset();
		}

		private static void OnShipMaidDropAndSetObjectTypePositionCalled(CallbackContext context)
		{
			if ((Object)(object)localPlayerController == null || !localPlayerController.isPlayerControlled || localPlayerController.isTypingChat || localPlayerController.inTerminalMenu || localPlayerController.IsServer && !localPlayerController.isHostPlayerObject)
			{
				return;
			}
			ShipMaid.Log("Set object cleanup position");

			ShipMaidFunctions.DropAndSetObjectTypePosition();
		}

		private static void OnShipMaidShipCleanupCalled(CallbackContext context)
		{
			if ((Object)(object)localPlayerController == null || !localPlayerController.isPlayerControlled || localPlayerController.isTypingChat || localPlayerController.inTerminalMenu || localPlayerController.IsServer && !localPlayerController.isHostPlayerObject || ConfigSettings.UseOnlyTerminal.Key.Value == "Enabled")
			{
				return;
			}
			if (!StartOfRound.Instance.shipHasLanded && !StartOfRound.Instance.inShipPhase)
			{
				ShipMaid.Log("Ship Has Not Landed and Not In Ship Phase, do not cleanup");
				return;
			}

			ShipMaid.Log("Cleanup Ship");

			LootOrganizingFunctions.OrganizeShipLoot();
		}

		private static void SubscribeToEvents()
		{
			if (ShipMaid.InputActionsInstance.ShipCleanupKey != null)
			{
				ShipMaid.InputActionsInstance.ShipCleanupKey.Enable();
				ShipMaid.InputActionsInstance.ShipCleanupKey.performed += OnShipMaidShipCleanupCalled;
			}
			if (ShipMaid.InputActionsInstance.StorageCleanupKey != null)
			{
				ShipMaid.InputActionsInstance.StorageCleanupKey.Enable();
				ShipMaid.InputActionsInstance.StorageCleanupKey.performed += OnShipMaidClosetCleanupCalled;
			}
			if (ShipMaid.InputActionsInstance.LocationOverrideKey != null)
			{
				ShipMaid.InputActionsInstance.LocationOverrideKey.Enable();
				ShipMaid.InputActionsInstance.LocationOverrideKey.performed += OnShipMaidDropAndSetObjectTypePositionCalled;
			}
		}
	}
}