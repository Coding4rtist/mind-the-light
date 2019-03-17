using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractiveObject {

   public bool IsOpen = false;
   public bool IsLocked = false;


   private Animator anim;
   private AudioSource audioS;
   private GameObject closedColliderGO;



   private new void Awake() {
      base.Awake();
      closedColliderGO = transform.GetChild(0).gameObject;

      anim = GetComponent<Animator>();
      audioS = GetComponent<AudioSource>();

      anim.SetBool("opened", IsOpen);
   }

   public override void OnEnterRange(Player interactor) {
      base.OnEnterRange(interactor);

   }

   public override void OnExitRange(Player interactor) {
      base.OnExitRange(interactor);
   }

   public override void Interact(Player interactor) {
      Debug.Log("INTERACT");
      //if(IsOpen) {
      //   Close();
      //}
      //else {
      //   if (IsLocked) {
      //      Unlock();
      //      Open();
      //   }
      //   else {
      //      Open();
      //   }
      //}

      if(IsLocked && interactor.actor.GetType() == typeof(Spy)) {
         Unlock();
         return;
      }

      WorldManager.Instance.InteractDoor(this);
   }

   public void SyncInteract() {
      if (IsOpen) {
         Close();
      }
      else {
         Open();
      }
   }


   private void Open() {
      if(anim.GetCurrentAnimatorStateInfo(0).IsName("front-closed") || anim.GetCurrentAnimatorStateInfo(0).IsName("side-closed")) {
         anim.SetBool("opened", true);
         closedColliderGO.SetActive(false);
         audioS.Play();
         IsOpen = true;
      }
   }

   private void Close() {
      if (anim.GetCurrentAnimatorStateInfo(0).IsName("front-opened") || anim.GetCurrentAnimatorStateInfo(0).IsName("side-opened")) {
         anim.SetBool("opened", false);
         closedColliderGO.SetActive(true);
         audioS.Play();
         IsOpen = false;
      }
      
   }

   private void Unlock() {

   }
}
