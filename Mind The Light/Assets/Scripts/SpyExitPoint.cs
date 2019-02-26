using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyExitPoint : MonoBehaviour {


   private void OnTriggerEnter2D(Collider2D other) {
      if(other.tag == "Spy") {
         //NetPlayer component = other.get_gameObject().get_transform().get_parent()
         //   .GetComponent<NetPlayer>();
         //if (component != null && component.team == 1) {
         //   if (component.stolenObjects.Count != 0) {
         //      Server.sendGameMsgToServer(component, "thiefEscape", string.Empty, string.Empty, string.Empty);
         //   }
         //   component.stolenObjects.Clear();
         //}
      }
   }
}
