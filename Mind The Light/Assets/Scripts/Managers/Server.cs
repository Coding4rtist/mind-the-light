using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Server : MonoBehaviour {

   public static Server Instance;

   public Chat chat;

   private PhotonView PV;

   private void Awake() {
      if (Instance == null) {
         Instance = this;
      }

      PV = GetComponent<PhotonView>();
   }

   //public void OnPlayerEnteredRoom(Photon.Realtime.Player player) {
   //   Debug.Log("[Server] Player joined the room: " + player.NickName + "("+ player.UserId + ")");
   //   PV.RPC("RPC_OnPlayerEnteredRoom", RpcTarget.Others, player.NickName);
   //}

   //public void OnPlayerLeftRoom(Photon.Realtime.Player player) {
   //   Debug.Log("[Server] Player left the room: " + player.NickName + "(" + player.UserId + ")");
   //   PV.RPC("RPC_OnPlayerLeftdRoom", RpcTarget.Others, player.NickName);
   //}

   public void SendGameMsgToClients(int team, string msgType, string arg1, string arg2, string arg3) {
      if(PhotonNetwork.IsMasterClient) {
         if(team == -1) {
            Server.Instance.PV.RPC("RPC_MsgFromServer", RpcTarget.Others, new object[4] {
               msgType,
               arg1,
               arg2,
               arg3
            });
         }
         else {
            // Uguale ????
            Server.Instance.PV.RPC("RPC_MsgFromServer", RpcTarget.Others, new object[4] {
               msgType,
               arg1,
               arg2,
               arg3
            });
         }
      }
   }

   public void SendGameMsgToPlayer() {

   }

   public static void Death(string killer, string victim) {
      if(PhotonNetwork.IsMasterClient) {
         //Server.Instance.S_Death(killer, victim);
      }
      Server.Instance.chat.KillText(killer, victim);
   }

   public void DoReload(Player player) {

   }

   public void SendWelcome(Photon.Realtime.Player player) {

   }

   public static void sendChatMsg(string sender, string msg, bool teamOnly) {
      Server.Instance.chat.AddLine(-1, sender, msg, teamOnly);
   }

   //[PunRPC]
   //private void RPC_Welcome() {

   //}
}
