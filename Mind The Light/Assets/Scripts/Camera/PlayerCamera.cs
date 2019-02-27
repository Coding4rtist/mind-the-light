using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

   public Transform target;

   public float smoothSpeed = 10f;
   public Vector3 offset;

   private IEnumerator currentShakeCoroutine;

   void Start() {

   }


   void FixedUpdate() {
      if (target == null) {
         return;
      }


      Vector3 desiredPosition = transform.position = target.position + offset;
      Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
      if(System.Math.Round(smoothedPosition.y - Mathf.FloorToInt(smoothedPosition.y), 1) == 0.5f) {
         smoothedPosition.y += 0.01f;
      }
      transform.position = smoothedPosition;
   }

   public void DoGunScreenShake(ShakeSettings settings) {
      if(currentShakeCoroutine != null) {
         StopCoroutine(currentShakeCoroutine);
      }
      currentShakeCoroutine = Shake(settings);
      StartCoroutine(currentShakeCoroutine);
   }

   private IEnumerator Shake(ShakeSettings settings) {
      float completionPercent = 0;
      float movePercent = 0;

      float angle_radians = settings.angle * Mathf.Deg2Rad - Mathf.PI;
      Vector3 previousWaypoint = new Vector3(0,0,-10);
      Vector3 currentWaypoint = new Vector3(0, 0, -10);
      float moveDistance = 0;

      while(true) {
         if (movePercent >= 1 || completionPercent == 0) {
            float dampingFactor = DampingCurve(completionPercent, settings.dampingPercent);
            float noiseAngle = (Random.value - .5f) * 2 * Mathf.PI / 2f;
            angle_radians += Mathf.PI + noiseAngle * settings.noisePercent;
            currentWaypoint = new Vector3(Mathf.Cos(angle_radians), Mathf.Sin(angle_radians)) * settings.strength * dampingFactor;
            currentWaypoint.z = -10;
            previousWaypoint = transform.localPosition;

            moveDistance = Vector2.Distance(currentWaypoint, previousWaypoint);
            movePercent = 0;
         }

         completionPercent += Time.deltaTime / settings.duration;
         movePercent += Time.deltaTime / moveDistance * settings.speed;
         transform.localPosition = Vector3.Lerp(previousWaypoint, currentWaypoint, movePercent);
         

         yield return null;

      }

   }

   private float DampingCurve(float x, float dampingPercent) {
      x = Mathf.Clamp01(x);
      float a = Mathf.Lerp(2, .25f, dampingPercent);
      float b = 1 - Mathf.Pow(x, a);
      return b * b * b;
   }
}

[System.Serializable]
public class ShakeSettings {
   public float angle;
   public float strength;
   public float speed;
   public float duration;
   [Range(0, 1)]
   public float noisePercent;
   [Range(0, 1)]
   public float dampingPercent;

   public ShakeSettings(float ang, float mag, float spd, float time, float noise, float damp) {
      angle = ang;
      strength = mag;
      speed = spd;
      duration = time;
      noisePercent = noise;
      dampingPercent = damp;
   }
}
