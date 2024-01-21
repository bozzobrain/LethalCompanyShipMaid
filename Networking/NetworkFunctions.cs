using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace ShipMaid.Networking
{
	internal class NetworkFunctions
	{
		public class NetworkingObjectManager : NetworkBehaviour
		{
			public static void MakeObjectFall(GrabbableObject obj, Vector3 placementPosition, bool shipParent)
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
				ShipMaid.Log($"Request to make GrabbableObject {obj.name} fall to ground in {debugLocation} - {targetlocation.x},{targetlocation.y},{targetlocation.z}");
				obj.gameObject.transform.SetPositionAndRotation(placementPosition, obj.transform.rotation);
				obj.hasHitGround = false;
				obj.startFallingPosition = placementPosition;
				if (obj.transform.parent != null)
				{
					obj.startFallingPosition = obj.transform.parent.InverseTransformPoint(obj.startFallingPosition);
				}
				obj.FallToGround(false);
			}

			[ClientRpc]
			public static void MakeObjectFallClientRpc(NetworkObjectReference obj, Vector3 placementPosition, bool shipParent)
			{
				NetworkManager networkManager = NetworkManager.Singleton;
				if ((object)networkManager == null || !networkManager.IsListening)
				{
					return;
				}

				FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);
				bufferWriter.WriteValueSafe(in obj, default);
				bufferWriter.WriteValueSafe(in placementPosition);
				bufferWriter.WriteValueSafe(shipParent);
				NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("MakeObjectFall", bufferWriter, NetworkDelivery.Reliable);

				if (obj.TryGet(out var networkObject))
				{
					GrabbableObject component = networkObject.GetComponent<GrabbableObject>();
					if (!Keybinds.localPlayerController.IsOwner)
					{
						MakeObjectFall(component, placementPosition, shipParent);
					}
				}
			}

			[ServerRpc]
			public static void MakeObjectFallServerRpc(NetworkObjectReference obj, Vector3 placementPosition, bool shipParent)
			{
				NetworkManager networkManager = NetworkManager.Singleton;
				if ((object)networkManager == null)
				{
					ShipMaid.LogError("Network Manager == null");
					return;
				}
				if (!networkManager.IsListening)
				{
					ShipMaid.LogError("Network Manager  not listening");
					return;
				}

				if (Keybinds.localPlayerController.OwnerClientId != networkManager.LocalClientId)
				{
					if (networkManager.LogLevel <= LogLevel.Normal)
					{
						Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
					}

					return;
				}

				FastBufferWriter bufferWriter = new FastBufferWriter(256, Unity.Collections.Allocator.Temp);
				bufferWriter.WriteValueSafe(in obj, default);
				bufferWriter.WriteValueSafe(placementPosition);
				bufferWriter.WriteValueSafe(shipParent);
				NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll("MakeObjectFall", bufferWriter, NetworkDelivery.Reliable);

				if (obj.TryGet(out var networkObject))
				{
					GrabbableObject component = networkObject.GetComponent<GrabbableObject>();

					MakeObjectFall(component, placementPosition, shipParent);
				}
				else
				{
					ShipMaid.LogError("if (obj.TryGet(out var networkObject))");
				}
			}

			public static void NetworkManagerInit()
			{
				ShipMaid.LogError("Registering named message");
				NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("MakeObjectFall", (senderClientId, reader) =>
				{
					if (senderClientId != Keybinds.localPlayerController.playerClientId)
					{
						reader.ReadValueSafe(out NetworkObjectReference value, default);
						reader.ReadValueSafe(out Vector3 value3);
						reader.ReadValueSafe(out bool shipParent);
						if (value.TryGet(out var networkObject))
						{
							GrabbableObject component = networkObject.GetComponent<GrabbableObject>();

							MakeObjectFall(component, value3, shipParent);
						}
					}
				});
			}

			[ClientRpc]
			public static void RunClientRpc(NetworkObjectReference obj, Vector3 placementPosition, bool shipParent)
			{
				MakeObjectFallServerRpc(obj, placementPosition, shipParent);
			}
		}
	}
}