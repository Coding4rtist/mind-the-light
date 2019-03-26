using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour {

   protected SpriteRenderer sr;

   protected Color outlineColor = Color.white;
   protected Color transparentColor = new Color(0f, 0f, 0f, 0f);

   protected void Awake() {
      sr = GetComponent<SpriteRenderer>();
   }

   public virtual void OnEnterRange(Player interactor) {
      //Debug.Log(transform.name + ": ENTER RANGE");
      sr.material.SetColor("_Color", outlineColor);
   }

   public virtual void OnExitRange(Player interactor) {
      //Debug.Log(transform.name + ": EXIT RANGE");
      sr.material.SetColor("_Color", transparentColor);
   }

   public abstract void Interact(Player interactor);
}
