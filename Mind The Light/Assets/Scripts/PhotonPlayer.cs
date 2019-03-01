using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPlayer : MonoBehaviour {

   public PhotonView PV;
   //public GameObject myChar;
   public int teamID = -1;

   private void Awake() {
      PV = GetComponent<PhotonView>();

      GameManager.Instance.myPlayerPV = PV;

   }

   private void Start() {
      if (PV.IsMine) {
         PV.RPC("RPC_GetTeam", RpcTarget.MasterClient); // Only the masterclient knows teams
      }
   }

   void Update() {
      //if(myChar == null && teamID != -1) {
      //   if(PV.IsMine) {
      //      SpawnActor();

      //      GameManager.Instance.ReadyRound();
      //   }
      //}
      if(!GameManager.Instance.roundReady) {
         return;
      }

      if(GameManager.Instance.timeToStartRound <= 0f && !GameManager.Instance.roundStarted) {
         if(PhotonNetwork.IsMasterClient) {
            PV.RPC("RPC_StartRound", RpcTarget.All);
         }
      }

      if (GameManager.Instance.timeToEndRound <= 0f && GameManager.Instance.roundStarted) {
         if (PhotonNetwork.IsMasterClient) {
            PV.RPC("RPC_EndRound", RpcTarget.All);
         }
      }
   }

   [PunRPC]
   private void RPC_GetTeam() {
      GameManager.Instance.LinkActor(this);

      PV.RPC("RPC_SentTeam", RpcTarget.Others, teamID);
   }

   [PunRPC]
   void RPC_SentTeam(int team) {
      teamID = team;
   }

   [PunRPC]
   void RPC_ReadyRound() {
      UIManager.Instance.ToGame(false);
      GameManager.Instance.ReadyRound();
   }

   [PunRPC]
   void RPC_StartRound() {
      GameManager.Instance.StartRound();
   }

   [PunRPC]
   void RPC_EndRound() {
      GameManager.Instance.EndRound();
   }
}
