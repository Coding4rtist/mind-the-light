using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour/*MonoBehaviourPun, IPunObservable*/ {

   public LayerMask collisionMask;

   private float speed = 10f;

   public GameObject particlePrefab;

   private Vector2 lastPos;

   private PhotonView PV;
   //private Vector2 _networkPosition;

   public void SetupDelay(float delay) {
      transform.Translate(Vector2.up * speed * (delay / 1000));
   }

   public void SetSpeed(float newSpeed) {
      speed = newSpeed;
   }

   void Awake() {
      PV = GetComponent<PhotonView>();
   }

   void Update() {
      //if (!PV.IsMine) {
      //   transform.position = _networkPosition;
      //   transform.Translate(Vector2.up * speed * Time.deltaTime);
      //}

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

      if(hit.collider.tag == "Spy") {
         Spy spy = hit.transform.GetComponent<Spy>();
         spy.OnSpyHit(10);
         Destroy(gameObject);
      }


      // Particles
      Vector2 direction = hit.transform.TransformDirection(hit.normal);
      if(direction.y <= 0) {
         GameObject particleGO = Instantiate(particlePrefab, hit.point, Quaternion.identity);
         Particle particle = particleGO.GetComponent<Particle>();
         particle.Play("wall-projectile-hit", direction);

      }
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
