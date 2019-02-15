using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonLobby : MonoBehaviourPunCallbacks, ILobbyCallbacks {

   public static PhotonLobby lobby;
   public string roomName;
   public GameObject roomListingPrefab;
   public Transform roomsPanel;
   public TextMeshProUGUI statusText;
   public TextMeshProUGUI roomNameText;

   public List<RoomInfo> roomListings;

   private void Awake () {
      lobby = this;
   }

   private void Start() {
      PhotonNetwork.ConnectUsingSettings();
      roomListings = new List<RoomInfo>();
   }

   public override void OnConnectedToMaster() {
      base.OnConnectedToMaster();
      Debug.Log("Player has connected to the Photon master server");
      PhotonNetwork.AutomaticallySyncScene = true;
      PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);

      if (!PhotonNetwork.InLobby) {
         PhotonNetwork.JoinLobby();
      }
   }

   public override void OnRoomListUpdate(List<RoomInfo> roomList) {
      base.OnRoomListUpdate(roomList);

      statusText.text = "<style=\"C1\">Loading...</style>";

      //RemoveRoomListings();
      int tempIndex;
      foreach(RoomInfo room in roomList) {
         if(roomListings != null) {
            tempIndex = roomListings.FindIndex(ByName(room.Name));
         }
         else {
            tempIndex = -1;
         }
         if(tempIndex != -1) {
            roomListings.RemoveAt(tempIndex);
            Destroy(roomsPanel.GetChild(tempIndex).gameObject);
         }
         else {
            if(room.IsOpen && room.IsVisible) {
               roomListings.Add(room);
               ListRoom(room);
            }
         }
      }

      statusText.text = "";
   }

   static System.Predicate<RoomInfo> ByName(string name) {
      return delegate (RoomInfo room) {
         return room.Name == name;
      };
   }

   private void RemoveRoomListings() {
      int i = 0; 
      while(roomsPanel.childCount != 0) {
         Destroy(roomsPanel.GetChild(i).gameObject);
         i++;
      }
   }

   private void ListRoom(RoomInfo room) {
      if(room.IsOpen && room.IsVisible) {
         GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
         RoomListItem roomListItem = tempListing.GetComponent<RoomListItem>();
         roomListItem.Setup(room.Name, room.MaxPlayers);
      }
   }

   public void CreateRoom() {
      if(roomName == "" || roomName == null) {
         Debug.Log("Can't create a new room, name field is empty");
         statusText.text = "<style=\"C2\">Can't create a new room, name field is empty</style>";
         return;
      }
      Debug.Log("Trying to create a new room");
      RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)Consts.GAME_SIZE };
      PhotonNetwork.CreateRoom(roomName, roomOps);

      statusText.text = "<style=\"C1\">Creating a new rooom...</style>";
      roomNameText.text = roomName;
   }

   public override void OnCreateRoomFailed(short returnCode, string message) {
      base.OnCreateRoomFailed(returnCode, message);
      Debug.Log("Tried to create a new room but failed, there must be already one room with the same name");

      statusText.text = "<style=\"C2\">Room creation failed.</style>";
   }

   public void OnNicknameChanged(string name) {
      PhotonNetwork.NickName = name;
   }

   public void OnRoomNameChanged(string name) {
      roomName = name;
   }

   public void JoinLobbyOnClick() {
      statusText.text = "";
      if(!PhotonNetwork.InLobby) {
         PhotonNetwork.JoinLobby();
      }
   }


   public void Disconnect() {
      PhotonNetwork.LeaveRoom();
      UIManager.Instance.ToMainMenu();
      //RemoveRoomListings();
      JoinLobbyOnClick();
   }
}
