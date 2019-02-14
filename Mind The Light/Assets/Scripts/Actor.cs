using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Actor : MonoBehaviour {

   //public string state;

   private const float accel = 1.0f;
   private const float fric = 2.0f;
   public float maxSpeed = 6.5f;

   public Vector2 velocity;
   public Facing facing;

   public Vector3 movDir = Vector3.zero;
   public float moveForce = 100f;
   private float dirVel;
   public bool slowdown;
   public float bushSlowdown;

   public Player p;
   private Animator anim;
   private Rigidbody2D rb;
   protected SpriteRenderer sr;

   protected void Start() {
      anim = GetComponent<Animator>();
      rb = GetComponent<Rigidbody2D>();
      sr = GetComponent<SpriteRenderer>();
   }

   protected void Update() {
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
      float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
      //Debug.Log("Angle: " + angle);
      // Side
      if (Mathf.Abs(angle) <= 45 || Mathf.Abs(angle) >= 135) {
         if(velocity.magnitude > 0.2f) {
            anim.Play("run-side");
         }
         else {
            anim.Play("idle-side");
         }
         sr.flipX = Mathf.Abs(angle) >= 135;
         facing = sr.flipX ? Facing.LEFT : Facing.RIGHT;
      }
      // Back
      if(angle > 45 && angle < 135) {
         if (velocity.magnitude > 0.2f) {
            anim.Play("run-back");
         }
         else {
            anim.Play("idle-back");
         }
         facing = Facing.TOP;
      }
      // Front
      if(angle > -135 && angle < -45) {
         if (velocity.magnitude > 0.2f) {
            anim.Play("run-front");
         }
         else {
            anim.Play("idle-front");
         }
         facing = Facing.DOWN;
      }



      //velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * maxSpeed;
   }

   void FixedUpdate() {
      rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
      //rb.velocity = velocity;
      //Vector3 position = transform.localPosition;

      //position.x = (Mathf.Round(transform.parent.position.x * 16) / 16) - transform.parent.position.x;
      //position.y = (Mathf.Round(transform.parent.position.y * 16) / 16) - transform.parent.position.y;

      //transform.localPosition = position;
   }


   private float Approach(float start, float end, float shift) {
      if (start < end)
         return Mathf.Min(start + shift, end);
      else
         return Mathf.Max(start - shift, end);
   }
}
