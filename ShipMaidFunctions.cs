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

namespace ShipMaid
{

	[HarmonyPatch]
	internal class ShipMaidFunctions
	{
		/// <summary>
		/// Get position of the ship.
		/// </summary>
		/// <returns>Vector3 position of ship.</returns>
		private static Vector3 GetShipLocation()
		{
			GameObject ship = GameObject.Find("/Environment/HangarShip");
			return ship.transform.position;
		}

		/// <summary>
		/// Get position inside of the ship for object placement.
		/// </summary>
		/// <returns>Vector3 position of ship to place objects.</returns>
		private static Vector3 GetShipCenterLocation()
		{
			GameObject ship = GameObject.Find("/Environment/HangarShip");
			Vector3 shiplocation = ship.transform.position;
			shiplocation.z += -5.75f;
			shiplocation.x += -4.85f;
			shiplocation.y += 1.66f;
			return shiplocation;
		}

		/// <summary>
		/// Get a value of x offset for a given scrap. Higher values have higher x offsets.
		/// Scale the values by 3 units to group them but order by value
		/// </summary>
		/// <returns>Offset x value scaled by scrap value.</returns>
		private static float GetXOffsetFromScrapValue(GrabbableObject obj)
		{
			return ((obj.scrapValue - 10) / 200f) * 5;
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
				shipObjects.Do(scrap => ShipMaid.Log($"{scrap.name} - ${scrap.scrapValue} - ${scrap.targetFloorPosition.x}- ${scrap.targetFloorPosition.y}- ${scrap.targetFloorPosition.z}"));
			}
			else if (where == "closet")
			{
				var closetObjects = GetObjectsInStorageCloset();
				closetObjects.Do(scrap => ShipMaid.Log($"{scrap.name} - ${scrap.scrapValue} - ${scrap.targetFloorPosition.x}- ${scrap.targetFloorPosition.y}- ${scrap.targetFloorPosition.z}"));
			}
		}

		private static bool NearLocation(float f1, float f2, float offset)
		{
			return f1 < f2 + offset && f1 > f2 - offset;

		}

		private static bool SameLocation(Vector3 pos1, Vector3 pos2)
		{
			return NearLocation(pos1.x, pos2.x, 0.01f) && NearLocation(pos1.z, pos2.z, 0.01f);
		}

		/// <summary>
		/// Organizes the scrap in the storage closet.
		/// </summary>
		/// 
		public static void OrganizeStorageCloset()
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
				var firstObjectOfType = objectsOfType.FirstOrDefault(obj => !obj.isHeld);

				if (firstObjectOfType != null)
				{
					var placementPosition = firstObjectOfType.transform.position;
					var placementRotation = firstObjectOfType.transform.rotation;
					foreach (var obj in objectsOfType)
					{
						// Make sure we dont move a held object
						if (obj.isHeld)
							continue;

						ShipMaid.Log($"Moving object - {obj.name} - From: {obj.transform.position.x},{obj.transform.position.y},{obj.transform.position.z} to {placementPosition.x},{placementPosition.y},{placementPosition.z}");
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
		public static void OrganizeShipLoot()
		{
			var shipObjects = ObjectsInShip();
			var storageClosetObjects = GetObjectsInStorageCloset();

			// Do not adjust the storage closet objects
			foreach (var storageClosetObject in storageClosetObjects)
			{
				if (shipObjects.Contains(storageClosetObject))
				{
					shipObjects.Remove(storageClosetObject);
				}
			}


			// Sort objects by two handed and one handed
			List<GrabbableObject> twoHandedObjects = new List<GrabbableObject>();
			List<GrabbableObject> oneHandedObjects = new List<GrabbableObject>();
			foreach (var scrap in shipObjects)
			{
				if (scrap.itemProperties.twoHanded)
				{
					twoHandedObjects.Add(scrap);
				}
				else
				{
					oneHandedObjects.Add(scrap);
				}
			}
			OrganizeItems(oneHandedObjects, false);
			OrganizeItems(twoHandedObjects, true);

		}


		private static void OrganizeItems(List<GrabbableObject> objects, bool twoHanded)
		{
			// Organize two handed object in a different location than single handed
			// Single handed objects are closer to the door
			var twoHandedOffset = 0;
			if (twoHanded)
			{
				twoHandedOffset = 4;
			}

			// Get all object types and make a list of them
			List<string> objectNames = new List<string>();
			foreach (var scrap in objects)
			{
				if (!objectNames.Contains(scrap.name))
				{
					objectNames.Add(scrap.name);
				}
			}

			// calculate a z offset that places objects on different z location by type
			float objectTypeZOffset = 3f / objectNames.Count;
			if (twoHanded)
			{
				objectTypeZOffset = 4.5f / objectNames.Count;
			}
			int itemCounter = 0;

			// Organize items by the name of the object (like objects together)
			foreach (var objectType in objectNames)
			{
				var objectsOfType = objects.Where(obj => obj.name.Contains(objectType)).ToList();

				// Make sure this item is not being held currently
				var firstObjectOfType = objectsOfType.FirstOrDefault(obj => !obj.isHeld);

				// Keep track of offset locations to disuade locations that identical (same scrap value)
				List<float> offsetLocations = new List<float>();

				// Make sure first object is not null in type
				if (firstObjectOfType != null)
				{
					// Find placement location adjust z by small amount for each type of object
					var placementPosition = GetShipCenterLocation();
					placementPosition.z -= objectTypeZOffset * itemCounter;

					// Two handed objects can be moved closer to the wall
					if (twoHanded)
						placementPosition.z += 1f;

					foreach (var obj in objectsOfType)
					{
						// Make sure we dont move a held object
						if (obj.isHeld)
							continue;

						// Shift item position by scrap value (higher value is closer to door)
						placementPosition.x = GetShipCenterLocation().x + GetXOffsetFromScrapValue(obj) + twoHandedOffset;

						// If we already placed an item here, move it by a small amount to offset common values
						while (offsetLocations.Contains(placementPosition.x))
						{
							placementPosition.x += 0.1f;
						}
						offsetLocations.Add(placementPosition.x);

						// Move the object if position needs adjusted
						if (!SameLocation(obj.transform.position, placementPosition))
						{
							ShipMaid.Log($"Moving object - {obj.name} - From: {obj.transform.position.x},{obj.transform.position.y},{obj.transform.position.z} to {placementPosition.x},{placementPosition.y},{placementPosition.z}");
							obj.gameObject.transform.SetPositionAndRotation(placementPosition, obj.transform.rotation);

							obj.hasHitGround = false;
							obj.startFallingPosition = obj.transform.position;
							if (obj.transform.parent != null)
							{
								obj.startFallingPosition = obj.transform.parent.InverseTransformPoint(obj.startFallingPosition);
							}
							obj.FallToGround(false);
						}
					}
					itemCounter++;
				}
			}
		}
	}
}
