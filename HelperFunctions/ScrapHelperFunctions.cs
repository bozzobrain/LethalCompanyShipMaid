using HarmonyLib;
using ShipMaid.EntityHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShipMaid.HelperFunctions
{
	public static class ScrapHelperFunctions
	{
		/// <summary>
		/// Get a list of all scrap in the ship.
		/// </summary>
		/// <returns>List of all scrap in ship.</returns>
		private static void ReportScrapList(string where)
		{
			if (where == "ship")
			{
				HangarShipHelper hsh = new();
				var shipObjects = hsh.ObjectsInShip();
				shipObjects.Do(scrap => ShipMaid.Log($"{scrap.name} - ${scrap.scrapValue} - ${scrap.targetFloorPosition.x}- ${scrap.targetFloorPosition.y}- ${scrap.targetFloorPosition.z}"));
			}
			else if (where == "closet")
			{
				StorageClosetHelper sch = new();
				var closetObjects = sch.GetObjectsInStorageCloset();
				closetObjects.Do(scrap => ShipMaid.Log($"{scrap.name} - ${scrap.scrapValue} - ${scrap.targetFloorPosition.x}- ${scrap.targetFloorPosition.y}- ${scrap.targetFloorPosition.z}"));
			}
		}

		/// <summary>
		/// Get the highest value of the loot in the ship.
		/// </summary>
		/// <returns>The value of the highest valued loot on the ship.</returns>
		public static float CalculateHighestScrapValue(List<GrabbableObject> objects)
		{
			float highestScrap = 0;

			foreach (GrabbableObject obj in objects)
			{
				if (obj.scrapValue > highestScrap)
				{
					highestScrap = obj.scrapValue;
				}
			}

			return highestScrap;
		}

		/// <summary>
		/// Get a list of all scrap on the map ouside of the ship room.
		/// </summary>
		/// <returns>List of all scrap on map.</returns>
		internal static List<GrabbableObject> FindAllScrapOnMap()
		{
			List<GrabbableObject> scrapList = new List<GrabbableObject>();

			var genericObjectThatIsScrap = UnityEngine.Object.FindObjectsByType<GrabbableObject>(FindObjectsSortMode.None);
			foreach (var actualScrap in genericObjectThatIsScrap)
			{
				if (!actualScrap.isInShipRoom)
				{
					scrapList.Add(actualScrap);
					ShipMaid.Log($"Found scrap: {actualScrap.name}");
				}
			}
			return scrapList;
		}
	}
}