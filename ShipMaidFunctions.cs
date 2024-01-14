using GameNetcodeStuff;
using HarmonyLib;
using ShipMaid.Configuration;
using ShipMaid.EntityHelpers;
using ShipMaid.ObjectHelpers;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using NetworkManager = Unity.Netcode.NetworkManager;

namespace ShipMaid
{
	[HarmonyPatch]
	internal class ShipMaidFunctions
	{
		public static List<GrabbableObjectPositionHelper> GrabbablesPositions = new();
		public static List<GrabbableObjectPositionHelper> OneHandedPositions = new();
		public static List<GrabbableObjectPositionHelper> TwoHandedPositions = new();

		internal static void DropAndSetObjectTypePosition()
		{
			//GrabbableObject obj = Keybinds.localPlayerController.ItemSlots[]; currentlyHeldObject
			GrabbableObject obj = GetCurrentlyHeldObject();
			if (obj != null)
			{
				if (GrabbablesPositions.Where(o => o.obj.name == obj.name).Any())
				{
					GrabbablesPositions.Where(o => o.obj.name == obj.name).First().PlacementPosition = obj.gameObject.transform.position;
				}
				else
				{
					GrabbablesPositions.Add(new(obj, obj.gameObject.transform.position));
				}
				if (obj.itemProperties.twoHanded)
				{
					if (TwoHandedPositions.Where(o => o.obj.itemProperties.twoHanded == true).Any())
					{
						TwoHandedPositions.Where(o => o.obj.itemProperties.twoHanded == true).First().PlacementPosition = obj.gameObject.transform.position;
					}
					else
					{
						TwoHandedPositions.Add(new(obj, obj.gameObject.transform.position));
					}
				}
				else
				{
					if (OneHandedPositions.Where(o => o.obj.itemProperties.twoHanded == false).Any())
					{
						OneHandedPositions.Where(o => o.obj.itemProperties.twoHanded == false).First().PlacementPosition = obj.gameObject.transform.position;
					}
					else
					{
						OneHandedPositions.Add(new(obj, obj.gameObject.transform.position));
					}
				}
				//Keybinds.localPlayerController.PlaceGrabbableObject(HangarShipHelper.GetShipTransform(), Keybinds.localPlayerController.transform.position, true, obj);
				//Keybinds.localPlayerController.PlaceGrabbableObject(HangarShipHelper.GetShipTransform(), new(0f, 0f, 0f), true, obj);
				//Keybinds.localPlayerController.DiscardHeldObject(true, HangarShipHelper.GetShipNetworkObject(), Keybinds.localPlayerController.transform.position, true);
				//Keybinds.localPlayerController.DespawnHeldObject();
			}
			else
			{
				ShipMaid.Log($"DropAndSetObjectTypePosition - No object held found");
			}
		}

		internal static GrabbableObject GetCurrentlyHeldObject()
		{
			for (int i = 0; i < Keybinds.localPlayerController.ItemSlots.Count(); i++)
			{
				if (Keybinds.localPlayerController.ItemSlots[i].isHeld && !Keybinds.localPlayerController.ItemSlots[i].isPocketed)
				{
					return Keybinds.localPlayerController.ItemSlots[i];
				}
			}
			return null;
		}

		internal static GrabbableObjectPositionHelper GetObjectPositionTarget(GrabbableObject obj)
		{
			if (GrabbablesPositions.Where(o => o.obj.name == obj.name).Any())
			{
				return GrabbablesPositions.Where(o => o.obj.name == obj.name).First();
			}

			return null;
		}

		internal static GrabbableObjectPositionHelper GetOneHandedPositionTarget()
		{
			if (OneHandedPositions.Where(o => o.obj.itemProperties.twoHanded == false).Any())
			{
				return OneHandedPositions.Where(o => o.obj.itemProperties.twoHanded == false).First();
			}

			return null;
		}

		internal static GrabbableObjectPositionHelper GetTwoHandedPositionTarget()
		{
			if (TwoHandedPositions.Where(o => o.obj.itemProperties.twoHanded == true).Any())
			{
				return TwoHandedPositions.Where(o => o.obj.itemProperties.twoHanded == true).First();
			}

			return null;
		}

		internal static void initItemLocations()
		{
			List<string> ItemsForStorageCloset = ConfigSettings.ClosetLocationOverride.GetStrings(ConfigSettings.ClosetLocationOverride.Key.Value);
			HangarShipHelper hsh = new();
			var allItems = hsh.FindAllScrapShip();
			if (ConfigSettings.UseItemTypePlacementOverrides.Key.Value == "Enabled")
			{
				foreach (var obj in allItems)
				{
					if (ItemsForStorageCloset.Where(ifsc => ifsc.Equals(obj.name)).Any())
						continue;
					if (GrabbablesPositions.Where(o => o.obj.name == obj.name).Any())
					{
						GrabbablesPositions.Where(o => o.obj.name == obj.name).First().PlacementPosition = obj.gameObject.transform.position;
					}
					else
					{
						GrabbablesPositions.Add(new(obj, obj.gameObject.transform.position));
					}
				}
			}
			if (ConfigSettings.UseOneHandedPlacementOverrides.Key.Value == "Enabled")
			{
				foreach (var obj in allItems)
				{
					if (ItemsForStorageCloset.Where(ifsc => ifsc.Equals(obj.name)).Any())
						continue;
					if (!obj.itemProperties.twoHanded)
					{
						if (OneHandedPositions.Where(o => o.obj.itemProperties.twoHanded == false).Any())
						{
							OneHandedPositions.Where(o => o.obj.itemProperties.twoHanded != true).First().PlacementPosition = obj.gameObject.transform.position;
						}
						else
						{
							OneHandedPositions.Add(new(obj, obj.gameObject.transform.position));
						}
					}
				}
			}
			if (ConfigSettings.UseTwoHandedPlacementOverrides.Key.Value == "Enabled")
			{
				foreach (var obj in allItems)
				{
					if (ItemsForStorageCloset.Where(ifsc => ifsc.Equals(obj.name)).Any())
						continue;
					if (obj.itemProperties.twoHanded)
					{
						if (TwoHandedPositions.Where(o => o.obj.itemProperties.twoHanded == true).Any())
						{
							TwoHandedPositions.Where(o => o.obj.itemProperties.twoHanded == true).First().PlacementPosition = obj.gameObject.transform.position;
						}
						else
						{
							TwoHandedPositions.Add(new(obj, obj.gameObject.transform.position));
						}
					}
				}
			}
		}
	}
}