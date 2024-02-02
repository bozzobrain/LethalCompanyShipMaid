using ShipMaid.Configuration;
using ShipMaid.HelperFunctions;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ShipMaid.Networking.NetworkFunctions;
using Unity.Netcode;
using TMPro;

namespace ShipMaid.EntityHelpers
{
	public class HangarShipHelper
	{
		public static Vector3 ShipBoundsCenter;
		public static Vector3 ShipBoundsExtents;
		public static Vector3 ShipBoundsMaxBack;
		public static Vector3 ShipBoundsMaxFront;
		public static Vector3 ShipBoundsMinBack;
		public static Vector3 ShipBoundsMinFront;
		public static Vector3 ShipCenter;
		public static Vector3 ShipCenterForPlacement;
		public static Vector3 ShipCollider;
		public static Vector3 ShipExtends;
		public static GameObject ShipObject;
		public static float XLocationZChange = 4f;

		public HangarShipHelper()
		{
			ShipObject = GameObject.Find("/Environment/HangarShip");
			ShipCenter = ShipObject.transform.position;
			GameObject shipBounds = GameObject.Find("/Environment/HangarShip/ShipInside");
			MeshCollider bounds = shipBounds.GetComponentInChildren<MeshCollider>();
			ShipBoundsCenter = bounds.bounds.m_Center;
			ShipBoundsExtents = bounds.bounds.m_Extents;
			ShipCenterForPlacement = new(
				ShipCenter.z - 6.25f,
				ShipCenter.x - 5.25f,
				ShipCenter.y + 1.66f);
			// These values are used to ensure the loots arms and legs stay in the ship at all times
			ShipBoundsMinBack = new(bounds.bounds.min.x + 1.75f, bounds.bounds.min.y, bounds.bounds.min.z + 0.5f);
			ShipBoundsMaxBack = new(bounds.bounds.max.x - 2f, bounds.bounds.max.y, bounds.bounds.max.z - 0.5f);

			ShipBoundsMinFront = new(bounds.bounds.min.x + 1.75f, bounds.bounds.min.y, bounds.bounds.min.z + 2f);
			ShipBoundsMaxFront = new(bounds.bounds.max.x - 2f, bounds.bounds.max.y, bounds.bounds.max.z - 1.75f);
			//ShipMaid.Log($"Inside dimensions {ShipBoundsMax.x - ShipBoundsMin.x},{ShipBoundsMax.y - ShipBoundsMin.y},{ShipBoundsMax.z - ShipBoundsMin.z}");
		}

		/// <summary>
		/// Get a string of the ship bounds min to ship bounds max.
		/// </summary>
		/// <returns>string of x,y,z to x,y,z [min to max].</returns>
		public static string GetDebugLocationShip(Vector3 targetPosition)
		{
			if (targetPosition.x > XLocationZChange)
				return $"{ShipBoundsMinBack.x},{ShipBoundsMinBack.y},{ShipBoundsMinBack.z} to {ShipBoundsMaxBack.x},{ShipBoundsMaxBack.y},{ShipBoundsMaxBack.z}";
			else
				return $"{ShipBoundsMinFront.x},{ShipBoundsMinFront.y},{ShipBoundsMinFront.z} to {ShipBoundsMaxFront.x},{ShipBoundsMaxFront.y},{ShipBoundsMaxFront.z}";
		}

		/// <summary>
		/// Get transform for the ship.
		/// </summary>
		/// <returns>Transform object of the ship.</returns>
		public static NetworkObject GetShipNetworkObject()
		{
			GameObject ship = GameObject.Find("/Environment/HangarShip");
			return ship.gameObject.GetComponent<NetworkObject>();
		}

		/// <summary>
		/// Get transform for the ship.
		/// </summary>
		/// <returns>Transform object of the ship.</returns>
		public static Transform GetShipTransform()
		{
			GameObject ship = GameObject.Find("/Environment/HangarShip");
			return ship.gameObject.transform;
		}

