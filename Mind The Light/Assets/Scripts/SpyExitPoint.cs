using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpyExitPoint : MonoBehaviour {


   private void OnTriggerEnter2D(Collider2D other) {
      if(other.tag == "Spy") {
         Spy spy = other.GetComponent<Spy>();
         if(spy.p.PV.IsMine && spy.ObjectsStolen == Consts.ITEM_TO_STEAL) {
            GameManager.Instance.myPlayer.PV.RPC("RPC_RequestEndRound", RpcTarget.MasterClient, PhotonNetwork.IsMasterClient);
         }
      }
   }
}
