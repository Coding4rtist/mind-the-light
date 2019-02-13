using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

   public LayerMask collisionMask;

   private float speed = 10f;

   public GameObject particlePrefab;

   private Vector2 lastPos;

   public void SetSpeed(float newSpeed) {
      speed = newSpeed;
   }

   void Start() {

   }

   void Update() {
      float moveDistance = speed * Time.deltaTime;
      CheckCollisions(moveDistance);
      transform.Translate(Vector2.up * moveDistance);
   }

   void CheckCollisions(float moveDistance) {
      Ray2D ray = new Ray2D(transform.position, transform.up);
      RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, moveDistance, collisionMask);

      if (hit) {
         OnHitObject(hit);
      }
   }

   void OnHitObject(RaycastHit2D hit) {
      Debug.Log("Projectile hit: " + hit.transform.name);
      Destroy(gameObject);
      //Destroy(hit.transform.gameObject);
      // GUn hand controller

      if(hit.collider.tag == "Player") { // TODO DA CAMBIARE IN THIEF
         Spy thief = hit.transform.GetComponent<Spy>();
         thief.TakeDamage(10);
      }


      // Particles
      Vector2 direction = hit.transform.TransformDirection(hit.normal);
      if(direction.y <= 0) {
         GameObject particleGO = Instantiate(particlePrefab, hit.point, Quaternion.identity);
         Particle particle = particleGO.GetComponent<Particle>();
         particle.Play("wall-projectile-hit", direction);

      }
   }
}
