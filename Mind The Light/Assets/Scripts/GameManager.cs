using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

   public static GameManager Instance;

   public int nextPlayerTeam;

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
}
