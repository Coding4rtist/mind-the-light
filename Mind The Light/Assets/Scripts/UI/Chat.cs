using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviour {

   public GameObject chatMessagePrefab;
   public Transform chatTransform;

   public string chatMsg;
   public List<string> chatLines = new List<string>();
   public AudioClip chatSound;

   private ScrollRect scroll;

   private void Awake() {
      scroll = GetComponent<ScrollRect>();
   }

   public void Reset() {
      chatLines.Clear();
   }

   public void KillText(string killer, string victim) {
      string color1 = "#2663a4";
      string color2 = "#cc3431";
      AddLine(-1, "", string.Format("<color={0}>{1}</color> killed <color={2}>{3}</color>", color1, killer, color2, victim), false);
   }

   public void AddLine(int team, string actorName, string txt, bool teamOnly) {
      txt = txt.Trim();
      string str = "";
      switch(team) {
         case 0:
            str = "<color=#2663a4>";
            break;
         case 1:
            str = "<color=#cc3431>";
            break;
      }

      string[] separator = new string[1] { "\n" };
      string[] array = txt.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
      if(team == -1) {
         string item = string.Format("<color={0}><i>{1}</i></color>", "#d57801", array[0]);
         chatLines.Add(item);

         GameObject msgGO = Instantiate(chatMessagePrefab, chatTransform);
         TextMeshProUGUI msg = msgGO.GetComponentInChildren<TextMeshProUGUI>();
         msg.text = item;
      }
      else {
         if(teamOnly) {
            actorName = "[T] " + actorName;
         }
         string item = str + actorName + "</color> :" + array[0];
         chatLines.Add(item);

         GameObject msgGO = Instantiate(chatMessagePrefab, chatTransform);
         TextMeshProUGUI msg = msgGO.GetComponentInChildren<TextMeshProUGUI>();
         msg.text = item;
      }

      for (int i = 1; i < array.Length; i++) {
         chatLines.Add("\t\t" + array[i]);

         GameObject msgGO = Instantiate(chatMessagePrefab, chatTransform);
         TextMeshProUGUI msg = msgGO.GetComponentInChildren<TextMeshProUGUI>();
         msg.text = "\t\t" + array[i];
      }
      //RebuildChatTextHistory();
      StartCoroutine(ForceScrollDown());
   }

   private void RebuildChatTextHistory() {
      for (int i = 0; i < chatLines.Count; i++) {
         GameObject msgGO = Instantiate(chatMessagePrefab, chatTransform);
         TextMeshProUGUI msg = msgGO.GetComponentInChildren<TextMeshProUGUI>();
         msg.text = chatLines[i];
      }
   }

   private IEnumerator ForceScrollDown() {
      // Wait for end of frame AND force update all canvases before setting to bottom.
      yield return new WaitForEndOfFrame();
      Canvas.ForceUpdateCanvases();
      scroll.verticalNormalizedPosition = 0f;
      Canvas.ForceUpdateCanvases();
   }
}
