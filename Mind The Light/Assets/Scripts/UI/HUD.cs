using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {

   public static HUD Instance;

   public static bool Paused = false;

   public TextMeshProUGUI localScoreText;
   public TextMeshProUGUI remoteScoreText;
   private int localScore = 0;
   private int remoteScore = 0;

   public TextMeshProUGUI roundInfoText;
   public TextMeshProUGUI roundTimeText;
   public TextMeshProUGUI playersReadyText;
   private float lastTextTime;

   [Header("Actor Hud")]
   public GameObject guardT;
   public GameObject spyT;

   [Header("Guard")]
   public GameObject[] ammo;
   //private TextMeshProUGUI objectiveGuardText;

   [Header("Thief")]
   public Image healthImage;
   public Image damageImage;
   private float damageHealthTimer;
   private float lastDamageHealthAmount;
   public TextMeshProUGUI weightText;
   public TextMeshProUGUI objectiveSpyText;

   private void Awake() {
      Instance = this;
   }

   private void Update() {
      if(damageImage.fillAmount > healthImage.fillAmount) {
         damageHealthTimer -= Time.deltaTime;
         if(damageHealthTimer <= 0f) {
            damageImage.fillAmount -= Time.deltaTime * 0.7f;
         }
      } 
   }

   public void SelectActor(int actorID) {
      guardT.SetActive(actorID == 0);
      spyT.SetActive(actorID == 1);

      if(actorID == 1) {
         lastDamageHealthAmount = 1;
         healthImage.fillAmount = 1;
         damageImage.fillAmount = 1;
         weightText.text = "0";
      }
   }

   public void Pause() {
      Paused = !Paused;
      UIManager.Instance.ToGame(Paused ? GameScreen.Pause : GameScreen.Empty);
   }

   #region Round

   public void Reset() {
      localScore = 0;
      remoteScore = 0;

      localScoreText.text = localScore.ToString();
      remoteScoreText.text = remoteScore.ToString();
   }

   public void UpdatePlayersReadyText(int playersReady) {
      playersReadyText.text = "Players ready [" + playersReady + "/" + Consts.GAME_SIZE + "]";
   }

   public void UpdateRoundInfoText(string text, bool disappear = true) {
      roundInfoText.text = text;
      if(disappear)
      Invoke("UpdateRoundInfoText", 2f); //TODO da trasformare in fade out
   }


   public void UpdateRoundInfoText(float timeToStartRound) {
      int roundedTime = Mathf.FloorToInt(timeToStartRound);
      roundInfoText.text = roundedTime.ToString();
      roundInfoText.fontSize = 35 + Mathf.Lerp(0, 45, lastTextTime - timeToStartRound);
      if (lastTextTime != roundedTime) {
         lastTextTime = roundedTime;
      }
   }

   public void UpdateRoundTimeText(float timeToEndRound) {
      int time = Mathf.FloorToInt(timeToEndRound);
      int minutesLeft = time / 60;
      int secondsLeft = time % 60;
      string minuteZero = (minutesLeft < 10) ? "0" : "";
      string secondZero = (secondsLeft < 10) ? "0" : "";
      roundTimeText.text = minuteZero + minutesLeft + ":" + secondZero + secondsLeft;
   }

   private void UpdateRoundInfoText() {
      roundInfoText.text = "";
   }

   public void UpdateRoundScores(bool win, bool gameEnd) {
      if (win) {
         localScore++;
         UpdateRoundInfoText("You win the round", false);
      } 
      else {
         remoteScore++;
         UpdateRoundInfoText("You lose the round", false);
      }

      if(gameEnd) {
         UpdateRoundInfoText("You " + ((localScore > remoteScore) ? "win" : "lose")  + " the game", false);
      }

      localScoreText.text = localScore.ToString();
      remoteScoreText.text = remoteScore.ToString();
   }

   #endregion

   #region Guard

   public void UpdateMagazine(int currentAmmo) {
      for (int i = 0; i < 9; i++) {
         ammo[i].SetActive(i < currentAmmo);
      }
   }

   #endregion

   #region Spy

   public void UpdateHealthBar(float fillAmount) {
      healthImage.fillAmount = fillAmount;

      damageHealthTimer = 1f;
      lastDamageHealthAmount = fillAmount;
   }

   public void UpdateObjectsStolen(int curObjects) {
      objectiveSpyText.text = "Objects to steal: " + (Consts.ITEM_TO_STEAL - curObjects);
      weightText.text = (curObjects * 5).ToString();
   }

   #endregion
}
