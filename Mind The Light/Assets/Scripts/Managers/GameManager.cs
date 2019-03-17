using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GameManager : MonoBehaviour {

   public static GameManager Instance;

   [HideInInspector]
   public PhotonPlayer myPlayer;

   public int randomTeam;
   public int currentRound;
   public float timeToStartRound;
   public float timeToEndRound;
   public bool roundReady = false;
   public bool roundStarted = false;
   
   [Header("Spawned Actors")]
   public GameObject[] guardActors;
   public GameObject[] spyActors;
   [Header("Actor Spawn Points")]
   public Transform[] spawnPointsGuards;
   public Transform[] spawnPointsSpies;

   private int guardsLinked = 0;
   private int spiesLinked = 0;

   public int PlayersLinked {
      get { return guardsLinked + spiesLinked; }
   }

   #region Unity Functions

   private void Awake() {
      if (Instance == null) {
         Instance = this;
      }
   }

   private void Update() {
      if (roundReady) {
         timeToStartRound -= Time.deltaTime;
         if (timeToStartRound <= 0f)
            timeToStartRound = 0f;

         HUD.Instance.UpdateRoundInfoText(timeToStartRound);
      }

      if (roundStarted) {
         timeToEndRound -= Time.deltaTime;
         if (timeToEndRound <= 0f)
            timeToEndRound = 0f;

         HUD.Instance.UpdateRoundTimeText(timeToEndRound);
      }
   }

   #endregion

   #region Actor Management (Server only)

   public void SpawnActors() {
      //randomTeam = Random.Range(0, 2); //TODO uncomment
      currentRound = 0;
      guardsLinked = 0;
      spiesLinked = 0;
      HUD.Instance.Reset();

      guardActors = new GameObject[Consts.GAME_SIZE / 2];
      for (int i = 0; i < Consts.GAME_SIZE / 2; i++) {
         string prefabName = Path.Combine(Consts.PHOTON_FOLDER, Consts.CHARACTER_NAMES[0]);
         guardActors[i] = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity, 0);
      }

      spyActors = new GameObject[Consts.GAME_SIZE / 2];
      for (int i = 0; i < Consts.GAME_SIZE / 2; i++) {
         string prefabName = Path.Combine(Consts.PHOTON_FOLDER, Consts.CHARACTER_NAMES[1]);
         spyActors[i] = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity, 0);
      }
   }

   //public void DespawnActors() { //TODO Cancellare se inutile
   //   for(int i=0; i<guardActors.Length; i++) {
   //      Destroy(guardActors[i]);
   //   }
   //   guardActors = null;
   //}

   public void LinkActor(PhotonPlayer photonPlayer) {
      int index = PhotonNetwork.CurrentRoom.Players.Keys.ToList().IndexOf(photonPlayer.PV.OwnerActorNr);
      //Debug.Log("LinkActor: index " + index);

      int team = (randomTeam + currentRound + index) % 2;
      //Debug.Log("LinkActor: team " + team);

      Vector3 spawnPoint = Vector3.zero;
      if (team == 0) {
         // Guard
         int rand = Random.Range(0, spawnPointsGuards.Length);
         spawnPoint = spawnPointsGuards[rand].position;

         Player player = guardActors[guardsLinked].GetComponent<Player>();
         player.TeamID = team;
         player.actor.Teleport(spawnPoint);
         player.PV.TransferOwnership(photonPlayer.PV.Owner);

         photonPlayer.actorID = player.PV.ViewID;
         photonPlayer.teamID = randomTeam;


         guardsLinked++;
         photonPlayer.PV.RPC("RPC_LinkedActor", RpcTarget.All, player.PV.ViewID, team, PlayersLinked);
      }
      else {
         // Spy
         int rand = Random.Range(0, spawnPointsSpies.Length);
         spawnPoint = spawnPointsSpies[rand].position;

         Player player = spyActors[spiesLinked].GetComponent<Player>();
         player.TeamID = team;
         player.actor.Teleport(spawnPoint);
         player.PV.TransferOwnership(photonPlayer.PV.Owner);

         photonPlayer.actorID = player.PV.ViewID;
         photonPlayer.teamID = randomTeam;

         spiesLinked++;
         photonPlayer.PV.RPC("RPC_LinkedActor", RpcTarget.All, player.PV.ViewID, team, PlayersLinked);
      }

      //Debug.Log("PI:" + PlayersLinked);

      if (PlayersLinked == Consts.GAME_SIZE) {
         WorldManager.Instance.PlaceTargetObjects();
         WorldManager.Instance.CloseAllDoors();
         myPlayer.PV.RPC("RPC_ReadyRound", RpcTarget.All, currentRound);
      }
      //UpdateTeam();
   }

   #endregion

   #region Rounds

   public void ReadyRound() {
      timeToStartRound = Consts.TIMER_ROUND_START;
      roundReady = true;
      Debug.Log("Round Ready!");
   }

   public void StartRound() {
      timeToEndRound = Consts.TIMER_ROUND_GAME;
      currentRound++;

      roundReady = false;
      roundStarted = true;

      Debug.Log("Round Started!");
      //HUD.Instance.UpdateRoundInfoText("");
      HUD.Instance.UpdateRoundInfoText("Round Started!");
   }

   public void EndRound(bool win) {
      //UpdateTeam();
      roundStarted = false;
      guardsLinked = 0;
      spiesLinked = 0;
      Debug.Log("Round Ended!");

      if (currentRound < Consts.GAME_ROUNDS) {
         //HUD.Instance.UpdateRoundInfoText("Round Ended!");
         HUD.Instance.UpdateRoundScores(win, false);
         HUD.Instance.UpdatePlayersReadyText(0);
         UIManager.Instance.ToGame(GameScreen.Round);
      }
      else {
         HUD.Instance.UpdateRoundScores(win, true);
         UIManager.Instance.ToGame(GameScreen.End);
      }
   }

   public void ResetRounds() {
      roundReady = false;
      roundStarted = false;
   }

   #endregion

   #region UI Callbacks

   public void OnRoundReadyButtonClicked() {
      if(myPlayer.actorID != -1 || myPlayer.teamID != -1) {
         return;
      }
      myPlayer.PV.RPC("RPC_LinkActor", RpcTarget.MasterClient);
   }

   #endregion

}
