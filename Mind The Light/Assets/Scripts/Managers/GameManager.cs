using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

   public static GameManager Instance;

   public int nextPlayerTeam;
   public int currentRound;
   private int startRoundCountdown;
   public bool roundReady = false;
   public bool roundStarted = false;

   //public GameObject[] allCharacters;
   public Transform[] spawnPointsGuards;
   public Transform[] spawnPointsSpies;

   private void Awake() {
      if (Instance == null) {
         Instance = this;
      }
   }

   void Start() {


   }

   void Update() {

   }

   public void UpdateTeam() {
      nextPlayerTeam = (nextPlayerTeam + 1) % 2; 
   }

   #region Rounds

   public void ReadyRound() {
      startRoundCountdown = 5;
      roundReady = true;
   }

   public void StartRound() {
      roundStarted = true;
   }

   public void EndRound() {
      roundStarted = false;
   }

   #endregion
}
