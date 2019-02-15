using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Actor {

   private GunController gunController;

   private new void Awake() {
      base.Awake();

      gunController = GetComponent<GunController>();
   }

   public override void UpdateActor() {
      base.UpdateActor();
   }

   public override void FixedUpdateActor() {
      base.FixedUpdateActor();
   }

}
