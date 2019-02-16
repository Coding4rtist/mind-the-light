using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;

public class NetworkManager : MonoBehaviourPunCallbacks, IInRoomCallbacks {

   public static NetworkManager Instance;

   private void Awake() {
      if (Instance == null) {
         Instance = this;
      }
      else {
         if (Instance != this) {
            Destroy(Instance.gameObject);
            Instance = this;
         }
      }

      PV = GetComponent<PhotonView>();

      PhotonNetwork.OfflineMode = Consts.OfflineMode;
   }

   private void Start() {
      PhotonNetwork.ConnectUsingSettings();
      roomListingItems = new List<RoomListItem>();

      SetRoomDefaults();
   }

   public override void OnEnable() {
      base.OnEnable();
      PhotonNetwork.AddCallbackTarget(this);
   }

   public override void OnDisable() {
      base.OnDisable();
      PhotonNetwork.RemoveCallbackTarget(this);
   }

   #region Lobby Management

   [Header("Lobby Management")]
   private string nickName = "";
   private string roomName;
   public TextMeshProUGUI roomNameText;
   public GameObject roomListingPrefab;
   public Transform roomsPanel;
   public TextMeshProUGUI lobbyStatusText;

   public List<RoomListItem> roomListingItems;

   public override void OnConnectedToMaster() {
      base.OnConnectedToMaster();
      Debug.Log("Player has connected to the Photon master server");
      PhotonNetwork.AutomaticallySyncScene = true;
      if (nickName == "")
         PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
      else
         PhotonNetwork.NickName = nickName;

      if (!PhotonNetwork.InLobby) {
         PhotonNetwork.JoinLobby();
      }
   }

   public override void OnRoomListUpdate(List<RoomInfo> roomList) {
      base.OnRoomListUpdate(roomList);

      lobbyStatusText.text = "<style=\"C1\">Loading...</style>";


      foreach(RoomInfo room in roomList) {
         RoomReceived(room);
      }

      lobbyStatusText.text = "";
   }

   private void RoomReceived(RoomInfo room) {
      int index = roomListingItems.FindIndex(x => x.roomName == room.Name);
      //Debug.Log(room.Name + ": " + room.PlayerCount + "/" + room.MaxPlayers + " I: " + index + " " + room.RemovedFromList);
      if (index == -1) {
         // New Room
         if(room.IsVisible && room.IsOpen /*&& room.PlayerCount < room.MaxPlayers*/ && room.MaxPlayers != 0) {
            GameObject roomListGO = Instantiate(roomListingPrefab, roomsPanel);
            RoomListItem roomListItem = roomListGO.GetComponent<RoomListItem>();
            roomListItem.SetValues(room.Name, room.PlayerCount, room.MaxPlayers);
            roomListingItems.Add(roomListItem);
         }
      }
      else {
         // Room Update
         if(room.RemovedFromList || !room.IsVisible || !room.IsOpen || room.MaxPlayers == 0) {
            roomListingItems.RemoveAt(index);
            Destroy(roomsPanel.GetChild(index).gameObject);
         }
         else {
            //if (&& room.PlayerCount < room.MaxPlayers) 
            roomListingItems[index].SetValues(room.Name, room.PlayerCount, room.MaxPlayers);
         }
      }
   }

   private void RemoveRoomListings() {
      int i = roomsPanel.childCount;
      while (roomsPanel.childCount != 0) {
         Destroy(roomsPanel.GetChild(i).gameObject);
         i--;
      }
      roomListingItems.Clear();
   }

