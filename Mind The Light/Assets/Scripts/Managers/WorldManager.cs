using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Photon.Pun;


public class WorldManager : MonoBehaviourPun {

   public static WorldManager Instance;

   [Header("Objects Spawn Points")]
   public Transform[] spawnPointsObjects;
   public Transform[] spawnPointsKeys;

   private List<TargetObject> targetObjects;
   private List<GameObject> keysObjects;
   private List<Door> doors;
   private List<LightSwitch> lightSwitches;

   private PhotonView PV;

   private void Awake() {
      Instance = this;

      PV = GetComponent<PhotonView>();

      targetObjects = FindObjectsOfType<TargetObject>().ToList();
      targetObjects.Sort((t1, t2) => { return t1.transform.GetSiblingIndex().CompareTo(t2.transform.GetSiblingIndex()); });

      doors = FindObjectsOfType<Door>().ToList();
      doors.Sort((d1, d2) => { return d1.transform.GetSiblingIndex().CompareTo(d2.transform.GetSiblingIndex()); });

      lightSwitches = FindObjectsOfType<LightSwitch>().ToList();
      lightSwitches.Sort((d1, d2) => { return d1.transform.GetSiblingIndex().CompareTo(d2.transform.GetSiblingIndex()); });
   }


   #region Objects Placement (private = Server Only; public = Client Only)

   public void PlaceTargetObjects() {
      int[] skins = new int[targetObjects.Count];
      int[] toSteal = new int[Consts.ITEM_TO_STEAL];

      // Choose skins
      List<int> allSkins = Enumerable.Range(0, 10).ToList();
      for (int i = 0; i < targetObjects.Count; i++) {
         int skin = Random.Range(0, allSkins.Count);
         targetObjects[i].Init(skin);
         skins[i] = skin;
         allSkins.RemoveAt(skin);
      }

      // Choose objects to steal
      List<int> targetNum = Enumerable.Range(0, targetObjects.Count).ToList();
      for (int i = 0; i < Consts.ITEM_TO_STEAL; i++) {
         int index = targetNum[Random.Range(0, targetNum.Count)];
         targetObjects[index].toSteal = true;
         toSteal[i] = index;
         targetNum.Remove(index);
      }

      PV.RPC("RPC_PlaceTargetObjects", RpcTarget.Others, skins, toSteal);
   }

   [PunRPC]
   public void RPC_PlaceTargetObjects(int[] skins, int[] toSteal) {
      // Choose skins
      for (int i=0;i<skins.Length;i++) {
         targetObjects[i].Init(skins[i]);
      }

      // Choose objects to steal
      foreach (int i in toSteal) {
         targetObjects[i].toSteal = true;
      }
   }

   public void StealObject(TargetObject target) {
      int id = target.transform.GetSiblingIndex();
      PV.RPC("RPC_StealObject", RpcTarget.AllViaServer, id);

   }

   [PunRPC]
   public void RPC_StealObject(int id) {
      targetObjects[id].SyncSteal();
   }

   //public void ReplaceAllObjects() {
   //   PV.RPC("RPC_ReplaceAllObjects", RpcTarget.All);
   //}

   //[PunRPC]
   //public void RPC_ReplaceAllObjects() {
   //   foreach(TargetObject target in targetObjects) {

   //   }
   //}

   public void PlaceKeys() {

   }

   #endregion

   #region Doors

   public void InteractDoor(Door door) {
      int id = door.transform.GetSiblingIndex();
      PV.RPC("RPC_InteractDoor", RpcTarget.AllViaServer, id);
   }

   [PunRPC]
   public void RPC_InteractDoor(int id) {
      doors[id].SyncInteract();
   }

   public void CloseAllDoors() {
      PV.RPC("RPC_CloseAllDoors", RpcTarget.All);
   }

   [PunRPC]
   public void RPC_CloseAllDoors() {
      foreach (Door door in doors) {
         if (door.IsOpen) {
            door.SyncInteract();
         }
      }
   }

   #endregion

   #region Light Switches

   public void InteractLightSwitch(LightSwitch light) {
      int id = light.transform.GetSiblingIndex();
      PV.RPC("RPC_InteractLightSwitch", RpcTarget.AllViaServer, id);
   }

   [PunRPC]
   public void RPC_InteractLightSwitch(int id) {
      lightSwitches[id].SyncLight();
   }

   public void CloseAllLightSwitches() {
      PV.RPC("RPC_CloseAllLightSwitches", RpcTarget.All);
   }

   [PunRPC]
   public void RPC_CloseAllLightSwitches() {
      foreach (LightSwitch light in lightSwitches) {
         if (light.isON) {
            light.SyncLight();
         }
      }
   }


   #endregion

}
