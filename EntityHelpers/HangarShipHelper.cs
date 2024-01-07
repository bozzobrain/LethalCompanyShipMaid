using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShipMaid.EntityHelpers
{
	public class HangarShipHelper
	{
		public static float xMax;
		public static float xMin;
		public static GameObject ShipObject;
		public static Vector3 ShipCenter;
		public static Vector3 ShipExtends;
		public static Vector3 ShipCollider;
		public static Vector3 ShipBoundsCenter;
		public static Vector3 ShipBoundsExtents;
		public static Vector3 ShipBoundsMin;
		public static Vector3 ShipBoundsMax;
		public HangarShipHelper()
		{
			ShipObject = GameObject.Find("/Environment/HangarShip");
			ShipCenter = ShipObject.transform.position;
			GameObject shipBounds = GameObject.Find("/Environment/HangarShip/ShipInside");
			MeshCollider bounds = shipBounds.GetComponentInChildren<MeshCollider>();
			ShipBoundsCenter = bounds.bounds.m_Center;
			ShipBoundsExtents = bounds.bounds.m_Extents;
			ShipBoundsMin = bounds.bounds.min;
			ShipBoundsMax = bounds.bounds.max;

			// These values are used to ensure the loots arms and legs stay in the ship at all times
			xMax = ShipBoundsMax.x - 7f; 
			xMin = ShipBoundsMin.x + 1.75f;
		}

		public bool IsPositionInsideShip(Vector3 tr)
		{
			return (tr.x < xMax) &&
				(tr.y < ShipBoundsMax.y) &&
				(tr.z < ShipBoundsMax.z) &&
				(tr.x > xMin) &&
				(tr.y > ShipBoundsMin.y) &&
				(tr.z > ShipBoundsMin.z);
		}

		public Vector3 AdjustPositionWithinShip(Vector3 targetPosition)
		{
			if (targetPosition.x > xMax)
			{
				targetPosition.x = xMax;
			}
			if (targetPosition.y > ShipBoundsMax.y)
			{
				targetPosition.y = ShipBoundsMax.y;
			}
			if (targetPosition.z > ShipBoundsMax.z)
			{
				targetPosition.z = ShipBoundsMax.z;
			}


			if (targetPosition.x < xMin)
			{
				targetPosition.x = xMin;
			}
			if (targetPosition.y < ShipBoundsMin.y)
			{
				targetPosition.y = ShipBoundsMin.y;
			}
			if (targetPosition.z < ShipBoundsMin.z)
			{
				targetPosition.z = ShipBoundsMin.z;
			}
			return targetPosition;
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


		/// <summary>
		/// Check if a GrabbableObject is wihtin the placement bounds.
		/// </summary>
		/// <returns>True if object is within placeable bounds.</returns>
		public bool IsObjectWithinShip(GrabbableObject obj)
		{
			return (obj.gameObject.transform.position.x < xMax) &&
				(obj.gameObject.transform.position.y < ShipBoundsMax.y) &&
				(obj.gameObject.transform.position.z < ShipBoundsMax.z) && 
				(obj.gameObject.transform.position.x > xMin) &&
				(obj.gameObject.transform.position.y > ShipBoundsMin.y) &&
				(obj.gameObject.transform.position.z > ShipBoundsMin.z);
		}
	}
}
