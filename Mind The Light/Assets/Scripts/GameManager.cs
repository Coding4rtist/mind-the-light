using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

   public static GameManager Instance;

   public Transform[] spawnPoints;

   private void Awake() {
      if (Instance == null) {
         Instance = this;
      }
   }

   void Start() {
      


   }

   void Update() {

   }
}
