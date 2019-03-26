using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : InteractiveObject {

   //public int index;
   public int skin;
   public bool toSteal = false;
   private bool discovered = false;

   public Sprite[] skins;

   public GameObject targetGO;
   public GameObject questionmarkGO;

   private SpriteRenderer objectSR;
   private AudioSource audioS;

   private Color32 STEAL_COLOR = new Color32(255, 200, 37, 255);
   private Color32 NOTSTEAL_COLOR = new Color32(196, 36, 48, 255);

   private new void Awake() {
      base.Awake();

      audioS = GetComponent<AudioSource>();

      objectSR = targetGO.GetComponent<SpriteRenderer>();
   }

   public void Init(/*int _index, */int _skin) {
      //index = _index;
      skin = _skin;
      objectSR.sprite = skins[_skin];
      toSteal = false;
      objectSR.enabled = true;
      discovered = false;
      questionmarkGO.SetActive(true);
   }

   public void Replace() {
      objectSR.enabled = true;
   }

   public void Steal() {
      objectSR.enabled = false;
      audioS.Play();

      WorldManager.Instance.StealObject(this);
   }

   public void SyncSteal() {
      objectSR.enabled = false;
      audioS.Play();

      Debug.Log("SYNC STEAL " + Camera.main.WorldToScreenPoint(transform.position));
      Vector2 targetDir = new Vector2(Screen.width / 2f, Screen.height / 2f) - (Vector2)Camera.main.WorldToScreenPoint(transform.position);
      Debug.Log(targetDir.normalized);
      HUD.Instance.ShowAlarmDirection(targetDir.normalized);
   }

   public bool IsStolen() {
      return !objectSR.enabled;
   }

   public override void OnEnterRange(Player interactor) {
      base.OnEnterRange(interactor);
      if (discovered) {
         sr.material.SetColor("_Color", toSteal ? STEAL_COLOR : NOTSTEAL_COLOR);
         objectSR.material.SetColor("_Color", toSteal ? STEAL_COLOR : NOTSTEAL_COLOR);
      }
      else {
         sr.material.SetColor("_Color", outlineColor);
         objectSR.material.SetColor("_Color", outlineColor);
      }
   }

   public override void OnExitRange(Player interactor) {
      base.OnExitRange(interactor);
      objectSR.material.SetColor("_Color", transparentColor);
   }

   public override void Interact(Player interactor) {
      if (IsStolen()) {
         return;
      }

      if (interactor.actor.GetType() == typeof(Spy)) {
         if(questionmarkGO.activeSelf) {
            // Discover
            questionmarkGO.SetActive(false);
            sr.material.SetColor("_Color", toSteal ? STEAL_COLOR : NOTSTEAL_COLOR);
            objectSR.material.SetColor("_Color", toSteal ? STEAL_COLOR : NOTSTEAL_COLOR);
            discovered = true;
         }
         else {
            if(toSteal) {
               Steal();
               ((Spy)interactor.actor).StealObject(this);
            }
         }
      }
   }
}
