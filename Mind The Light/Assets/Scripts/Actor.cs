using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Actor : MonoBehaviourPun, IPunObservable {

   //public string state;

   private const float accel = 1.0f * 16;
   private const float fric = 2.0f * 16;
   private const float maxSpeed = 3.5f * 16;

   public Vector2 velocity;
   public Facing facing;
   private bool oldFlipX;

   //public Vector3 movDir = Vector3.zero;
   //public float moveForce = 100f;
   //private float dirVel;
   public bool slowdown;
   public float bushSlowdown;

   public Player p;
   private Animator anim;
   private Rigidbody2D rb;
   protected SpriteRenderer sr;
   protected AudioSource audioS;

   private Vector2 _networkPosition;

   private bool doDustUps = true;
   private float dustUpInterval = 0.4f;
   private float dustUpTimer;
   public AudioClip[] stepSounds;
   private int stepSoundIndex = 0;

   protected void Awake() {
      anim = GetComponent<Animator>();
      rb = GetComponent<Rigidbody2D>();
      sr = GetComponent<SpriteRenderer>();
      audioS = GetComponent<AudioSource>();
   }

   private void Update() {
      // Dust Particles
      if (doDustUps && velocity.magnitude > 0f) {
         dustUpTimer += Time.deltaTime;
         if (dustUpTimer >= dustUpInterval) {
            GameObject dustGO = PoolManager.Instance.GetPooledObject("Walk-Puff", transform.position, transform.rotation);
            Particle dust = dustGO.GetComponent<Particle>();
            dust.Play("walk-puff");
            dustUpTimer = 0f;
            audioS.PlayOneShot(stepSounds[stepSoundIndex]);
            stepSoundIndex = (stepSoundIndex + 1) % stepSounds.Length;
         }
      }

      if (!p.PV.IsMine) {
         return;
      }

      // Left
      if (p.input.kLeft && !p.input.kRight) {
         if (velocity.x > 0)
            velocity.x = Approach(velocity.x, 0, fric);
         velocity.x = Approach(velocity.x, -maxSpeed, accel);
         //state = "RUN";
      }

      // Right
      if (p.input.kRight && !p.input.kLeft) {
         if (velocity.x < 0)
            velocity.x = Approach(velocity.x, 0, fric);
         velocity.x = Approach(velocity.x, maxSpeed, accel);
         //state = "RUN";
      }

      // Up
      if (p.input.kUp && !p.input.kDown) {
         if (velocity.y < 0)
            velocity.y = Approach(velocity.y, 0, fric);
         velocity.y = Approach(velocity.y, maxSpeed, accel);
         //state = "RUN";
      }

      // Down
      if (p.input.kDown && !p.input.kUp) {
         if (velocity.y > 0)
            velocity.y = Approach(velocity.y, 0, fric);
         velocity.y = Approach(velocity.y, -maxSpeed, accel);
         //state = "RUN";
      }

      // Friction
      if (!p.input.kRight && !p.input.kLeft)
         velocity.x = Approach(velocity.x, 0, fric);
      if (!p.input.kDown && !p.input.kUp)
         velocity.y = Approach(velocity.y, 0, fric);

      //if (!p.input.kRight && !p.input.kLeft && !p.input.kDown && !p.input.kUp)
      //state = "IDLE";

      // Face mouse position
      Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
      mousePos = mousePos.normalized;
      float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
      // Side
      if (Mathf.Abs(angle) <= 45 || Mathf.Abs(angle) >= 135) {
         sr.flipX = Mathf.Abs(angle) >= 135;
         facing = sr.flipX ? Facing.LEFT : Facing.RIGHT;
      }
      // Back
      if(angle > 45 && angle < 135) {
         facing = Facing.TOP;
      }
      // Front
      if(angle > -135 && angle < -45) {
         facing = Facing.DOWN;
      }
      anim.SetFloat("FaceX", mousePos.x);
      anim.SetFloat("FaceY", mousePos.y);
      anim.SetFloat("Magnitude", velocity.magnitude);

      if(sr.flipX != oldFlipX) {
         p.PV.RPC("RPC_SendActorFlipX", RpcTarget.Others,sr.flipX);
         oldFlipX = sr.flipX;
      }

      //velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * maxSpeed;
   }

   private void FixedUpdate() {
      if(!p.PV.IsMine) {
         rb.position = Vector2.MoveTowards(rb.position, _networkPosition, Time.fixedDeltaTime);
         return;
      }
      rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
      //rb.velocity = velocity;
   }

   [PunRPC]
   protected void RPC_SendActorFlipX(bool flip) {
      sr.flipX = flip;
      oldFlipX = flip;
   }


   private float Approach(float start, float end, float shift) {
      if (start < end)
         return Mathf.Min(start + shift, end);
      else
         return Mathf.Max(start - shift, end);
   }

   public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
      if(stream.IsWriting) {
         stream.SendNext(rb.position);
         stream.SendNext(velocity);
      }
      else {
         _networkPosition = (Vector2)stream.ReceiveNext();
         rb.velocity = (Vector2)stream.ReceiveNext();
         velocity = rb.velocity;
         float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
         _networkPosition += (velocity * lag);
      }
   }
}
