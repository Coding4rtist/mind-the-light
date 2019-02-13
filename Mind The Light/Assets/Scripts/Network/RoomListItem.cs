using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomListItem : MonoBehaviour {

   //public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);
   //private JoinRoomDelegate joinRoomCallback;

   private string roomName;

   [SerializeField]
   private Text roomNameText;

   //private MatchInfoSnapshot match;

   //public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallback) {
   //   match = _match;
   //   joinRoomCallback = _joinRoomCallback;

   //   roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
   //}

   public void Setup(string name, int roomSize) {
      roomName = name;

      roomNameText.text = roomName + " [" + roomSize + "]";
   }

   public void JoinRoomOnClick() {
      PhotonNetwork.JoinRoom(roomName);
   }
}
