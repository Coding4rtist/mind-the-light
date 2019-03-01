using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks, IInRoomCallbacks {

   public static NetworkManager Instance;

   #region Unity Functions

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

      // DEBUG
      //if(PhotonNetwork.IsMasterClient)
      //   AudioListener.pause = true;
   }

   private void Start() {
      PhotonNetwork.AutomaticallySyncScene = true;
      PhotonNetwork.ConnectUsingSettings();

      cachedRoomList = new Dictionary<string, RoomInfo>();
      roomListEntries = new Dictionary<string, GameObject>();

      SetRoomDefaults();
   }

   void Update() {
      if (isGameLoaded || isGameStarted) {
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

   public override void OnEnable() {
      base.OnEnable();
      PhotonNetwork.AddCallbackTarget(this);
   }

   public override void OnDisable() {
      base.OnDisable();
      PhotonNetwork.RemoveCallbackTarget(this);
   }

   #endregion

   #region Lobby Management

   [Header("Lobby Management")]
   private string nickName = "";
   private string roomName;
   public TextMeshProUGUI roomNameText;
   public GameObject roomListEntryPrefab;
   public Transform roomListContent;
   public TextMeshProUGUI lobbyStatusText;

   private Dictionary<string, RoomInfo> cachedRoomList;
   private Dictionary<string, GameObject> roomListEntries;

   public override void OnConnectedToMaster() {
      base.OnConnectedToMaster();

      Debug.Log("Player has connected to the Photon master server");

      if (nickName == "") {
         UIManager.Instance.ToMainMenu(0);
         return;
      }

      if (!PhotonNetwork.InLobby) {
         PhotonNetwork.JoinLobby();
      }
   }

   public override void OnRoomListUpdate(List<RoomInfo> roomList) {
      base.OnRoomListUpdate(roomList);

      lobbyStatusText.text = "<style=\"C1\">Loading...</style>";

      //Debug.Log("room list update");
      //foreach (RoomInfo room in roomList) {
      //   Debug.Log(room.Name + ": " + room.PlayerCount + "/" + room.MaxPlayers + "Removed: " + room.RemovedFromList);
      //}
      //Debug.Log("end room list update");
      ClearRoomListView();
      UpdateCachedRoomList(roomList);
      UpdateRoomListView();

      lobbyStatusText.text = "";
   }

   public override void OnJoinedLobby() {
      base.OnJoinedLobby();

      //Debug.Log("Joined lobby");

      UIManager.Instance.ToMainMenu(1);
   }

   public override void OnCreateRoomFailed(short returnCode, string message) {
      base.OnCreateRoomFailed(returnCode, message);
      Debug.Log("Tried to create a new room but failed, there must be already one room with the same name");

      lobbyStatusText.text = "<style=\"C2\">Room creation failed.</style>";
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

   // Delayed start
   private bool readyToCount; // MANUAL START GAME
   private bool readyToStart; // FULL ROOM
   public float startingTime;
   private float timeToStart;

   public TextMeshProUGUI roomStatusText;
   public Transform playersPanel;
   public GameObject playerListingPrefab;
   public GameObject startButton;

   public override void OnJoinedRoom() {
      base.OnJoinedRoom();
      Debug.Log("You are now in room");

      UIManager.Instance.ToMainMenu(2);
      SetRoomDefaults();
      startButton.SetActive(PhotonNetwork.IsMasterClient && Consts.GAME_SIZE > 2);

      ClearPlayerListings();
      ListPlayers();

      photonPlayers = PhotonNetwork.PlayerList;
      playersInRoom = photonPlayers.Length;

      Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + "/" + Consts.GAME_SIZE + ")");
      roomName = PhotonNetwork.CurrentRoom.Name;
      roomNameText.text = roomName;
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

   public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
      base.OnPlayerLeftRoom(otherPlayer);

      playersInRoom--;
      Debug.Log(otherPlayer.NickName + " has left the game");

      if (!isGameStarted) {
         PhotonNetwork.CurrentRoom.IsOpen = true;
         roomStatusText.text = "<style=\"C1\">" + otherPlayer.NickName + " has left the room [" + playersInRoom + "/" + Consts.GAME_SIZE + "]</style>";
         ClearPlayerListings();
         ListPlayers();

         startButton.SetActive(PhotonNetwork.IsMasterClient && Consts.GAME_SIZE > 2);

         if (playersInRoom == 1) {
            SetRoomDefaults();
         }
      }
      else {
         // TODO Show message in game

         if (playersInRoom == 1) {
            lobbyStatusText.text = "<style=\"C2\">Not enought players to continue the game.</style>";
            //Disconnect();
            PhotonNetwork.LeaveRoom();
         }
      }
   }

   public override void OnLeftRoom() {
      base.OnLeftRoom();

      //Debug.Log("Room Left " + isGameStarted + " " + PhotonNetwork.CountOfRooms);

      UIManager.Instance.ToMainMenu(1);
      if (isGameStarted) {
         cachedRoomList.Remove(roomName);
         ClearRoomListView();
         UpdateRoomListView();
      }
      SetRoomDefaults();
   }

   [PunRPC]
   private void RPC_StartCountdown() {
      SetStartingTimer(false);
   }

   private void StartGame() {
      PhotonNetwork.CurrentRoom.IsOpen = false;
      PhotonNetwork.CurrentRoom.IsVisible = false;
      isGameLoaded = true;

      if (PhotonNetwork.IsMasterClient) {
         GameManager.Instance.SpawnActors();
         return;
      }

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
      isGameStarted = true;
      PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "NetworkPlayer"), transform.position, Quaternion.identity, 0);
   }

   #endregion

   #region UI Callbacks

   public void OnLoginButtonClicked() {
      if (nickName == "") {
         nickName = "Player " + Random.Range(0, 1000);
         PhotonNetwork.NickName = nickName;
      } 
      else
         PhotonNetwork.NickName = nickName;

      if (!PhotonNetwork.InLobby) {
         PhotonNetwork.JoinLobby();
      }
   }

   public void OnCreateRoomButtonClicked() {
      if (roomName == "" || roomName == null) {
         Debug.Log("Can't create a new room, name field is empty");
         lobbyStatusText.text = "<style=\"C2\">Can't create a new room, name field is empty</style>";
         return;
      }
      Debug.Log("Trying to create a new room");
      RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)Consts.GAME_SIZE, CleanupCacheOnLeave = true };
      PhotonNetwork.CreateRoom(roomName, roomOps);

      lobbyStatusText.text = "<style=\"C1\">Creating a new rooom...</style>";
      roomNameText.text = roomName;
   }

   public void OnStartGameButtonClicked() {
      if (readyToCount || readyToStart)
         return;
      if (playersInRoom > 1) {
         PV.RPC("RPC_StartCountdown", RpcTarget.All);
      }
   }

   public void OnLeaveGameButtonClicked() {
      PhotonNetwork.LeaveRoom();
   }


   public void OnNicknameChanged(string name) {
      nickName = name;
      PhotonNetwork.NickName = name;
   }

   public void OnRoomNameChanged(string name) {
      roomName = name;
   }

   #endregion

   #region Utils Functions

   private void ClearRoomListView() {
      foreach (GameObject entry in roomListEntries.Values) {
         Destroy(entry.gameObject);
      }

      roomListEntries.Clear();
   }

   private void UpdateCachedRoomList(List<RoomInfo> roomList) {
      foreach (RoomInfo info in roomList) {
         // Remove room from cached room list if it got closed, became invisible or was marked as removed
         if (!info.IsOpen || !info.IsVisible || info.RemovedFromList) {
            if (cachedRoomList.ContainsKey(info.Name)) {
               cachedRoomList.Remove(info.Name);
            }
            continue;
         }

         // Update cached room info
         if (cachedRoomList.ContainsKey(info.Name)) {
            cachedRoomList[info.Name] = info;
         }
         // Add new room info to cache
         else {
            cachedRoomList.Add(info.Name, info);
         }
      }
   }

   private void UpdateRoomListView() {
      foreach (RoomInfo info in cachedRoomList.Values) {
         GameObject entry = Instantiate(roomListEntryPrefab, roomListContent);
         entry.GetComponent<RoomListItem>().SetValues(info.Name, info.PlayerCount, info.MaxPlayers);
         roomListEntries.Add(info.Name, entry);
      }
   }

   private void SetRoomDefaults() {
      isGameLoaded = false;
      isGameStarted = false;
      readyToCount = false;
      readyToStart = false;
   }

   private void SetStartingTimer(bool roomIsFull) {
      if (roomIsFull) {
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

   #endregion
}
