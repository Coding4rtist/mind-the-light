using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunController : MonoBehaviour {

   private PhotonView PV;

   private Gun equippedGun;

   public GameObject gun;
   private Transform muzzlePoint;
   private SpriteRenderer sr;

   private float rightHandX = 0.2f;
   private float leftHandX = -0.35f;
   private float handY = 0.25f;
   private float aimOffset = 0.35f;

   [SerializeField]
   private float rotationOffset = 20f;

   private float lastAngle = 0;
   private Vector2 handPos;
   
   void Awake() {
      sr = gun.GetComponent<SpriteRenderer>();
      muzzlePoint = gun.GetComponentInChildren<Transform>();

      PV = GetComponent<PhotonView>();
      equippedGun = GetComponentInChildren<Gun>();

      handPos = new Vector2(rightHandX, handY);
   }

  
   private void Update() {
      if(!PV.IsMine) {
         return;
      }

      Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position + handPos;
      //Debug.DrawLine(mousePos, aimPoint);
      float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

      if(angle >= 0) {
         if (angle >= lastAngle) {
            // Senso antiorario
            if (angle > 90 + rotationOffset) {
               handPos.x = leftHandX;
               sr.flipY = true;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f, -0.34f);
            }
         }
         else {
            // Senso orario
            if (angle < 90 - rotationOffset) {
               handPos.x = rightHandX;
               sr.flipY = false;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f, 0.4f);
            }
         }
      }
      else {
         if (angle <= lastAngle) {
            // Senso orario
            if (angle <= -90 - rotationOffset) {
               handPos.x = leftHandX;
               sr.flipY = true;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f, -0.34f);
            }
         }
         else {
            // Senso antiorario
            if (angle > -90 + rotationOffset) {
               handPos.x = rightHandX;
               sr.flipY = false;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f, 0.4f);
            }
         }
      }
      

      gun.transform.localPosition = handPos;
      gun.transform.eulerAngles = new Vector3(0, 0, angle);
      lastAngle = angle;


      if(Input.GetMouseButtonDown(0)) {
         equippedGun.Shoot();
      }


   }


}
