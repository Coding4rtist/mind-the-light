using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelCursor : MonoBehaviour {

   private SpriteRenderer sr;
   
   void Start() {
      Cursor.visible = false;
      sr = GetComponent<SpriteRenderer>();
   }
   
   void LateUpdate() {
      Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      transform.position = cursorPos;
   }
}
