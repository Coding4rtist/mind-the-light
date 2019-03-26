using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour {

   public LayerMask collisionMask;

   private float damage;

   public GameObject particlePrefab;

   private Vector2 lastPos;

   private PhotonView PV;
   private Rigidbody2D rb;

   public Actor shooter;


   void Awake() {
      rb = GetComponent<Rigidbody2D>();
      PV = GetComponent<PhotonView>();
   }

   void FixedUpdate() {
      //if (!PV.IsMine) {
      //   transform.position = _networkPosition;
      //   transform.Translate(Vector2.up * speed * Time.deltaTime);
      //}

      float moveDistance = 20f * Time.deltaTime;
      CheckCollisions(moveDistance);
      //transform.Translate(Vector2.up * moveDistance);
   }

   void CheckCollisions(float moveDistance) {
      //Ray2D ray = new Ray2D(transform.position, rb.velocity);
      RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity.normalized, rb.velocity.magnitude * Time.fixedDeltaTime, collisionMask);
      //Debug.DrawLine(transform.position, (Vector2)transform.position + rb.velocity * Time.fixedDeltaTime, Color.red);

      if (hit) {
         OnHitObject(hit);
      }
   }

   void OnHitObject(RaycastHit2D hit) {
      if(hit.collider.tag == "Guard") {
         return;
      }

      //Debug.Log("Projectile hit: " + hit.transform.name);

      if(hit.collider.tag == "Spy") {
         Spy spy = hit.transform.GetComponent<Spy>();
         spy.OnSpyHit(shooter, 10);
         //Destroy(gameObject);
         
      }


      // Particles
      Vector2 direction = hit.transform.TransformDirection(hit.normal);
      if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Collidable") && direction.y <= 0) {
         //GameObject particleGO = Instantiate(particlePrefab, hit.point, Quaternion.identity);
         GameObject particleGO = PoolManager.Instance.GetPooledObject("Wall-Proj-Smoke", hit.point, Quaternion.identity);
         Particle particle = particleGO.GetComponent<Particle>();
         particle.Play("wall-projectile-hit", direction);

      }

      //gameObject.SetActive(false);
      PhotonNetwork.Destroy(gameObject);
   }

   public void Setup(Actor _shooter, Vector2 _velocity, float _delay, float _damage) {
      shooter = _shooter;
      rb.velocity = _velocity;
      transform.Translate(rb.velocity * (_delay / 1000));
      damage = _damage;
   }

   //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
   //   if (stream.IsWriting) {
   //      stream.SendNext((Vector2)transform.position);
   //   }
   //   else {
   //      _networkPosition = (Vector2)stream.ReceiveNext();

   //      float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
   //      _networkPosition += Vector2.up * (speed * lag);
   //   }
   //}
}
