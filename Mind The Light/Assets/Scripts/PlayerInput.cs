using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

   [HideInInspector]
   public bool kLeft, kRight, kUp, kDown;

   void Update() {
      kLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
      kRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
      kUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
      kDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
   }
}
