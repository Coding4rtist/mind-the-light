using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoom : MonoBehaviour {

   [SerializeField]
   private Text _roomName;
   public Text RoomName {
      get { return _roomName; }
   }


   void Start() {

   }


   void Update() {

   }

   public void OnClick_CreateRoom() {
      if(PhotonNetwork.CreateRoom(RoomName.text)) {
         print("Create room successfully sent.");
      }
      else {
         print("Create room failed to send.");
      }
   }

}
