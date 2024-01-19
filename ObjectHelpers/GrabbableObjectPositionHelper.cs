using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShipMaid.ObjectHelpers
{
	public class GrabbableObjectPositionHelper
	{
		public string objName;

		public Vector3 PlacementPosition;

		public GrabbableObjectPositionHelper(string name, Vector3 pos)
		{
			objName = name;
			PlacementPosition = pos;
		}

		public GrabbableObjectPositionHelper(string name, float xPos, float yPos, float zPos)
		{
			objName = name;
			PlacementPosition = new(xPos, yPos, zPos);
		}
	}
}