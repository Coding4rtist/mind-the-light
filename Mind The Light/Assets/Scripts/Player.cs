using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour {

   private Actor actor;
   [HideInInspector]
   public PlayerInput input;

   public GameObject cameraPrefab;

   private PlayerCamera myCamera;

   [HideInInspector]
   public PhotonView PV;

   void Start() {
      PV = GetComponent<PhotonView>();
      input = GetComponent<PlayerInput>();

      actor = GetComponent<Actor>();
      actor.p = this;
   }
}
