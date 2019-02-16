using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonPlayer : MonoBehaviour {

   private PhotonView PV;
   public GameObject myChar;
   public int teamID = -1;

   void Awake() {
      PV = GetComponent<PhotonView>();

      if (PV.IsMine) {
         PV.RPC("RPC_GetTeam", RpcTarget.MasterClient); // Only the masterclient knows teams
         
      }
   }

   void Update() {
      if(myChar == null && teamID != -1) {
         if(PV.IsMine) {
            string prefabName = Path.Combine(Consts.PHOTON_FOLDER, Consts.CHARACTER_NAMES[teamID]);
            Vector3 spawnPoint = Vector3.zero;
            if(teamID == 0) {
               int rand = Random.Range(0, GameManager.Instance.spawnPointsGuards.Length);
               spawnPoint = GameManager.Instance.spawnPointsGuards[rand].position;
            }
            else {
               int rand = Random.Range(0, GameManager.Instance.spawnPointsSpies.Length);
               spawnPoint = GameManager.Instance.spawnPointsSpies[rand].position;
            }
            myChar = PhotonNetwork.Instantiate(prefabName, spawnPoint, Quaternion.identity, 0);
            myChar.transform.SetParent(transform);

            //Camera.main.transform.parent.GetComponent<PlayerCamera>().target = myChar.transform;
         }
      }
   }

   [PunRPC]
   private void RPC_GetTeam() {
      teamID = GameManager.Instance.nextPlayerTeam;
      GameManager.Instance.UpdateTeam();
      PV.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, teamID);
   }

   [PunRPC]
   void RPC_SentTeam(int team) {
      teamID = team;
   }
}
