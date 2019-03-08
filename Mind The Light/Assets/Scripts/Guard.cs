using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Guard : Actor {

   private GunController gunController;

   private new void Awake() {
      base.Awake();

      gunController = GetComponent<GunController>();
   }

   public override void SetDefaults() {
      p.PV.RPC("RPC_SetDefaults", RpcTarget.All);
   }

   [PunRPC]
   public override void RPC_SetDefaults() {
      base.RPC_SetDefaults();
      gunController.gun.SetDefaults();
   }

}
