using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShipMaid.ObjectHelpers
{
	public class GrabbableObjectPositionHelperShotgun
	{
		public int AmmoQuantity;
		public string objName;

		public Vector3 PlacementPosition;

		public GrabbableObjectPositionHelperShotgun(string name, Vector3 pos, int ammoQuantity)
		{
			objName = name;
			PlacementPosition = pos;
			AmmoQuantity = ammoQuantity;
		}

		public GrabbableObjectPositionHelperShotgun(string name, float xPos, float yPos, float zPos, int ammoQuantity)
		{
			objName = name;
			PlacementPosition = new(xPos, yPos, zPos);
			AmmoQuantity = ammoQuantity;
		}
	}
}