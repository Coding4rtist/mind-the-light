using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractiveObject {

   public bool IsFront = true;

   public bool IsOpen = false;
   public bool IsLocked = false;

   private SpriteRenderer sr;
   private Animator anim;
   private AudioSource audioS;
   private GameObject closedColliderGO;

   private Color outlineColor = Color.white;
   private Color transparentColor = new Color(0f, 0f, 0f, 0f);

   private void Awake() {
      closedColliderGO = transform.GetChild(0).gameObject;

      sr = GetComponent<SpriteRenderer>();
      anim = GetComponent<Animator>();
      audioS = GetComponent<AudioSource>();

      anim.SetBool("front", IsFront);
      anim.SetBool("opened", IsOpen);
   }

   public override void OnEnteredRange(Player interactor) {
      // enable outline
      Debug.Log("DOOR ENTERED RANGE");
      sr.material.SetColor("_Color", outlineColor);
   }

   public override void OnExitRange(Player interactor) {
      // disable outline
      Debug.Log("DOOR EXIT RANGE");
      sr.material.SetColor("_Color", transparentColor);
   }

   public override void Interact(Player interactor) {
      Debug.Log("INTERACT");
      if(IsOpen) {
         Close(interactor);
      }
      else {
         if (IsLocked) {
            Unlock();
            Open(interactor);
         }
         else {
            Open(interactor);
         }
      }
   }


   private void Open(Player player) {
      if(!anim.GetCurrentAnimatorStateInfo(0).IsName("front-closed") && !anim.GetCurrentAnimatorStateInfo(0).IsName("side-closed")) {
         return;
      }

      anim.SetBool("opened", true);
      closedColliderGO.SetActive(false);
      audioS.Play();
      IsOpen = true;
   }

   private void Close(Player player) {
      if (!anim.GetCurrentAnimatorStateInfo(0).IsName("front-opened") && !anim.GetCurrentAnimatorStateInfo(0).IsName("side-opened")) {
         return;
      }
      anim.SetBool("opened", false);
      closedColliderGO.SetActive(true);
      audioS.Play();
      IsOpen = false;
   }

   private void Unlock() {

   }
}
