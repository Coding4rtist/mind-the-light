using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : InteractiveObject {

   public GameObject lightGO;

   public bool isON = false;

   public Sprite onSprite;
   public Sprite offSprite;

   public AudioClip onSound;
   public AudioClip offSound;

   private AudioSource audioS;

   private new void Awake() {
      base.Awake();

      audioS = GetComponent<AudioSource>();
   }

   public override void Interact(Player interactor) {
      WorldManager.Instance.InteractLightSwitch(this);
   }

   public override void OnEnterRange(Player interactor) {
      base.OnEnterRange(interactor);
   }

   public override void OnExitRange(Player interactor) {
      base.OnExitRange(interactor);
   }

   public void SyncLight() {
      isON = !isON;
      lightGO.SetActive(isON);
      sr.sprite = isON ? onSprite : offSprite;
      audioS.PlayOneShot(isON ? onSound : offSound);
   }

}
