using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;


public class GameManager : MonoBehaviour {

   public static GameManager Instance;

   [HideInInspector]
   public PhotonView myPlayerPV;

   public int nextPlayerTeam;
   public int currentRound;
   public float timeToStartRound;
   public float timeToEndRound;
   public bool roundReady = false;
   public bool roundStarted = false;

   public GameObject[] allCharacters;
   public Transform[] spawnPointsGuards;
   public Transform[] spawnPointsSpies;

   private int playersInizialized = 0;

   private void Awake() {
      if (Instance == null) {
         Instance = this;
      }
   }

   void Start() {
      nextPlayerTeam = Random.Range(0, 2);
   }

   void Update() {
      if(roundReady) {
         timeToStartRound -= Time.deltaTime;

         HUD.Instance.UpdateRoundInfoText(timeToStartRound);
      }

      if(roundStarted) {
         timeToEndRound -= Time.deltaTime;

         HUD.Instance.UpdateRoundTimeText(timeToEndRound);
      }
   }


   #region Photon Player Functions (Server Only)

   public void SpawnActors() {
      playersInizialized = 0;
      allCharacters = new GameObject[Consts.GAME_SIZE];
      for(int i=0; i<Consts.GAME_SIZE; i++) {
         string prefabName = Path.Combine(Consts.PHOTON_FOLDER, Consts.CHARACTER_NAMES[i%2]);
         allCharacters[i] = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity, 0);
      }
   }

   public void DespawnActors() { //TODO Cancellare se inutile
      for(int i=0; i<allCharacters.Length; i++) {
         Destroy(allCharacters[i]);
      }
      allCharacters = null;
   }

   public void LinkActor(PhotonPlayer photonPlayer) {
      Vector3 spawnPoint = Vector3.zero;
      if (nextPlayerTeam == 0) {
         int rand = Random.Range(0, spawnPointsGuards.Length);
         spawnPoint = spawnPointsGuards[rand].position;
      }
      else {
         int rand = Random.Range(0, spawnPointsSpies.Length);
         spawnPoint = spawnPointsSpies[rand].position;
      }

      Debug.Log("PI:" + playersInizialized);
      Player player = allCharacters[playersInizialized].GetComponent<Player>();
      player.TeamID = nextPlayerTeam;
      player.PV.TransferOwnership(photonPlayer.PV.Owner);

      photonPlayer.actorID = player.PV.ViewID;
      photonPlayer.teamID = nextPlayerTeam;

      playersInizialized++;
      if(playersInizialized == Consts.GAME_SIZE) {
         myPlayerPV.RPC("RPC_ReadyRound", RpcTarget.All);
      }
      UpdateTeam();
   }

   #endregion

   private void UpdateTeam() {
      nextPlayerTeam = (nextPlayerTeam + 1) % 2; 
   }

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

   public void EndRound() {
      UpdateTeam();
      roundStarted = false;
      Debug.Log("Round Ended!");
      HUD.Instance.UpdateRoundInfoText("Round Ended!");

      if(currentRound < Consts.GAME_ROUNDS) {
         ReadyRound();
      }
      else {
         // TODO Game Over Screen
      }
   }

   public void ResetRounds() {
      roundReady = false;
      roundStarted = false;
   }

   #endregion
}
