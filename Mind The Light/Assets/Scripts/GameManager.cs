using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

   public static GameManager Instance;

   public MatchSettings matchSettings;

   private void Awake() {
      if(Instance != null) {
         Debug.LogError("More than one GameManager in scene.");
      }
      else {
         Instance = this;
      }
   }

   #region Player Tracking

   private const string PLAYER_ID_PREFIX = "Player ";

   private static Dictionary<string, Player> players = new Dictionary<string, Player>();

   public static void RegisterPlayer(string netID, Player player) {
      string playerID = PLAYER_ID_PREFIX + netID;
      players.Add(playerID, player);
      player.transform.name = playerID;
   }

   public static void UnregisterPlayer(string playerID) {
      players.Remove(playerID);
   }

   public static Player GetPlayer(string playerID) {
      return players[playerID];
   }

   //private void OnGUI() {
   //   GUILayout.BeginArea(new Rect(200, 200, 200, 500));
   //   GUILayout.BeginVertical();
   //   foreach(string playerID in players.Keys) {
   //      GUILayout.Label(playerID + " - " + players[playerID].transform.name);
   //   }
   //   GUILayout.EndVertical();
   //   GUILayout.EndArea();
   //}

   #endregion
}
