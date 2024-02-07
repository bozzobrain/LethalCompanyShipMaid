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
		private const float StorageLocationXOffsetToShelve = 0.2f;
		private Vector3 ClosetRotation;
		private string LastItemPlaced = string.Empty;
		private float placementLocationAcrossOffset = 0;
		private int shelveToPlaceOn = 1;
		private List<Vector3> ShevleListCenter = new List<Vector3>();
		private GameObject StorageCloset;
		private Vector3 StorageLocationEnd;
		private Vector3 StorageLocationStart;
		private float StorageLocationXRange;
		private float StorageLocationXStepItem = 0.2f;
		private float StorageLocationXStepSize = 0.1f;
		private float StorageLocationZOffset = 0.2f;
		private float StorageLocationZRange;

		public StorageClosetHelper()
		{
			StorageCloset = GameObject.Find("/Environment/HangarShip/StorageCloset");
			MeshCollider storageMeshCollider = StorageCloset.GetComponentInChildren<MeshCollider>();

			ClosetRotation = StorageCloset.gameObject.transform.rotation.eulerAngles;

			StorageLocationStart = new((storageMeshCollider.bounds.min.x + Mathf.Cos(StorageLocationXOffsetToShelve * Mathf.Deg2Rad)), (0), (storageMeshCollider.bounds.max.z - Mathf.Sin(StorageLocationXOffsetToShelve * Mathf.Deg2Rad) - Mathf.Cos(StorageLocationXOffsetToShelve * Mathf.Deg2Rad)));
			StorageLocationEnd = new((storageMeshCollider.bounds.max.x - Mathf.Cos(StorageLocationXOffsetToShelve * Mathf.Deg2Rad)), (0), (storageMeshCollider.bounds.min.z - Mathf.Cos(StorageLocationXOffsetToShelve * Mathf.Deg2Rad)));

			ClosetBoundsMin = new(storageMeshCollider.bounds.min.x + StorageLocationXOffsetToShelve, storageMeshCollider.bounds.min.y, storageMeshCollider.bounds.min.z);
			ClosetBoundsMax = new(storageMeshCollider.bounds.max.x - StorageLocationXOffsetToShelve, storageMeshCollider.bounds.max.y, storageMeshCollider.bounds.max.z);

			StorageLocationZRange = storageMeshCollider.bounds.max.z - storageMeshCollider.bounds.min.z;
			StorageLocationXRange = storageMeshCollider.bounds.max.x - storageMeshCollider.bounds.min.x;

			ShevleListCenter.Add(new(storageMeshCollider.bounds.center.x, 0.75f, storageMeshCollider.bounds.min.z + 0.5f));
			ShevleListCenter.Add(new(storageMeshCollider.bounds.center.x, 1.4f, storageMeshCollider.bounds.min.z + 0.5f));
			ShevleListCenter.Add(new(storageMeshCollider.bounds.center.x, 2f, storageMeshCollider.bounds.min.z + 0.5f));
			ShevleListCenter.Add(new(storageMeshCollider.bounds.center.x, 2.5f, storageMeshCollider.bounds.min.z + 0.5f));

			//ShipMaid.LogError($"Rotation of storage - {PositionHelperFunctions.DebugVector3(ClosetRotation)}");
			//ShipMaid.LogError($"StorageLocationStart - {PositionHelperFunctions.DebugVector3(StorageLocationStart)}");
			//ShipMaid.LogError($"Collider min - {PositionHelperFunctions.DebugVector3(storageMeshCollider.bounds.min)}");
			//ShipMaid.LogError($"Collider max - {PositionHelperFunctions.DebugVector3(storageMeshCollider.bounds.max)}");
			//ShipMaid.LogError($"Width of storage - {StorageLocationXRange}");
			//ShipMaid.LogError($"Depth of storage - {StorageLocationZRange}");
			//ShipMaid.LogError($"Center of storage - {PositionHelperFunctions.DebugVector3(storageMeshCollider.bounds.center)}");
		}

		/// <summary>
		/// Get a string of the ship bounds min to ship bounds max.
		/// </summary>
		/// <returns>string of x,y,z to x,y,z [min to max].</returns>
		public static string GetDebugLocationCloset()
		{
			return $"{ClosetBoundsMin.x},{ClosetBoundsMin.y},{ClosetBoundsMin.z} to {ClosetBoundsMax.x},{ClosetBoundsMax.y},{ClosetBoundsMax.z}";
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
				if (placementLocationAcrossOffset + StorageLocationXStepItem + StorageLocationXStepSize * objectsOfType.Count > 2.25f)
				{
					// Start on a new shelve
					placementLocationAcrossOffset = 0;
					if (shelveToPlaceOn < 4)
						shelveToPlaceOn++;
				}
				else
				{
					// Otherwise adjust for the spacing between new items
					placementLocationAcrossOffset += StorageLocationXStepItem;
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
			float forwardBackwardOffset = StorageLocationZOffset;

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

				// Handle a rotated storage closet
				Vector3 itemOffsetOriginal = new(placementLocationAcrossOffset, ShevleListCenter[shelveToPlaceOn - 1].y, forwardBackwardOffset);
				Vector3 itemOffsetFinal = Quaternion.Euler(0, ClosetRotation.y, 0) * itemOffsetOriginal;
				Vector3 StorageClosetOffsets = Quaternion.Euler(0, ClosetRotation.y, 0) * new Vector3(-1.75f, 0, -0.75f);
				Vector3 placementLocation = new(itemOffsetFinal.x + StorageCloset.gameObject.transform.position.x + StorageClosetOffsets.x, ShevleListCenter[shelveToPlaceOn - 1].y, itemOffsetFinal.z + StorageCloset.gameObject.transform.position.z + StorageClosetOffsets.z);

				if (!IsPositionWithinCloset(placementLocation))
				{
					ShipMaid.Log($"Fixed object location out of closet {obj.name} - Original Location {PositionHelperFunctions.DebugVector3(placementLocation)} -  Bounds are {GetDebugLocationCloset()}");
					placementLocation = PositionHelperFunctions.AdjustPositionWithinBounds(placementLocation, ClosetBoundsMin, ClosetBoundsMax);
				}
				ShipMaid.Log($"Placing {obj.name} [{i + 1} of {objectsOfType.Count}] on shelve {shelveToPlaceOn} at {PositionHelperFunctions.DebugVector3(placementLocation)}");

				NetworkingObjectManager.RunClientRpc(obj.NetworkObject, placementLocation, obj.transform.rotation, false);

				placementLocationAcrossOffset += StorageLocationXStepSize;
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