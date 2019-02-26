using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractiveObject {

   public bool IsOpen;
   public bool IsLocked;

   public override void OnEnteredRange(Player interactor) {
      // enable outline
      Debug.Log("DOOR ENTERED RANGE");
   }

   public override void OnExitRange(Player interactor) {
      // disable outline
      Debug.Log("DOOR EXIT RANGE");
   }

   public override void Interact(Player interactor) {
      if (IsLocked) {
         Unlock();
         Open(interactor);
      }
      else {
         Open(interactor);
      }
   }


   private void Open(Player player) {

   }

   private void Unlock() {

   }
}
