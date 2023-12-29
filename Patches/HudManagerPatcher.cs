using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ShipMaid.Patches
{

	[HarmonyPatch]
	internal class HudManagerPatcher
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.PingScan_performed))]
		private static void OnScan(HUDManager __instance, InputAction.CallbackContext context)
		{
			ShipMaid.Log.LogDebug("On scan.");

			if (GameNetworkManager.Instance.localPlayerController == null)
				return;
			if (!context.performed || !__instance.CanPlayerScan() || __instance.playerPingingScan > -0.5f)
				return;
			// Only allow this special scan to work while inside the ship.
			if (!StartOfRound.Instance.inShipPhase && !GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom)
				return;
			
			//ReportScrapList("closet");

			OrganizeStorageCloset(); 
			OrganizeShipLoot();
		}

	

		/// <summary>
		/// Calculate the value of all scrap in the ship.
		/// </summary>
		/// <returns>The total scrap value.</returns>
		private static float CalculateLootValue()
		{
			GameObject ship = GameObject.Find("/Environment/HangarShip");
			// Get all objects that can be picked up from inside the ship. Also remove items which technically have
			// scrap value but don't actually add to your quota.
			var loot = ship.GetComponentsInChildren<GrabbableObject>()
				.Where(obj => obj.name != "ClipboardManual" && obj.name != "StickyNoteItem").ToList();
			ShipMaid.Log.LogDebug("Calculating total ship scrap value.");
			loot.Do(scrap => ShipMaid.Log.LogDebug($"{scrap.name} - ${scrap.scrapValue}"));
			return loot.Sum(scrap => scrap.scrapValue);
		}

		/// <summary>
		/// Get a list of all scrap in the ship.
		/// </summary>
		/// <returns>List of all scrap in ship.</returns>
		private static List<GrabbableObject> ObjectsInShip()
		{
			GameObject ship = GameObject.Find("/Environment/HangarShip");
			// Get all objects that can be picked up from inside the ship. Also remove items which technically have
			// scrap value but don't actually add to your quota.
			var loot = ship.GetComponentsInChildren<GrabbableObject>()
				.Where(obj => obj.name != "ClipboardManual" && obj.name != "StickyNoteItem").ToList();
			return loot;
		}

		/// <summary>
		/// Get a list of all scrap in the ship.
		/// </summary>
		/// <returns>List of all scrap in ship.</returns>
		private static void ReportScrapList(string where)
		{
			if (where == "ship")
			{
				var shipObjects = ObjectsInShip();
				shipObjects.Do(scrap => ShipMaid.Log.LogInfo($"{scrap.name} - ${scrap.scrapValue} - ${scrap.targetFloorPosition.x}- ${scrap.targetFloorPosition.y}- ${scrap.targetFloorPosition.z}"));
			}
			else if (where == "closet")
			{
				var closetObjects = GetObjectsInStorageCloset();
				closetObjects.Do(scrap => ShipMaid.Log.LogInfo($"{scrap.name} - ${scrap.scrapValue} - ${scrap.targetFloorPosition.x}- ${scrap.targetFloorPosition.y}- ${scrap.targetFloorPosition.z}"));
			}

		}

		/// <summary>
		/// Get a list of all scrap in the storage closet.
		/// </summary>
		/// <returns>List of all scrap in storage closet.</returns>
		private static List<GrabbableObject> GetObjectsInStorageCloset()
		{
			GameObject storageCloset = GameObject.Find("/Environment/HangarShip/StorageCloset");
			// Get all objects that can be picked up from inside the ship. Also remove items which technically have
			// scrap value but don't actually add to your quota.
			var loot = storageCloset.GetComponentsInChildren<GrabbableObject>()
				.Where(obj => obj.name != "ClipboardManual" && obj.name != "StickyNoteItem").ToList();
			return loot;
		}

		/// <summary>
		/// Organizes the scrap in the storage closet.
		/// </summary>
		/// 
		private static void OrganizeStorageCloset()
		{
			GameObject storageCloset = GameObject.Find("/Environment/HangarShip/StorageCloset");
			var storageClosetObjects = GetObjectsInStorageCloset();
			List<string> objectNames = new List<string>();
			foreach (var scrap in storageClosetObjects)
			{
				if (!objectNames.Contains(scrap.name))
				{
					objectNames.Add(scrap.name);
				}
			}

			foreach (var objectType in objectNames)
			{				
				var objectsOfType = storageClosetObjects.Where(obj => obj.name.Contains(objectType)).ToList();

				// Make sure this item is not being held currently
				var firstObjectOfType = objectsOfType.FirstOrDefault(obj=>!obj.isHeld);			

				if (firstObjectOfType != null)
				{					
					var placementPosition = firstObjectOfType.transform.position;
					var placementRotation = firstObjectOfType.transform.rotation;
					foreach (var obj in objectsOfType)
					{
						// Make sure we dont move a held object
						if (obj.isHeld)
							continue;						

						ShipMaid.Log.LogInfo($"Moving object - {obj.name} - From: {obj.transform.position.x},{obj.transform.position.y},{obj.transform.position.z} to {placementPosition.x},{placementPosition.y},{placementPosition.z}");
						obj.gameObject.transform.SetPositionAndRotation(placementPosition, placementRotation);
						

						obj.hasHitGround = false;
						obj.startFallingPosition = obj.transform.position;
						if (obj.transform.parent != null)
						{
							obj.startFallingPosition = obj.transform.parent.InverseTransformPoint(obj.startFallingPosition);
						}
						obj.FallToGround(false);

						placementPosition.x += 0.1f;
					}
				}
			}
		}

		/// <summary>
		/// Organizes the scrap in the ship.
		/// </summary>
		/// 
		private static void OrganizeShipLoot()
		{
			var shipObjects = ObjectsInShip();
			var storageClosetObjects = GetObjectsInStorageCloset();

			// Do not adjust the storage closet objects
			foreach(var storageClosetObject in storageClosetObjects)
			{
				if(shipObjects.Contains(storageClosetObject))
				{
					shipObjects.Remove(storageClosetObject);
				}
			}

			// Get all object types and make a list of them
			List<string> objectNames = new List<string>();
			foreach (var scrap in shipObjects)
			{
				if (!objectNames.Contains(scrap.name))
				{
					objectNames.Add(scrap.name);
				}
			}

			// Organize items by the name of the object (like objects together)
			foreach (var objectType in objectNames)
			{
				var objectsOfType = shipObjects.Where(obj => obj.name.Contains(objectType)).ToList();

				// Make sure this item is not being held currently
				var firstObjectOfType = objectsOfType.FirstOrDefault(obj => !obj.isHeld);

				if (firstObjectOfType != null)
				{
					var placementPosition = firstObjectOfType.transform.position;
					foreach (var obj in objectsOfType)
					{
						// Make sure we dont move a held object
						if (obj.isHeld)
							continue;

						ShipMaid.Log.LogInfo($"Moving object - {obj.name} - From: {obj.transform.position.x},{obj.transform.position.y},{obj.transform.position.z} to {placementPosition.x},{placementPosition.y},{placementPosition.z}");
						obj.gameObject.transform.SetPositionAndRotation(placementPosition, obj.transform.rotation);


						obj.hasHitGround = false;
						obj.startFallingPosition = obj.transform.position;
						if (obj.transform.parent != null)
						{
							obj.startFallingPosition = obj.transform.parent.InverseTransformPoint(obj.startFallingPosition);
						}
						obj.FallToGround(false);

						//placementPosition.x += 0.01f;
					}
				}
			}
		}	
	}

}
