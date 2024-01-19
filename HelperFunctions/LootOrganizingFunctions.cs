using ShipMaid.Configuration;
using ShipMaid.EntityHelpers;
using ShipMaid.ObjectHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ShipMaid.Networking.NetworkFunctions;

namespace ShipMaid.HelperFunctions
{
	public class LootOrganizingFunctions
	{
		private static List<string> ItemsForStorageCloset = ConfigSettings.ClosetLocationOverride.GetStrings(ConfigSettings.ClosetLocationOverride.Key.Value);
		private static List<string> SortingBlacklist = ConfigSettings.SortingLocationBlacklist.GetStrings(ConfigSettings.SortingLocationBlacklist.Key.Value);

		/// <summary>
		/// Organizes the scrap in the ship.
		/// </summary>
		///
		public static void OrganizeShipLoot()
		{
			var sch = new StorageClosetHelper();
			var hsh = new HangarShipHelper();

			var shipObjects = hsh.ObjectsInShip();
			var storageClosetObjects = sch.GetObjectsInStorageCloset();

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
			OrganizeItems(sch, oneHandedObjects, false);
			OrganizeItems(sch, twoHandedObjects, true);
		}

		/// <summary>
		/// Organizes the scrap in the storage closet.
		/// </summary>
		///
		public static void OrganizeStorageCloset()
		{
			var sch = new StorageClosetHelper();
			var storageClosetObjects = sch.GetObjectsInStorageCloset();
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
					sch.PlaceStorageObjectOnShelve(objectsOfType);
				}
			}
		}

		/// <summary>
		/// Get a value of x offset for a given scrap. Higher values have higher x offsets.
		/// Scale the values by 3 units to group them but order by value
		/// </summary>
		/// <returns>Offset x value scaled by scrap value.</returns>
		private static float GetXOffsetFromScrapValue(GrabbableObject obj, float highestScrapValue, float maxXOffset)
		{
			return ((obj.scrapValue - 10) / highestScrapValue) * maxXOffset;
		}

		private static void OrganizeItems(StorageClosetHelper sch, List<GrabbableObject> objects, bool twoHanded)
		{
			HangarShipHelper hsh = new();
			// Get all object types and make a list of them
			List<string> objectNames = new List<string>();
			foreach (var scrap in objects)
			{
				if (!objectNames.Contains(scrap.name))
				{
					objectNames.Add(scrap.name);
				}
			}
			float xPositionOffset = 0;

			// Organize two handed object in a different location than single handed
			// Single handed objects are closer to the door
			// calculate a z offset that places objects on different z location by type
			float frontOfShipAreaZOffset = 2.25f / objectNames.Count;
			float frontOfShipAreaXOffset = 0;
			float backOfShipAreaZOffset = 2.75f / objectNames.Count;
			float backOfShipAreaXOffset = 7;
			bool targetLocationFront = false;
			float objectTypeZOffset = 0;
			float objectTypeXOffset = 0;
			if (twoHanded)
			{
				if (ConfigSettings.TwoHandedItemLocation.Key.Value == "Front")
				{
					objectTypeXOffset = frontOfShipAreaXOffset;
					objectTypeZOffset = frontOfShipAreaZOffset;
					targetLocationFront = true;
				}
				else
				{
					objectTypeXOffset = backOfShipAreaXOffset;
					objectTypeZOffset = backOfShipAreaZOffset;
				}
			}
			else
			{
				if (ConfigSettings.TwoHandedItemLocation.Key.Value == "Front")
				{
					objectTypeZOffset = backOfShipAreaZOffset;
					objectTypeXOffset = backOfShipAreaXOffset;
				}
				else
				{
					objectTypeXOffset = frontOfShipAreaXOffset;
					objectTypeZOffset = frontOfShipAreaZOffset;
					targetLocationFront = true;
				}
			}

			int itemCounter = 0;

			// Organize items by the name of the object (like objects together)
			foreach (var objectType in objectNames)
			{
				var objectsOfType = objects.Where(obj => obj.name.Contains(objectType)).ToList();

				// If object is on blacklist (forced to closet) - place in closet
				if (ItemsForStorageCloset.Where(objectType.Contains).Any())
				{
					sch.PlaceStorageObjectOnShelve(objectsOfType);
					continue;
				}
				else if (SortingBlacklist.Where(objectType.Contains).Any())
				{
					continue;
				}
				// Make sure this item is not being held currently
				var firstObjectOfType = objectsOfType.FirstOrDefault(obj => !obj.isHeld);

				// Make sure first object is not null in type
				if (firstObjectOfType != null)
				{
					// Keep track of offset locations to disuade locations that identical (same scrap value)
					List<float> offsetLocations = new List<float>();

					// if stacking move the stacks around a bit to separate piles
					if (ConfigSettings.OrganizationTechnique.Key.Value == "Stack" && ConfigSettings.UseItemTypePlacementOverrides.Key.Value != "Enabled" && ConfigSettings.UseOneHandedPlacementOverrides.Key.Value != "Enabled" && ConfigSettings.UseTwoHandedPlacementOverrides.Key.Value != "Enabled")
					{
						System.Random r = new();
						xPositionOffset = (float)r.NextDouble() * r.Next(-1, 2);

						// Force objects to the boundaries for debugging
						//xPositionOffset = r.Next(-1,2);

						if (ConfigSettings.ItemGrouping.Key.Value == "Loose") xPositionOffset *= 2.0f;
					}
					Vector3 placementPosition;
					if (ShipMaidFunctions.GetObjectPositionTarget(firstObjectOfType) is GrabbableObjectPositionHelper goph && goph != null && ConfigSettings.UseItemTypePlacementOverrides.Key.Value == "Enabled")
					{
						ShipMaid.Log($"Setting position from memory for {firstObjectOfType.name}");
						placementPosition = goph.PlacementPosition;
					}
					else if (ShipMaidFunctions.GetTwoHandedPositionTarget() is Vector3 goph_TwoHanded && goph_TwoHanded != null && ConfigSettings.UseTwoHandedPlacementOverrides.Key.Value == "Enabled" && twoHanded)
					{
						ShipMaid.LogError($"Setting Two Handed position from memory for {firstObjectOfType.name} - {goph_TwoHanded.x},{goph_TwoHanded.y},{goph_TwoHanded.z}");
						placementPosition = goph_TwoHanded;
					}
					else if (ShipMaidFunctions.GetOneHandedPositionTarget() is Vector3 goph_OneHanded && goph_OneHanded != null && ConfigSettings.UseOneHandedPlacementOverrides.Key.Value == "Enabled" && !twoHanded)
					{
						ShipMaid.LogError($"Setting One Handed position from memory for {firstObjectOfType.name} - {goph_OneHanded.x},{goph_OneHanded.y},{goph_OneHanded.z}");
						placementPosition = goph_OneHanded;
					}
					else
					{
						// Find placement location adjust z by small amount for each type of object
						placementPosition = new(hsh.GetShipCenterLocation().x, hsh.GetShipCenterLocation().y, hsh.GetShipCenterLocation().z);
						if (ConfigSettings.ItemGrouping.Key.Value == "Loose")
							placementPosition.z -= objectTypeZOffset * itemCounter;
						else
							placementPosition.z -= (objectTypeZOffset * itemCounter * 0.1f) + objectTypeZOffset * 0.9f * objectNames.Count;

						// Objects in back of shop can be moved closer to the wall
						if (!targetLocationFront)
							placementPosition.z += 0.5f;

						// Shift item position by scrap value (higher value is closer to door)
						placementPosition.x = hsh.GetShipCenterLocation().x + objectTypeXOffset;
					}

					foreach (var obj in objectsOfType)
					{
						// Make sure we dont move a held object
						if (obj.isHeld)
							continue;

						// Choose how to organze each item of loot
						if (ConfigSettings.OrganizationTechnique.Key.Value == "Value" && ConfigSettings.UseItemTypePlacementOverrides.Key.Value != "Enabled" && ConfigSettings.UseOneHandedPlacementOverrides.Key.Value != "Enabled" && ConfigSettings.UseTwoHandedPlacementOverrides.Key.Value != "Enabled")
						{
							if (targetLocationFront)
							{
								placementPosition.x += GetXOffsetFromScrapValue(obj, ScrapHelperFunctions.CalculateHighestScrapValue(hsh.ObjectsInShip()), 2.5f);
							}
							else
							{
								placementPosition.x += GetXOffsetFromScrapValue(obj, ScrapHelperFunctions.CalculateHighestScrapValue(hsh.ObjectsInShip()), 4f);
							}
							// If we already placed an item here, move it by a small amount to offset common values
							while (offsetLocations.Contains(placementPosition.x))
							{
								placementPosition.x += 0.1f;
							}
							offsetLocations.Add(placementPosition.x);
						}
						else if (ConfigSettings.UseItemTypePlacementOverrides.Key.Value != "Enabled" && ConfigSettings.UseOneHandedPlacementOverrides.Key.Value != "Enabled" && ConfigSettings.UseTwoHandedPlacementOverrides.Key.Value != "Enabled")
						{
							placementPosition.x += xPositionOffset;
						}

						// Make sure object is within ship (fix if outside boundaries)
						if (!hsh.IsPositionWithinShip(placementPosition))
						{
							var newPosition = hsh.AdjustPositionWithinShip(placementPosition);
							ShipMaid.LogError($"Ship loot detected outside of ship - {obj.name} - {placementPosition.x},{placementPosition.y},{placementPosition.z}");
							placementPosition = new(newPosition.x, newPosition.y, newPosition.z);
							ShipMaid.LogError($"Corrected to- {obj.name} - {placementPosition.x},{placementPosition.y},{placementPosition.z}");
						}
						// Move the object if position needs adjusted
						if (!PositionHelperFunctions.SameLocation(obj.transform.position, placementPosition))
						{
							ShipMaid.Log($"Moving item to ship - {obj.name} - {placementPosition.x},{placementPosition.y},{placementPosition.z}");
							NetworkingObjectManager.MakeObjectFallRpc(obj, placementPosition, true);
							if (!hsh.IsObjectWithinShip(obj))
							{
								ShipMaid.Log($"Found item outside of the ship - {obj.name}");
							}
						}
					}
					itemCounter++;
				}
			}
			// TODO - This seems like a hacky approach (double movement)
			OrganizeStorageCloset();
		}
	}
}