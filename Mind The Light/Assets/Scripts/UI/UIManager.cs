using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

   public static UIManager Instance;

   public GameObject mainMenuScreen;
   public GameObject roomScreen;
   public GameObject gameScreen;
   public GameObject pauseScreen;

   void Start() {
      if(Instance == null) {
         Instance = this;
      }
   }

   #region Transictions

   public void ToMainMenu() {
      roomScreen.SetActive(false);
      gameScreen.SetActive(false);
      pauseScreen.SetActive(false);
      mainMenuScreen.SetActive(true);
   }

   public void ToRoomScreen() {
      mainMenuScreen.SetActive(false);
      gameScreen.SetActive(false);
      pauseScreen.SetActive(false);
      roomScreen.SetActive(true);
   }

   public void ToPause() {
      mainMenuScreen.SetActive(false);
      roomScreen.SetActive(false);
      gameScreen.SetActive(false);
      pauseScreen.SetActive(true);
   }

   public void ToGame() {
      mainMenuScreen.SetActive(false);
      roomScreen.SetActive(false);
      pauseScreen.SetActive(true);
      gameScreen.SetActive(true);
   }

   #endregion

}
