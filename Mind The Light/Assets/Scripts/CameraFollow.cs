using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

   public Transform target;

   public float smoothSpeed = 10f;
   public Vector3 offset;

    void Start() {
        
    }


    void FixedUpdate() {
      if(target == null) {
         return;
      }


      Vector3 desiredPosition = transform.position = target.position + offset;
      Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
      transform.position = smoothedPosition;
      //transform.LookAt(target);
    }
}
