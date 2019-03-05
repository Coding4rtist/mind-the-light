using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPlayer : MonoBehaviour {

   public PhotonView PV;
   //public GameObject myChar;
   public int actorID = -1;
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
      if(!PV.IsMine) {
         return;
      }
      //if(myChar == null && teamID != -1) {
      //   if(PV.IsMine) {
      //      SpawnActor();

      //      GameManager.Instance.ReadyRound();
      //   }
      //}

      if (PhotonNetwork.IsMasterClient) {
         if (GameManager.Instance.timeToStartRound <= 0f && GameManager.Instance.roundReady) {
            GameManager.Instance.StartRound();
            PV.RPC("RPC_StartRound", RpcTarget.Others);
         }
      
         if (GameManager.Instance.timeToEndRound <= 0f && GameManager.Instance.roundStarted) {
            GameManager.Instance.EndRound();
            PV.RPC("RPC_EndRound", RpcTarget.Others);
         }
      }
   }

   [PunRPC]
   private void RPC_GetTeam() {
      GameManager.Instance.LinkActor(this);

      PV.RPC("RPC_SentTeam", RpcTarget.All, actorID, teamID);
   }

   [PunRPC]
   void RPC_SentTeam(int actor, int team) {
      actorID = actor;
      teamID = team;

      // Set Camera Target
      if(PV.IsMine) {
         Player player = PhotonView.Find(actor).GetComponent<Player>();
         PlayerCamera pCamera = Camera.main.transform.parent.GetComponent<PlayerCamera>();
         player.SetCamera(pCamera);
         pCamera.target = player.transform;

         UIManager.Instance.ToGame(false);
         // Set Default Values (for the current actor)
         HUD.Instance.SelectActor(teamID);
      }
   }

   [PunRPC]
   void RPC_ReadyRound() {
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
