using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour {

   public static HUD Instance;

   public TextMeshProUGUI roundInfoText;
   public TextMeshProUGUI roundTimeText;

   private float lastTextTime;

   private void Awake() {
      Instance = this;
   }

   public void UpdateRoundInfoText(string text) {
      roundInfoText.text = text;
      Invoke("UpdateRoundInfoText", 2f); //TODO da trasformare in fade out
   }


   public void UpdateRoundInfoText(float timeToStartRound) {
      int roundedTime = Mathf.RoundToInt(timeToStartRound);
      roundInfoText.text = roundedTime.ToString();
      roundInfoText.fontSize = 35 + Mathf.Lerp(0, 45, lastTextTime - timeToStartRound);
      if (lastTextTime != roundedTime) {
         lastTextTime = roundedTime;
      }
   }

   public void UpdateRoundTimeText(int timeToEndRound) {
      int minutesLeft = timeToEndRound / 60;
      int secondsLeft = timeToEndRound % 60;
      string minuteZero = (minutesLeft < 10) ? "0" : "";
      string secondZero = (secondsLeft < 10) ? "0" : "";
      roundTimeText.text = minuteZero + minutesLeft + ":" + secondZero + secondsLeft;
   }

   private void UpdateRoundInfoText() {
      roundInfoText.text = "";
   }

}
