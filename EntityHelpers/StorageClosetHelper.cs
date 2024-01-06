using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShipMaid.EntityHelpers
{
	public class StorageClosetHelper
	{
		GameObject StorageCloset;
		Vector3 StorageClosetPosition;
		Vector3 LeftDoor;
		Vector3 RightDoor;
		Vector3 Shelve1Center;
		Vector3 Shelve2Center;
		Vector3 Shelve3Center;
		Vector3 Shelve4Center;

		string LastItemPlaced = string.Empty;

		float placementLocationX;
		int shelveToPlaceOn = 1;

		float StorageLocationXStart;
		float StorageLocationXEnd;

		const float StorageLocationDoorOffsetToShelve = .25f;
		public StorageClosetHelper()
		{
			StorageCloset = GameObject.Find("/Environment/HangarShip/StorageCloset");
			StorageClosetPosition = StorageCloset.transform.position;

			// Cube.000
			var cube000 = StorageCloset.GetComponentsInChildren<Transform>().First(tf => tf.GetParent().name == "Cube.000").gameObject;
			var cube002 = StorageCloset.GetComponentsInChildren<Transform>().First(tf => tf.GetParent().name == "Cube.002").gameObject;
			//var cube002 = StorageCloset.GetComponent("Cube.002").gameObject;
			LeftDoor = cube000.transform.position;
			RightDoor = cube002.transform.position;

			StorageLocationXStart = LeftDoor.x + StorageLocationDoorOffsetToShelve;
			StorageLocationXEnd = RightDoor.x - StorageLocationDoorOffsetToShelve;

			//Shelve1Center = new((LeftDoor.x + RightDoor.x) / 2, 0.67f, LeftDoor.z);
			//Shelve2Center = new((LeftDoor.x + RightDoor.x) / 2, 0.6f, LeftDoor.z);
			//Shelve3Center = new((LeftDoor.x + RightDoor.x) / 2, 0.43f, LeftDoor.z);
			//Shelve4Center = new((LeftDoor.x + RightDoor.x) / 2, 0.27f, LeftDoor.z);
			Shelve1Center = new((LeftDoor.x + RightDoor.x) / 2, 0.75f, LeftDoor.z);
			Shelve2Center = new((LeftDoor.x + RightDoor.x) / 2, 1.4f, LeftDoor.z);
			Shelve3Center = new((LeftDoor.x + RightDoor.x) / 2, 2f, LeftDoor.z);
			Shelve4Center = new((LeftDoor.x + RightDoor.x) / 2, 2.5f, LeftDoor.z);

			placementLocationX = StorageLocationXStart;

		}
		float StorageLocationXStepSize = 0.2f;
		float StorageLocationXStepItem = 0.4f;
		float StorageLocationZOffset = 0.5f;
		public void PlaceStorageObjectOnShelve(List<GrabbableObject> objectsOfType)
		{
			// If we are placing a new object type
			if (objectsOfType.First().name != LastItemPlaced && LastItemPlaced!=string.Empty)
			{

				if (placementLocationX + StorageLocationXStepItem + StorageLocationXStepSize * objectsOfType.Count > StorageLocationXEnd)
				{
					placementLocationX = StorageLocationXStart;
					shelveToPlaceOn++;
				}


				LastItemPlaced = objectsOfType.First().name;
			}
			else if(LastItemPlaced == string.Empty)
			{
				LastItemPlaced = objectsOfType.First().name;

			}
			int tempShelveToPlaceOn = shelveToPlaceOn;
			switch(LastItemPlaced)
			{
				case "Key(Clone)":
					shelveToPlaceOn = 4;
					StorageLocationXStepSize = 0.1f;
					break;
				case "WhoopieCushion(Clone)":
					shelveToPlaceOn = 3;
					StorageLocationXStepSize = 0.2f;
					break;
				default:
					StorageLocationXStepSize = 0.1f;
					break;
			}

			for (int i = 0; i < objectsOfType.Count; i++)
			{
				GrabbableObject obj = objectsOfType[i];
				ShipMaid.Log($"Placing {obj.name} [{i+1} of {objectsOfType.Count}] on shelve {shelveToPlaceOn}");
				placementLocationX += StorageLocationXStepSize;
				switch (shelveToPlaceOn)
				{
					case 1:

						ShipMaidFunctions.MakeObjectFallRpc(obj, new(placementLocationX, Shelve1Center.y, Shelve1Center.z + StorageLocationZOffset),false);
						break;
					case 2:
						ShipMaidFunctions.MakeObjectFallRpc(obj, new(placementLocationX, Shelve2Center.y, Shelve2Center.z + StorageLocationZOffset),false);

						break;
					case 3:
						ShipMaidFunctions.MakeObjectFallRpc(obj, new(placementLocationX, Shelve3Center.y, Shelve3Center.z + StorageLocationZOffset), false);

						break;
					case 4:
						ShipMaidFunctions.MakeObjectFallRpc(obj, new(placementLocationX, Shelve4Center.y, Shelve4Center.z + StorageLocationZOffset), false);

						break;
				}

			}
			// Return shelve iterator to default setting
			switch (LastItemPlaced)
			{
				case "Key(Clone)":
					shelveToPlaceOn = tempShelveToPlaceOn;
					break;
				default:
					StorageLocationXStepSize = 0.2f;
					break;
			}

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
	}
}
