using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

   public static UIManager Instance;

   [Header("Main Menu UI")]
   public GameObject mainMenuUI;
   public GameObject loginScreen;
   public GameObject lobbyScreen;
   public GameObject roomScreen;
   [Header("Game UI")]
   public GameObject gameUI;
   public GameObject pauseScreen;

   void Start() {
      if(Instance == null) {
         Instance = this;
      }
   }

   #region Transictions

   public void ToMainMenu(int screen) {
      loginScreen.SetActive(screen == 0);
      lobbyScreen.SetActive(screen == 1);
      roomScreen.SetActive(screen == 2);

      if(HUD.Paused) {
         HUD.Instance.Pause();
      }

      gameUI.SetActive(false);
      mainMenuUI.SetActive(true);
   }

   public void ToGame(bool pause) {
      pauseScreen.SetActive(pause);

      mainMenuUI.SetActive(false);
      gameUI.SetActive(true);
   }

   public void ToGame(int screen) {
      //TODO rimpiazzare togame (pause) e gestire hud.pause come fatto in tomainmenu
   }

   #endregion

}
