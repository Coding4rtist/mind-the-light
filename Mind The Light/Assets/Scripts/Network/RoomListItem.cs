using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class RoomListItem : MonoBehaviour {

   public string roomName;
   public bool updated;

   public TextMeshProUGUI roomNameText;

   public void SetValues(string name, int playerCount, int roomSize) {
      roomName = name;
      roomNameText.text = roomName + " [" + playerCount + "/" + roomSize + "]";
      updated = true;
   }

   public void JoinRoomOnClick() {
      PhotonNetwork.JoinRoom(roomName);
   }
}
