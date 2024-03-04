using GameNetcodeStuff;
using HarmonyLib;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;

namespace ShipMaid.Networking
{
	internal class NetworkFunctions
	{
		public class NetworkingObjectManager : NetworkBehaviour
		{
			public static void MakeObjectFall(GrabbableObject obj, Vector3 placementPosition, Quaternion placementRotation, bool shipParent)
			{
				GameObject ship = GameObject.Find("/Environment/HangarShip");
				GameObject storageCloset = GameObject.Find("/Environment/HangarShip/StorageCloset");
				string debugLocation = string.Empty;
				Vector3 targetlocation = new();
				if (shipParent)
				{
					if (obj.gameObject.transform.GetParent() == null || obj.gameObject.transform.GetParent().name != "HangarShip")
					{
						obj.gameObject.transform.SetParent(ship.transform);
					}
					targetlocation = placementPosition;
					debugLocation = "ship";
				}
				else
				{
					if (obj.gameObject.transform.GetParent() == null || obj.gameObject.transform.GetParent().name != "StorageCloset")
					{
						obj.gameObject.transform.SetParent(storageCloset.transform);
					}
					targetlocation = placementPosition;
					debugLocation = "storage";
				}
				//ShipMaid.Log($"Request to make GrabbableObject {obj.name} fall to ground in {debugLocation} - {targetlocation.x},{targetlocation.y},{targetlocation.z}");
				obj.gameObject.transform.SetPositionAndRotation(placementPosition, placementRotation);
				obj.hasHitGround = false;
				obj.startFallingPosition = placementPosition;
				obj.floorYRot = -1;
				//obj.itemProperties.restingRotation = placementRotation.eulerAngles;

				if (obj.transform.parent != null)
				{
					obj.startFallingPosition = obj.transform.parent.InverseTransformPoint(obj.startFallingPosition);
				}
				obj.FallToGround(false);
				obj.floorYRot = -1;
			}

			[ClientRpc]
			public static void MakeObjectFallClientRpc(NetworkObjectReference obj, Vector3 placementPosition, Quaternion placementRotation, bool shipParent)
			{
				NetworkManager networkManager = NetworkManager.Singleton;
				if ((object)networkManager == null || !networkManager.IsListening)
				{
					return;
				}

				FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);
				bufferWriter.WriteValueSafe(in obj, default);
				bufferWriter.WriteValueSafe(in placementPosition);
				bufferWriter.WriteValueSafe(in placementRotation);
				bufferWriter.WriteValueSafe(shipParent);
				NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("MakeObjectFall", bufferWriter, NetworkDelivery.Reliable);

				if (obj.TryGet(out var networkObject))
				{
					if (networkObject.TryGetComponent(out GrabbableObject component))
					{
						if (!Keybinds.localPlayerController.IsOwner)
						{
							MakeObjectFall(component, placementPosition, placementRotation, shipParent);
						}
					}
					else
					{
						ShipMaid.LogError("Failed to get grabbable object ref from network object - ClientRpc");
					}
				}
			}

			[ServerRpc]
			public static void MakeObjectFallServerRpc(NetworkObjectReference obj, Vector3 placementPosition, Quaternion placementRotation, bool shipParent)
			{
				NetworkManager networkManager = NetworkManager.Singleton;
				if ((object)networkManager == null)
				{
					ShipMaid.LogError("Network Manager == null");
					return;
				}
				if (!networkManager.IsListening)
				{
					ShipMaid.LogError("Network Manager not listening");
					return;
				}

				if (Keybinds.localPlayerController.OwnerClientId != networkManager.LocalClientId)
				{
					if (networkManager.LogLevel <= LogLevel.Normal)
					{
						ShipMaid.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
					}

					return;
				}

				FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);
				bufferWriter.WriteValueSafe(in obj, default);
				bufferWriter.WriteValueSafe(placementPosition);
				bufferWriter.WriteValueSafe(placementRotation);
				bufferWriter.WriteValueSafe(shipParent);
				NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("MakeObjectFall", bufferWriter, NetworkDelivery.Reliable);

				if (obj.TryGet(out var networkObject))
				{
					if (networkObject.TryGetComponent(out GrabbableObject component))
					{
						MakeObjectFall(component, placementPosition, placementRotation, shipParent);
					}
					else
					{
						ShipMaid.LogError("Failed to get grabbable object ref from network object - ServerRpc");
					}
				}
			}

			public static void NetworkManagerInit()
			{
				ShipMaid.Log("Registering named message");
				NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("MakeObjectFall", (senderClientId, reader) =>
				{
					if (senderClientId != Keybinds.localPlayerController.playerClientId)
					{
						reader.ReadValueSafe(out NetworkObjectReference GrabbableObjectRef, default);
						reader.ReadValueSafe(out Vector3 position);
						reader.ReadValueSafe(out Quaternion rotation);
						reader.ReadValueSafe(out bool shipParent);
						if (GrabbableObjectRef.TryGet(out var GrabbableObjectNetworkObj))
						{
							GrabbableObject component = GrabbableObjectNetworkObj.GetComponent<GrabbableObject>();
							MakeObjectFall(component, position, rotation, shipParent);
						}
					}
				});
			}

			[ClientRpc]
			public static void RunClientRpc(NetworkObjectReference obj, Vector3 placementPosition, Quaternion placementRotation, bool shipParent)
			{
				MakeObjectFallServerRpc(obj, placementPosition, placementRotation, shipParent);
			}
		}
	}
}