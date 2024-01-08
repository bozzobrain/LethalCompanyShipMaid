using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;

namespace ShipMaid.HelperFunctions
{
	public static class PositionHelperFunctions
	{
		public static Vector3 AdjustPositionWithinBounds(Vector3 targetPosition, Vector3 min, Vector3 max)
		{
			if (targetPosition.x > max.x)
			{
				targetPosition.x = max.x;
			}
			if (targetPosition.y > max.y)
			{
				targetPosition.y = max.y;
			}
			if (targetPosition.z > max.z)
			{
				targetPosition.z = max.z;
			}

			if (targetPosition.x < min.x)
			{
				targetPosition.x = min.x;
			}
			if (targetPosition.y < min.y)
			{
				targetPosition.y = min.y;
			}
			if (targetPosition.z < min.z)
			{
				targetPosition.z = min.z;
			}
			return targetPosition;
		}

		public static bool IsPositionWithinBounds(Vector3 testPosition, Vector3 boundingPositionMin, Vector3 boundingPositionMax)
		{
			return (testPosition.x < boundingPositionMax.x) &&
				(testPosition.y < boundingPositionMax.y) &&
				(testPosition.z < boundingPositionMax.z) &&
				(testPosition.x > boundingPositionMin.x) &&
				(testPosition.y > boundingPositionMin.y) &&
				(testPosition.z > boundingPositionMin.z);
		}

		public static bool NearLocation(float f1, float f2, float offset)
		{
			return f1 < f2 + offset && f1 > f2 - offset;
		}

		public static bool SameLocation(Vector3 pos1, Vector3 pos2)
		{
			return NearLocation(pos1.x, pos2.x, 0.01f) && NearLocation(pos1.z, pos2.z, 0.01f);
		}
	}
}