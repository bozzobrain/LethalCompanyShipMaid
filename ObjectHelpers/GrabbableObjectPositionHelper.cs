using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShipMaid.ObjectHelpers
{
	internal class GrabbableObjectPositionHelper
	{
		public GrabbableObject obj;

		public Vector3 PlacementPosition;

		public GrabbableObjectPositionHelper(GrabbableObject o, Vector3 pos)
		{
			obj = o;
			PlacementPosition = pos;
		}

		public GrabbableObjectPositionHelper(GrabbableObject o, float xPos, float yPos, float zPos)
		{
			obj = o;
			PlacementPosition = new(xPos, yPos, zPos);
		}
	}
}