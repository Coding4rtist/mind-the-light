using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {

   private Animator anim;
   private SpriteRenderer sr;

   private void Awake() {
      anim = GetComponent<Animator>();
      sr = GetComponent<SpriteRenderer>();
   }

   public void Play(string animation) {
      StartCoroutine(OnAnimationEnd(animation));
   }

   public void Play(string animation, Vector2 direction) {
      string suffix = "-front";
      if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
         suffix = "-side";
         sr.flipX = direction.x < 0;
      }
      StartCoroutine(OnAnimationEnd(animation + suffix));
   }

   private IEnumerator OnAnimationEnd(string animation) {
      anim.Play(animation);
      yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
      gameObject.SetActive(false);
      Destroy(gameObject);
   }
}
