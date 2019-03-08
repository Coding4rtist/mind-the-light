using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {

   public static HUD Instance;

   public static bool Paused = false;

   public TextMeshProUGUI roundInfoText;
   public TextMeshProUGUI roundTimeText;
   private float lastTextTime;

   [Header("Actor Hud")]
   public GameObject guardT;
   public GameObject spyT;

   [Header("HealthBar")]
   public Image healthImage;
   public Image damageImage;
   private float damageHealthTimer;
   private float lastDamageHealthAmount;

   [Header("Magazine")]
   public GameObject[] ammo;

   public TextMeshProUGUI playersReadyText;


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
      }
   }

   public void Pause() {
      Paused = !Paused;
      UIManager.Instance.ToGame(Paused ? GameScreen.Pause : GameScreen.Empty);
   }

   public void UpdateRoundInfoText(string text) {
      roundInfoText.text = text;
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

   public void UpdateHealthBar(float fillAmount) {
      healthImage.fillAmount = fillAmount;

      damageHealthTimer = 1f;
      lastDamageHealthAmount = fillAmount;
   }

   public void UpdateMagazine(int currentAmmo) {
      for(int i=0; i<9; i++) {
         ammo[i].SetActive(i < currentAmmo);
      }
   }
}
