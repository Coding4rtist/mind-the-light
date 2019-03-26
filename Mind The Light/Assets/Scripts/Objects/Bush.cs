using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour {

   private AudioSource audioS;

   public AudioClip enterSound;
   public AudioClip exitSound;

   private void Awake() {
      audioS = GetComponent<AudioSource>();
   }

   private void OnTriggerEnter2D(Collider2D other) {
      if((other.tag == "Guard" || other.tag == "Spy") && other.isTrigger) {
         //Debug.Log("Enter");
         audioS.PlayOneShot(enterSound);
      }
   }

   private void OnTriggerExit2D(Collider2D other) {
      if ((other.tag == "Guard" || other.tag == "Spy") && other.isTrigger) {
         //Debug.Log("Exit");
         audioS.PlayOneShot(exitSound);
      }
   }

}
