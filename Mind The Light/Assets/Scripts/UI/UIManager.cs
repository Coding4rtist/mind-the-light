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
   public GameObject roundScreen;
   public GameObject endScreen;

   void Start() {
      if(Instance == null) {
         Instance = this;
      }
   }

   #region Transictions

   public void ToMainMenu(MainMenuScreen screen) {
      loginScreen.SetActive(screen == MainMenuScreen.Login);
      lobbyScreen.SetActive(screen == MainMenuScreen.Lobby);
      roomScreen.SetActive(screen == MainMenuScreen.Room);

      if(HUD.Paused) {
         HUD.Instance.Pause();
      }

      gameUI.SetActive(false);
      mainMenuUI.SetActive(true);
   }

   public void ToGame(GameScreen screen) {
      pauseScreen.SetActive(screen == GameScreen.Pause);
      roundScreen.SetActive(screen == GameScreen.Round);
      endScreen.SetActive(screen == GameScreen.End);

      if (screen != GameScreen.Pause && HUD.Paused) {
         HUD.Instance.Pause();
      }

      mainMenuUI.SetActive(false);
      gameUI.SetActive(true);
   }

   #endregion
}

public enum MainMenuScreen {
   Login = 0,
   Lobby = 1,
   Room = 2
}

public enum GameScreen {
   Empty = 0,
   Pause = 1,
   Round = 2,
   End = 3
}
