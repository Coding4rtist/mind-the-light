using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Actor : MonoBehaviourPun {

   private const float ACCEL = 1.0f * 16;
   private const float FRIC = 2.0f * 16;
   protected const float MAX_SPEED = 3.5f * 16;

   //public string state;
   protected float maxSpeed = MAX_SPEED;

   public Vector2 velocity;
   public Facing facing;
   private bool oldFlipX;

   [HideInInspector]
   protected bool isDead = false;

   //public Vector3 movDir = Vector3.zero;
   //public float moveForce = 100f;
   //private float dirVel;
   public bool slowdown;
   public float bushSlowdown;

   public Player p;
   protected Animator anim;
   private Rigidbody2D rb;
   protected SpriteRenderer sr;
   protected AudioSource audioS;

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

   public virtual void SetDefaults() {

   }

   [PunRPC]
   public virtual void RPC_SetDefaults() {

   }

   protected void Update() {
      // Dust Particles
      if (doDustUps && rb.velocity.magnitude > 0.1f) {
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

      if (!p.PV.IsMine || isDead) {
         return;
      }

      // Left
      if (p.input.kLeft && !p.input.kRight) {
         if (velocity.x > 0)
            velocity.x = Approach(velocity.x, 0, FRIC);
         velocity.x = Approach(velocity.x, -maxSpeed, ACCEL);
         //state = "RUN";
      }

      // Right
      if (p.input.kRight && !p.input.kLeft) {
         if (velocity.x < 0)
            velocity.x = Approach(velocity.x, 0, FRIC);
         velocity.x = Approach(velocity.x, maxSpeed, ACCEL);
         //state = "RUN";
      }

      // Up
      if (p.input.kUp && !p.input.kDown) {
         if (velocity.y < 0)
            velocity.y = Approach(velocity.y, 0, FRIC);
         velocity.y = Approach(velocity.y, maxSpeed, ACCEL);
         //state = "RUN";
      }

      // Down
      if (p.input.kDown && !p.input.kUp) {
         if (velocity.y > 0)
            velocity.y = Approach(velocity.y, 0, FRIC);
         velocity.y = Approach(velocity.y, -maxSpeed, ACCEL);
         //state = "RUN";
      }

      // Friction
      if (!p.input.kRight && !p.input.kLeft)
         velocity.x = Approach(velocity.x, 0, FRIC);
      if (!p.input.kDown && !p.input.kUp)
         velocity.y = Approach(velocity.y, 0, FRIC);

      //if (!p.input.kRight && !p.input.kLeft && !p.input.kDown && !p.input.kUp)
      //state = "IDLE";

      // Face mouse position
      Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position - new Vector3(0, 4f, 0);
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
         return;
      }
      rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
      rb.velocity = velocity;
   }

   public void Teleport(Vector2 newPos) {
      p.PV.RPC("RPC_Teleport", RpcTarget.All, newPos);
   }


   [PunRPC]
   protected void RPC_Teleport(Vector2 newPos) {
      rb.position = newPos;
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
}
