using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Actor {

   private GunController gunController;

   private new void Start() {
      base.Start();

      gunController = GetComponent<GunController>();
   }

   private new void Update() {
      base.Update();
   }

}
