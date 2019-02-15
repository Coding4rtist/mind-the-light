using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using TMPro;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks {

   // Room info
   public static PhotonRoom room;
   private PhotonView PV;

   public bool isGameLoaded;

   // Player info
   private Photon.Realtime.Player[] photonPlayers;
   public int playersInRoom;
   //public int playersInGame;

   // Delayed start
   private bool readyToCount;
   private bool readyToStart;
   public float startingTime;
   public float lessThanMaxPlayers;
   private float atMaxPlayers;
   private float timeToStart;

   public TextMeshProUGUI statusText;
   public Transform playersPanel;
   public GameObject playerListingPrefab;
   public GameObject startButton;

   private void Awake() {
      if(room == null) {
         room = this;
      }
      else {
         if(room != this) {
            Destroy(room.gameObject);
            room = this;
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

   private void Start() {
      PV = GetComponent<PhotonView>();
      readyToCount = false;
      readyToStart = false;
      lessThanMaxPlayers = startingTime;
      atMaxPlayers = 6;
      timeToStart = startingTime;
   }

   void Update() {
      if(Consts.DELAYED_START) {
         if(playersInRoom == 1) {
            RestartTimer();
         }
         if(!isGameLoaded) {
            if(readyToStart) {
               atMaxPlayers -= Time.deltaTime;
               lessThanMaxPlayers = atMaxPlayers;
               timeToStart = atMaxPlayers;
            }
            else if(readyToCount) {
               lessThanMaxPlayers -= Time.deltaTime;
               timeToStart = lessThanMaxPlayers;
            }
            
            if (timeToStart != startingTime) {
               //Debug.Log("Display time to start to the players " + timeToStart);
               statusText.text = "<style=\"C1\">Time to start: " + Mathf.RoundToInt(timeToStart) + "</style>";
            }
            if(timeToStart <= 0) {
               StartGame();
            }
         }
      }
   }

   public override void OnJoinedRoom() {
      base.OnJoinedRoom();
      Debug.Log("You are now in room");

      UIManager.Instance.ToRoomScreen();
      if(PhotonNetwork.IsMasterClient) {
         startButton.SetActive(true);
      }
      ClearPlayerListings();
      ListPlayers();

      photonPlayers = PhotonNetwork.PlayerList;
      playersInRoom = photonPlayers.Length;

      if(Consts.DELAYED_START) {
         Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + "/" + Consts.GAME_SIZE + ")");
         statusText.text = "<style=\"C1\">You joined the room [" + playersInRoom + "/" + Consts.GAME_SIZE + "]</style>";
         if (playersInRoom > 1) {
            readyToCount = true;
         }
         if(playersInRoom == Consts.GAME_SIZE) {
            readyToStart = true;
            if (!PhotonNetwork.IsMasterClient)
               return;
            PhotonNetwork.CurrentRoom.IsOpen = false;
         }
      }
      //else {
      //   StartGame();
      //}
   }

   void ClearPlayerListings() {
      for (int i = playersPanel.childCount - 1; i >= 0; i--) {
         Destroy(playersPanel.GetChild(i).gameObject);
      }
   }

   void ListPlayers() {
      if(PhotonNetwork.InRoom) {
         foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            GameObject tempListing = Instantiate(playerListingPrefab, playersPanel);
            TextMeshProUGUI tempText = tempListing.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tempText.text = player.NickName;
         }
      }
   }

   public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
      base.OnPlayerEnteredRoom(newPlayer);
      Debug.Log("A new player has joined the room");

      ClearPlayerListings();
      ListPlayers();

      photonPlayers = PhotonNetwork.PlayerList;
      playersInRoom++;
      if(Consts.DELAYED_START) {
         Debug.Log("Displayer player in room out of max players possible (" + playersInRoom + "/" + Consts.GAME_SIZE + ")");
         statusText.text = "<style=\"C1\">A new player has joined the room [" + playersInRoom + "/" + Consts.GAME_SIZE + "]</style>";
         if (playersInRoom > 1) {
            readyToCount = true;
         }
         if(playersInRoom == Consts.GAME_SIZE) {
            readyToStart = true;
            if (!PhotonNetwork.IsMasterClient)
               return;
            PhotonNetwork.CurrentRoom.IsOpen = false;
         }
      }
       
   }

   public void StartGame() {
      isGameLoaded = true;
      if (PhotonNetwork.IsMasterClient)
         return;

      if(Consts.DELAYED_START) {
         PhotonNetwork.CurrentRoom.IsOpen = false;
      }

      // START GAME
      if (Consts.DELAYED_START) {
         PV.RPC("RPC_LoadedGame", RpcTarget.MasterClient);
      }
      else {
         RPC_CreatePlayer();
      }
   }

   [PunRPC]
   private void RPC_LoadedGame() {
      //playersInGame++;
      //if(playersInGame == PhotonNetwork.PlayerList.Length) {
         PV.RPC("RPC_CreatePlayer", RpcTarget.All);
      //}
   }

   [PunRPC]
   private void RPC_CreatePlayer() {
      UIManager.Instance.ToGame();
      PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "NetworkPlayer"), transform.position, Quaternion.identity, 0);
   }

   private void RestartTimer() {
      lessThanMaxPlayers = startingTime;
      timeToStart = startingTime;
      atMaxPlayers = 6;
      readyToCount = false;
      readyToStart = false;
   }

   public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
      base.OnPlayerLeftRoom(otherPlayer);
      playersInRoom--;
      Debug.Log(otherPlayer.NickName + " has left the game");
      statusText.text = "<style=\"C1\">"+ otherPlayer.NickName + "has left the room [" + playersInRoom + "/" + Consts.GAME_SIZE + "]</style>";
      ClearPlayerListings();
      ListPlayers();
   }
}
