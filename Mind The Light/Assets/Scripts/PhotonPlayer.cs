﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPlayer : MonoBehaviour {

   public PhotonView PV;
   //public GameObject myChar;
   public int actorID = -1;
   public int teamID = -1;

   private Player player;

   private void Awake() {
      PV = GetComponent<PhotonView>();

      if (PV.IsMine)
         GameManager.Instance.myPlayer = this;

   }

   private void Start() {
      if (PV.IsMine) {
         PV.RPC("RPC_LinkActor", RpcTarget.MasterClient); // Only the masterclient knows teams
      }
   }

   void Update() {
      if (!PV.IsMine) {
         return;
      }

      if (PhotonNetwork.IsMasterClient) {
         if (GameManager.Instance.timeToStartRound <= 0f && GameManager.Instance.roundReady) {
            PV.RPC("RPC_StartRound", RpcTarget.All);
         }
      
         if (GameManager.Instance.timeToEndRound <= 0f && GameManager.Instance.roundStarted) {
            PV.RPC("RPC_EndRound", RpcTarget.MasterClient, player.gameObject.tag == "Guard");
            PV.RPC("RPC_EndRound", RpcTarget.Others, player.gameObject.tag == "Spy");
         }
      }
   }

   [PunRPC]
   private void RPC_LinkActor() {
      GameManager.Instance.LinkActor(this);
   }

   [PunRPC]
   void RPC_LinkedActor(int actor, int team, int playersReady) {
      actorID = actor;
      teamID = team;


      if (HUD.Instance != null) {
         HUD.Instance.UpdatePlayersReadyText(playersReady);
      }

      player = PhotonView.Find(actor).GetComponent<Player>();
      player.actor.SetDefaults();

      // Set Camera Target
      if (PV.IsMine) {
         PlayerCamera pCamera = Camera.main.transform.parent.GetComponent<PlayerCamera>();
         player.SetCamera(pCamera);
         pCamera.target = player.transform;
      }
   }

   [PunRPC]
   void RPC_ReadyRound(int currentRound) {
      GameManager.Instance.currentRound = currentRound;

      player.actor.SetDefaults();

      Chat.Instance.Reset();
      UIManager.Instance.ToGame(GameScreen.Empty);
      // Set Default Values (for the current actor)
      //Debug.Log("SELECT ACTOR " + GameManager.Instance.myPlayer.teamID + "-" + teamID);
      HUD.Instance.SelectActor(GameManager.Instance.myPlayer.teamID);

      GameManager.Instance.ReadyRound();
   }

   [PunRPC]
   void RPC_StartRound() {
      GameManager.Instance.StartRound();
   }

   [PunRPC]
   public void RPC_RequestEndRound(bool isMaster) {
      PV.RPC("RPC_EndRound", RpcTarget.MasterClient, isMaster);
      PV.RPC("RPC_EndRound", RpcTarget.Others, !isMaster);
   }

   [PunRPC]
   void RPC_EndRound(bool win) {
      GameManager.Instance.EndRound(win);
      PhotonView.Find(actorID).TransferOwnership(PhotonNetwork.CurrentRoom.MasterClientId);
      GameManager.Instance.myPlayer.actorID = -1;
      GameManager.Instance.myPlayer.teamID = -1;
   }
}
