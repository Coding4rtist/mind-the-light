using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour {

   public static AudioManager Instance;

   public Sound[] sounds;
   
   private void Awake() {
      Instance = this;

      foreach(Sound s in sounds) {
         s.source = gameObject.AddComponent<AudioSource>();
         s.source.clip = s.clip;
         s.source.volume = s.volume;
         s.source.pitch = s.pitch;
      }
   }

   public void Play (string name) {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      s.source.Play();
   }
}
