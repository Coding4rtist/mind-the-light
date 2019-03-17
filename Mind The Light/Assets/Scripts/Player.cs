using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour {

   public string NickName { get { return PV.Controller.NickName; } }
   public int TeamID { get; set; }

   public Actor actor;
   [HideInInspector]
   public PlayerInput input;

   [HideInInspector]
   public PlayerCamera myCamera;

   [HideInInspector]
   public PhotonView PV;

   public LayerMask interactiveObjectsMask;
   public LayerMask obstacleMask;
   public List<InteractiveObject> interactiveObjects = new List<InteractiveObject>();

   void Awake() {
      PV = GetComponent<PhotonView>();
      input = GetComponent<PlayerInput>();

      actor = GetComponent<Actor>();
      actor.p = this;
   }

   private void OnTriggerEnter2D(Collider2D other) {
      if (!PV.IsMine)
         return;

      if (((1 << other.gameObject.layer) & interactiveObjectsMask) != 0) {
         InteractiveObject interactive = other.GetComponent<InteractiveObject>();

         if (!interactiveObjects.Contains(interactive)) {
            interactive.OnEnterRange(this);
            interactiveObjects.Add(interactive);
         }

      }
   }

   private void OnTriggerExit2D(Collider2D other) {
      if (!PV.IsMine)
         return;

      if (((1 << other.gameObject.layer) & interactiveObjectsMask) != 0) {
         InteractiveObject interactive = other.GetComponent<InteractiveObject>();
         interactive.OnExitRange(this);
         interactiveObjects.Remove(interactive);
      }
   }

   private void Update() {
      if (!PV.IsMine)
         return;

      if (HUD.Paused) {
         return;
      }

      if(Input.GetKeyDown(KeyCode.E)) {
         foreach(InteractiveObject obj in interactiveObjects) {
            obj.Interact(this);
         }
      }

      if(Input.GetKeyDown(KeyCode.Escape)) {
         HUD.Instance.Pause();
      }
   }

   public void SetCamera(PlayerCamera _camera) {
      myCamera = _camera;
   }
}
