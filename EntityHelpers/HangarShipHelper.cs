using ShipMaid.Configuration;
using ShipMaid.HelperFunctions;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ShipMaid.Networking.NetworkFunctions;

namespace ShipMaid.EntityHelpers
{
	public class HangarShipHelper
	{
		public static Vector3 ShipCenterForPlacement;
		public static Vector3 ShipBoundsCenter;
		public static Vector3 ShipBoundsExtents;
		public static Vector3 ShipBoundsMax;
		public static Vector3 ShipBoundsMin;
		public static Vector3 ShipCenter;
		public static Vector3 ShipCollider;
		public static Vector3 ShipExtends;
		public static GameObject ShipObject;

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
			ShipBoundsMin = new(bounds.bounds.min.x + 1.75f, bounds.bounds.min.y, bounds.bounds.min.z);
			ShipBoundsMax = new(bounds.bounds.max.x - 7f, bounds.bounds.max.y, bounds.bounds.max.z);
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
		/// Makes target position bounded by ShipBounds
		/// </summary>
		/// <returns>Vector3 position of the target bounded by ship.</returns>
		public Vector3 AdjustPositionWithinShip(Vector3 targetPosition)
		{
			return PositionHelperFunctions.AdjustPositionWithinBounds(targetPosition, ShipBoundsMin, ShipBoundsMax);
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
			return PositionHelperFunctions.IsPositionWithinBounds(obj.gameObject.transform.position, ShipBoundsMin, ShipBoundsMax);
		}

		/// <summary>
		/// Check if a GrabbableObject is wihtin the placement bounds.
		/// </summary>
		/// <returns>True if object is within placeable bounds.</returns>
		public bool IsPositionWithinShip(Vector3 position)
		{
			return PositionHelperFunctions.IsPositionWithinBounds(position, ShipBoundsMin, ShipBoundsMax);
		}

		/// <summary>
		/// Move a GrabbableObject to the ship.
		/// </summary>
		///
		public void MoveItemToShip(GrabbableObject obj)
		{
			ShipMaid.Log($"Moving Item To Ship Center - {obj.name}");
			obj.hasBeenHeld = true;
			obj.isInFactory = false;

			obj.isInShipRoom = true;
			RoundManager.Instance.scrapCollectedInLevel += obj.scrapValue;
			StartOfRound.Instance.gameStats.allPlayerStats[Keybinds.localPlayerController.playerClientId].profitable += obj.scrapValue;
			RoundManager.Instance.CollectNewScrapForThisRound(obj);
			obj.transform.SetParent(ShipObject.transform);
			obj.OnBroughtToShip();

			NetworkingObjectManager.MakeObjectFallRpc(obj, GetShipCenterLocation(), true);
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