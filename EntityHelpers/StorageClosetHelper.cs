using ShipMaid.HelperFunctions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ShipMaid.Networking.NetworkFunctions;

namespace ShipMaid.EntityHelpers
{
	public class StorageClosetHelper
	{
		public static Vector3 ClosetBoundsMax;
		public static Vector3 ClosetBoundsMin;
		private const float StorageLocationDoorOffsetToShelve = 0.2f;
		private string LastItemPlaced = string.Empty;
		private Vector3 LeftDoor;
		private float placementLocationX;
		private Vector3 RightDoor;
		private int shelveToPlaceOn = 1;
		private List<Vector3> ShevleListCenter = new List<Vector3>();
		private GameObject StorageCloset;
		private float StorageLocationXEnd;
		private float StorageLocationXStart;
		private float StorageLocationXStepItem = 0.2f;
		private float StorageLocationXStepSize = 0.1f;
		private float StorageLocationZOffset = 0.6f;

		public StorageClosetHelper()
		{
			StorageCloset = GameObject.Find("/Environment/HangarShip/StorageCloset");

			// Cube.000 and Cube.002 are door entities - use location for bounding
			var cube000 = StorageCloset.GetComponentsInChildren<Transform>().First(tf => tf.GetParent().name == "Cube.000").gameObject;
			var cube002 = StorageCloset.GetComponentsInChildren<Transform>().First(tf => tf.GetParent().name == "Cube.002").gameObject;

			LeftDoor = cube000.transform.position;
			RightDoor = cube002.transform.position;

			StorageLocationXStart = LeftDoor.x + StorageLocationDoorOffsetToShelve;
			StorageLocationXEnd = RightDoor.x - StorageLocationDoorOffsetToShelve;

			ShevleListCenter.Add(new((LeftDoor.x + RightDoor.x) / 2, 0.75f, LeftDoor.z));
			ShevleListCenter.Add(new((LeftDoor.x + RightDoor.x) / 2, 1.4f, LeftDoor.z));
			ShevleListCenter.Add(new((LeftDoor.x + RightDoor.x) / 2, 2f, LeftDoor.z));
			ShevleListCenter.Add(new((LeftDoor.x + RightDoor.x) / 2, 2.5f, LeftDoor.z));

			GameObject storageBounds = GameObject.Find("/Environment/HangarShip/StorageCloset");
			MeshCollider bounds = storageBounds.GetComponentInChildren<MeshCollider>();
			ClosetBoundsMin = new(bounds.bounds.min.x + 0.3f, bounds.bounds.min.y, bounds.bounds.min.z);
			ClosetBoundsMax = new(bounds.bounds.max.x - 0.3f, bounds.bounds.max.y, bounds.bounds.max.z);

			placementLocationX = StorageLocationXStart;
		}

		/// <summary>
		/// Get a list of all scrap in the storage closet.
		/// </summary>
		/// <returns>List of all scrap in storage closet.</returns>
		public List<GrabbableObject> GetObjectsInStorageCloset()
		{
			// Get all objects that can be picked up from inside the ship. Also remove items which technically have
			// scrap value but don't actually add to your quota.
			var loot = StorageCloset.GetComponentsInChildren<GrabbableObject>()
				.Where(obj => obj.name != "ClipboardManual" && obj.name != "StickyNoteItem").ToList();
			return loot;
		}

		/// <summary>
		/// Check if a GrabbableObject is wihtin the placement bounds.
		/// </summary>
		/// <returns>True if object is within placeable bounds.</returns>
		public bool IsObjectWithinCloset(GrabbableObject obj)
		{
			return PositionHelperFunctions.IsPositionWithinBounds(obj.gameObject.transform.position, ClosetBoundsMin, ClosetBoundsMax);
		}

		public bool IsPositionWithinCloset(Vector3 placementPosition)
		{
			return PositionHelperFunctions.IsPositionWithinBounds(placementPosition, ClosetBoundsMin, ClosetBoundsMax);
		}

		public void PlaceStorageObjectOnShelve(List<GrabbableObject> objectsOfType)
		{
			switch (objectsOfType.First().name)
			{
				case "Key(Clone)":
					StorageLocationXStepSize = 0.05f;
					break;

				case "WhoopieCushion(Clone)":
					StorageLocationXStepSize = 0.2f;
					break;

				default:
					StorageLocationXStepSize = 0.1f;
					break;
			}
			// If we are placing a new object type
			if (objectsOfType.First().name != LastItemPlaced && LastItemPlaced != string.Empty)
			{
				// if the all objects will not fit on this shelve
				if (placementLocationX + StorageLocationXStepItem + StorageLocationXStepSize * objectsOfType.Count > StorageLocationXEnd)
				{
					// Start on a new shelve
					placementLocationX = StorageLocationXStart;
					if (shelveToPlaceOn < 4)
						shelveToPlaceOn++;
				}
				else
				{
					// Otherwise adjust for the spacing between new items
					placementLocationX += StorageLocationXStepItem;
				}

				LastItemPlaced = objectsOfType.First().name;
			}
			else if (LastItemPlaced == string.Empty)
			{
				LastItemPlaced = objectsOfType.First().name;
			}

			// Setup forward backward shifting of objects in closet
			int forwardBackwardSpaces = 2;
			bool forwardBackward = true;
			int forwardBackwardStep = 0;
			float forwardBackwardStepSize = 0.1f;
			float forwardBackwardOffset = 0;

			for (int i = 0; i < objectsOfType.Count; i++)
			{
				GrabbableObject obj = objectsOfType[i];
				if (forwardBackward)
				{
					if (forwardBackwardStep < forwardBackwardSpaces)
					{
						forwardBackwardStep++;
					}
					else
					{
						forwardBackward = false;
					}
					forwardBackwardOffset += forwardBackwardStepSize;
				}
				else
				{
					if (forwardBackwardStep >= 0)
					{
						forwardBackwardStep--;
					}
					else
					{
						forwardBackward = true;
					}
					forwardBackwardOffset -= forwardBackwardStepSize;
				}

				Vector3 placementLocation = new(placementLocationX, ShevleListCenter[shelveToPlaceOn - 1].y, ShevleListCenter[shelveToPlaceOn - 1].z + StorageLocationZOffset + forwardBackwardOffset);// Check for and fix x locations out of bounds
				if (!IsPositionWithinCloset(placementLocation))
				{
					placementLocation = PositionHelperFunctions.AdjustPositionWithinBounds(placementLocation, ClosetBoundsMin, ClosetBoundsMax);
					ShipMaid.Log($"Fixed object location out of closet {obj.name}");
				}
				ShipMaid.Log($"Placing {obj.name} [{i + 1} of {objectsOfType.Count}] on shelve {shelveToPlaceOn} at {placementLocation.x},{placementLocation.y},{placementLocation.z}");

				NetworkingObjectManager.RunClientRpc(obj.NetworkObject, placementLocation, false);

				placementLocationX += StorageLocationXStepSize;
			}
			// Return shelve iterator to default setting
			switch (LastItemPlaced)
			{
				case "Key(Clone)":
					//shelveToPlaceOn = tempShelveToPlaceOn;
					StorageLocationXStepSize = 0.1f;
					break;

				default:
					StorageLocationXStepSize = 0.1f;
					break;
			}
		}
	}
}