		/// <summary>
		/// Makes target position bounded by ShipBounds
		/// </summary>
		/// <returns>Vector3 position of the target bounded by ship.</returns>
		public Vector3 AdjustPositionWithinShip(Vector3 targetPosition)
		{
			if (targetPosition.x > XLocationZChange)
				return PositionHelperFunctions.AdjustPositionWithinBounds(targetPosition, ShipBoundsMinBack, ShipBoundsMaxBack);
			else
				return PositionHelperFunctions.AdjustPositionWithinBounds(targetPosition, ShipBoundsMinFront, ShipBoundsMaxFront);
		}

		/// <summary>
		/// Get a list of all scrap on the ship.
		/// </summary>
		/// <returns>List of all scrap in ship.</returns>
		public List<GrabbableObject> FindAllScrapShip()
		{
			List<GrabbableObject> scrapList = new();

			var genericObjectThatIsScrap = Object.FindObjectsByType<GrabbableObject>(FindObjectsSortMode.None);
			foreach (var actualScrap in genericObjectThatIsScrap)
			{
				if (actualScrap.isInShipRoom)
				{
					scrapList.Add(actualScrap);
				}
			}
			return scrapList;
		}

		/// <summary>
		/// Get position inside of the ship for object placement.
		/// </summary>
		/// <returns>Vector3 position of ship to place objects.</returns>
		public Vector3 GetShipCenterLocation()
		{
			GameObject ship = GameObject.Find("/Environment/HangarShip");
			Vector3 shiplocation = ship.transform.position;
			shiplocation.z += -6.25f;
			shiplocation.x += -5.25f;
			shiplocation.y += 1.66f;
			return shiplocation;
		}

		/// <summary>
		/// Check if a GrabbableObject is wihtin the placement bounds.
		/// </summary>
		/// <returns>True if object is within placeable bounds.</returns>
		public bool IsObjectWithinShip(GrabbableObject obj)
		{
			if (obj.gameObject.transform.position.x > XLocationZChange)
				return PositionHelperFunctions.IsPositionWithinBounds(obj.gameObject.transform.position, ShipBoundsMinBack, ShipBoundsMaxBack);
			else
				return PositionHelperFunctions.IsPositionWithinBounds(obj.gameObject.transform.position, ShipBoundsMinFront, ShipBoundsMaxFront);
		}

		/// <summary>
		/// Check if a GrabbableObject is wihtin the placement bounds.
		/// </summary>
		/// <returns>True if object is within placeable bounds.</returns>
		public bool IsPositionWithinShip(Vector3 position)
		{
			if (position.x > XLocationZChange)
				return PositionHelperFunctions.IsPositionWithinBounds(position, ShipBoundsMinBack, ShipBoundsMaxBack);
			else
				return PositionHelperFunctions.IsPositionWithinBounds(position, ShipBoundsMinFront, ShipBoundsMaxFront);
		}

		/// <summary>
		/// Move a GrabbableObject to the ship.
		/// </summary>
		///
		public void MoveItemToShip(GrabbableObject obj)
		{
			//ShipMaid.Log($"Moving Item To Ship Center - {obj.name}");
			obj.hasBeenHeld = true;
			obj.isInFactory = false;

			obj.isInShipRoom = true;
			RoundManager.Instance.scrapCollectedInLevel += obj.scrapValue;
			StartOfRound.Instance.gameStats.allPlayerStats[Keybinds.localPlayerController.playerClientId].profitable += obj.scrapValue;
			RoundManager.Instance.CollectNewScrapForThisRound(obj);
			obj.transform.SetParent(ShipObject.transform);
			obj.OnBroughtToShip();

			NetworkingObjectManager.RunClientRpc(obj.NetworkObject, GetShipCenterLocation(), obj.transform.rotation, true);
			return;
		}

		/// <summary>
		/// Get a list of all scrap in the ship.
		/// </summary>
		/// <returns>List of all scrap in ship.</returns>
		public List<GrabbableObject> ObjectsInShip()
		{
			// Get all objects that can be picked up from inside the ship. Also remove items which technically have
			// scrap value but don't actually add to your quota.
			var loot = ShipObject.GetComponentsInChildren<GrabbableObject>()
				.Where(obj => obj.name != "ClipboardManual" && obj.name != "StickyNoteItem").ToList();
			return loot;
		}
	}
}