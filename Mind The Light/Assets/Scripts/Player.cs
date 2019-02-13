using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour {

   private Actor actor;
   [HideInInspector]
   public PlayerInput input;

   private PlayerCamera myCamera;

   void Start() {
      input = GetComponent<PlayerInput>();

      actor = GetComponent<Actor>();
      actor.p = this;
   }

   void Update() {

   }
}