   public void CreateRoom() {
      if (roomName == "" || roomName == null) {
         Debug.Log("Can't create a new room, name field is empty");
         lobbyStatusText.text = "<style=\"C2\">Can't create a new room, name field is empty</style>";
         return;
      }
      Debug.Log("Trying to create a new room");
      RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)Consts.GAME_SIZE };
      PhotonNetwork.CreateRoom(roomName, roomOps);

      lobbyStatusText.text = "<style=\"C1\">Creating a new rooom...</style>";
      roomNameText.text = roomName;
   }

   public override void OnCreateRoomFailed(short returnCode, string message) {
      base.OnCreateRoomFailed(returnCode, message);
      Debug.Log("Tried to create a new room but failed, there must be already one room with the same name");

      lobbyStatusText.text = "<style=\"C2\">Room creation failed.</style>";
   }

   public void OnNicknameChanged(string name) {
      nickName = name;
      PhotonNetwork.NickName = name;
   }

   public void OnRoomNameChanged(string name) {
      roomName = name;
   }

   public void JoinLobbyOnClick() {
      lobbyStatusText.text = "";
      if (!PhotonNetwork.InLobby) {
         PhotonNetwork.JoinLobby();
      }
   }

   #endregion

   #region Room Management

   [Header("Room Management")]
   public bool isGameLoaded;
   public bool isGameStarted;
   private PhotonView PV;

   // Player info
   private Photon.Realtime.Player[] photonPlayers;
   public int playersInRoom;
   //public int playersInGame;

   // Delayed start
   private bool readyToCount; // MANUAL START GAME
   private bool readyToStart; // FULL ROOM
   public float startingTime;
   private float timeToStart;

   public TextMeshProUGUI roomStatusText;
   public Transform playersPanel;
   public GameObject playerListingPrefab;
   public GameObject startButton;

   void Update() {
      if(isGameLoaded || isGameStarted) {
         return;
      }

      if (playersInRoom <= 1) {
         return;
      }
      if (readyToCount || readyToStart) {
         timeToStart -= Time.deltaTime;

         if (timeToStart != startingTime) {
            //Debug.Log("Display time to start to the players " + timeToStart);
            roomStatusText.text = "<style=\"C1\">Time to start: " + Mathf.RoundToInt(timeToStart) + "</style>";
         }
         if (timeToStart <= 0) {
            StartGame();
         }
      }
      
   }

   public override void OnJoinedRoom() {
      base.OnJoinedRoom();
      Debug.Log("You are now in room");

      UIManager.Instance.ToRoomScreen();
      SetRoomDefaults();
      startButton.SetActive(PhotonNetwork.IsMasterClient);

      ClearPlayerListings();
      ListPlayers();

      photonPlayers = PhotonNetwork.PlayerList;
      playersInRoom = photonPlayers.Length;

      Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + "/" + Consts.GAME_SIZE + ")");
      roomStatusText.text = "<style=\"C1\">You joined the room [" + playersInRoom + "/" + Consts.GAME_SIZE + "]</style>";

      if (playersInRoom == Consts.GAME_SIZE) {
         SetStartingTimer(true);

         if (!PhotonNetwork.IsMasterClient)
            return;
         PhotonNetwork.CurrentRoom.IsOpen = false;
      }
   }

   public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
      base.OnPlayerEnteredRoom(newPlayer);
      Debug.Log("A new player has joined the room");

      ClearPlayerListings();
      ListPlayers();

      photonPlayers = PhotonNetwork.PlayerList;
      playersInRoom++;

      Debug.Log("Displayer player in room out of max players possible (" + playersInRoom + "/" + Consts.GAME_SIZE + ")");
      roomStatusText.text = "<style=\"C1\">A new player has joined the room [" + playersInRoom + "/" + Consts.GAME_SIZE + "]</style>";

      if (playersInRoom == Consts.GAME_SIZE) {
         SetStartingTimer(true);

         if (!PhotonNetwork.IsMasterClient)
            return;
         PhotonNetwork.CurrentRoom.IsOpen = false;
      }
      

   }

   private void SetRoomDefaults() {
      isGameLoaded = false;
      isGameStarted = false;
      readyToCount = false;
      readyToStart = false;
   }

   private void SetStartingTimer(bool roomIsFull) {
      if(roomIsFull) {
         startingTime = Consts.TIMER_FULL_ROOM;
         readyToStart = true;
      }
      else {
         startingTime = Consts.TIMER_NOT_FULL_ROOM;
         readyToCount = true;
      }
      timeToStart = startingTime;
   }

   void ClearPlayerListings() {
      for (int i = playersPanel.childCount - 1; i >= 0; i--) {
         Destroy(playersPanel.GetChild(i).gameObject);
      }
   }

   void ListPlayers() {
      if (PhotonNetwork.InRoom) {
         foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            GameObject tempListing = Instantiate(playerListingPrefab, playersPanel);
            TextMeshProUGUI tempText = tempListing.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tempText.text = player.NickName;
         }
      }
   }


   public void StartGameMaster() {
      if (readyToCount || readyToStart)
         return;
      if (playersInRoom > 1) {
         SetStartingTimer(false);
         readyToCount = true;
         PV.RPC("RPC_StartCountdown", RpcTarget.Others);
      }
   }

   [PunRPC]
   private void RPC_StartCountdown() {
      SetStartingTimer(false);
   }

   private void StartGame() {
      if (PhotonNetwork.IsMasterClient) {
         return;
      }

      isGameLoaded = true;

      PhotonNetwork.CurrentRoom.IsOpen = false;
      PhotonNetwork.CurrentRoom.IsVisible = false;

      PV.RPC("RPC_LoadedGame", RpcTarget.MasterClient); 
   }

   [PunRPC]
   private void RPC_LoadedGame() {
      //playersInGame++;
      //if(playersInGame == PhotonNetwork.PlayerList.Length) {
      if(!isGameStarted) {
         isGameStarted = true;
         PV.RPC("RPC_CreatePlayer", RpcTarget.All);
      }
      
      //}
   }

   [PunRPC]
   private void RPC_CreatePlayer() {
      UIManager.Instance.ToGame();
      isGameStarted = true;
      PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "NetworkPlayer"), transform.position, Quaternion.identity, 0);
   }

   public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
      base.OnPlayerLeftRoom(otherPlayer);

      playersInRoom--;
      Debug.Log(otherPlayer.NickName + " has left the game");

      if(!isGameStarted) {
         roomStatusText.text = "<style=\"C1\">" + otherPlayer.NickName + " has left the room [" + playersInRoom + "/" + Consts.GAME_SIZE + "]</style>";
         ClearPlayerListings();
         ListPlayers();

         startButton.SetActive(PhotonNetwork.IsMasterClient);

         if(playersInRoom == 1) {
            SetRoomDefaults();
         }
      }
      else {
         // TODO Show message in game

         if (playersInRoom == 1) {
            lobbyStatusText.text = "<style=\"C2\">Not enought players to continue the game.</style>";
            Disconnect();
         }
      }
   }

   public void LeaveRoom() {
      PhotonNetwork.LeaveRoom();
   }

   public void Disconnect() {
      PhotonNetwork.LeaveRoom();
      SetRoomDefaults();
      UIManager.Instance.ToMainMenu();
      PhotonNetwork.Disconnect();
      PhotonNetwork.ConnectUsingSettings();
      JoinLobbyOnClick();
   }

   public override void OnLeftRoom() {
      base.OnLeftRoom();

      //RemoveRoomListings();
      SetRoomDefaults();
      UIManager.Instance.ToMainMenu();
   }

   #endregion
}
