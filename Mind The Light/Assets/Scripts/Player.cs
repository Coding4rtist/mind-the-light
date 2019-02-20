using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour {

   public string NickName { get { return PV.Controller.NickName; } }
   public int TeamID { get; set; }

   private Actor actor;
   [HideInInspector]
   public PlayerInput input;

   [HideInInspector]
   public PlayerCamera myCamera;

   [HideInInspector]
   public PhotonView PV;

   void Start() {
      PV = GetComponent<PhotonView>();
      input = GetComponent<PlayerInput>();

      actor = GetComponent<Actor>();
      actor.p = this;
   }

   public void SetCamera(PlayerCamera _camera) {
      myCamera = _camera;
   }
}
