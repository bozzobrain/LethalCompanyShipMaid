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
		public static Vector3 OneHandedPosition = new(UnsetPosition.x, UnsetPosition.y, UnsetPosition.z);
		public static Vector3 TwoHandedPosition = new(UnsetPosition.x, UnsetPosition.y, UnsetPosition.z);
		public static Vector3 UnsetPosition = new(-9f, -9f, -9f);

		internal static void DropAndSetObjectTypePosition()
		{
			//GrabbableObject obj = Keybinds.localPlayerController.ItemSlots[]; currentlyHeldObject
			GrabbableObject obj = GetCurrentlyHeldObject();
			if (obj != null)
			{
				Vector3 goPos = obj.gameObject.transform.position;
				if (GrabbablesPositions.Where(o => o.objName == obj.name).Any())
				{
					GrabbablesPositions.Where(o => o.objName == obj.name).First().PlacementPosition = new(goPos.x, goPos.y, goPos.z);
					GrabbableObjectPositionHelper goph = new(obj.name, goPos);
					ConfigSettings.ItemPlacementOverrideLocation.AddOrUpdateObjectPositionSetting(goph);
					ShipMaid.instance.Config.Save();
				}
				else
				{
					GrabbablesPositions.Add(new(obj.name, goPos));
					GrabbableObjectPositionHelper goph = new(obj.name, goPos);
					ConfigSettings.ItemPlacementOverrideLocation.AddOrUpdateObjectPositionSetting(goph);
					ShipMaid.instance.Config.Save();
				}
				if (obj.itemProperties.twoHanded)
				{
					TwoHandedPosition = new(goPos.x, goPos.y, goPos.z);
					ConfigSettings.TwoHandedItemPlacementOverrideLocation.SetVector3(TwoHandedPosition);
					ShipMaid.instance.Config.Save();
				}
				else
				{
					ShipMaid.LogError($"Updating OneHanded Position");
					OneHandedPosition = new(goPos.x, goPos.y, goPos.z);
					ConfigSettings.OneHandedItemPlacementOverrideLocation.SetVector3(OneHandedPosition);
					ShipMaid.instance.Config.Save();
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
			if (GrabbablesPositions.Where(o => o.objName == obj.name).Any())
			{
				return GrabbablesPositions.Where(o => o.objName == obj.name).First();
			}

			return null;
		}

		internal static Vector3? GetOneHandedPositionTarget()
		{
			if (OneHandedPosition != UnsetPosition)
			{
				return OneHandedPosition;
			}

			return null;
		}

		internal static Vector3? GetTwoHandedPositionTarget()
		{
			if (TwoHandedPosition != UnsetPosition)
			{
				return TwoHandedPosition;
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
				var itemPlacementListConfig = ConfigSettings.ItemPlacementOverrideLocation.GetObjectPositionList(ConfigSettings.ItemPlacementOverrideLocation.Key.Value);
				if (itemPlacementListConfig.Count > 0 && itemPlacementListConfig.First().objName != "name")
				{
					foreach (var itemPlacement in itemPlacementListConfig)
					{
						GrabbablesPositions.Add(itemPlacement);
					}
				}
				else
				{
					foreach (var obj in allItems)
					{
						if (ItemsForStorageCloset.Where(ifsc => ifsc.Equals(obj.name)).Any())
							continue;
						if (GrabbablesPositions.Where(o => o.objName == obj.name).Any())
						{
							GrabbablesPositions.Where(o => o.objName == obj.name).First().PlacementPosition = obj.gameObject.transform.position;
						}
						else
						{
							GrabbablesPositions.Add(new(obj.name, obj.gameObject.transform.position));
						}
					}
				}
			}
			if (ConfigSettings.UseOneHandedPlacementOverrides.Key.Value == "Enabled")
			{
				if (ConfigSettings.OneHandedItemPlacementOverrideLocation.GetVector3(ConfigSettings.OneHandedItemPlacementOverrideLocation.Key.Value, out Vector3 oneHandedOverrideLocation))
				{
					OneHandedPosition = oneHandedOverrideLocation;
				}
				else
				{
					foreach (var obj in allItems)
					{
						if (ItemsForStorageCloset.Where(ifsc => ifsc.Equals(obj.name)).Any())
							continue;
						if (!obj.itemProperties.twoHanded)
						{
							if (OneHandedPosition == UnsetPosition)
							{
								OneHandedPosition = obj.gameObject.transform.position;
							}
						}
					}
				}
			}
			if (ConfigSettings.UseTwoHandedPlacementOverrides.Key.Value == "Enabled")
			{
				if (ConfigSettings.TwoHandedItemPlacementOverrideLocation.GetVector3(ConfigSettings.TwoHandedItemPlacementOverrideLocation.Key.Value, out Vector3 twoHandedOverrideLocation))
				{
					TwoHandedPosition = twoHandedOverrideLocation;
				}
				else
				{
					foreach (var obj in allItems)
					{
						if (ItemsForStorageCloset.Where(ifsc => ifsc.Equals(obj.name)).Any())
							continue;
						if (obj.itemProperties.twoHanded)
						{
							if (TwoHandedPosition == UnsetPosition)
							{
								TwoHandedPosition = obj.gameObject.transform.position;
								break;
							}
						}
					}
				}
			}
		}
	}
}