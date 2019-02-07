using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerController : NetworkBehaviour {

   private float speed = 5f;

   private Rigidbody rb;
   private Vector3 velocity = Vector3.zero;

   public Weapon weapon;
   public LayerMask mask;

   public Transform weaponPivot;

   public GameObject cameraPrefab;
   private CameraFollow playerCamera;

    void Start() {
      rb = GetComponent<Rigidbody>();

      string id = "Player " + GetComponent<NetworkIdentity>().netId;
      transform.name = id;

      if(!isLocalPlayer) {
         gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
      }

      GetComponent<Player>().Setup();

      GameObject newCamera = Instantiate(cameraPrefab);
      playerCamera = newCamera.AddComponent<CameraFollow>();
      playerCamera.target = transform;
      playerCamera.offset = new Vector3(0, 20, -10);
    }

   public override void OnStartClient() {
      base.OnStartClient();
      string netID = GetComponent<NetworkIdentity>().netId.ToString();
      Player player = GetComponent<Player>();

      GameManager.RegisterPlayer(netID, player);
   }

   private void OnDisable() {
      GameManager.UnregisterPlayer(transform.name);
   }

   private void Update() {
      if(!isLocalPlayer) {
         return;
      }

      float xMov = Input.GetAxisRaw("Horizontal");
      float zMov = Input.GetAxisRaw("Vertical");

      Vector3 movH = transform.right * xMov;
      Vector3 movV = transform.forward * zMov;

      Vector3 _velocity = (movH + movV).normalized * speed;
      Move(_velocity);

      if(Input.GetButtonDown("Fire1")) {
         Shoot();
      }

      // Tentativo 1: DISCOTECA
      /*
      Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
      float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), 10000 * Time.deltaTime);
      */

      // Tentativo 2
      /*
      Vector3 mousePos = Input.mousePosition;
      mousePos.z = Camera.main.transform.position.y - transform.position.y;
      Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
      Debug.DrawLine(transform.position, worldPosition);
      transform.LookAt((Vector3)worldPosition);
      */

      // Tentativo 3
      Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
      Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
      float rayLength;
      if(groundPlane.Raycast(cameraRay, out rayLength)) {
         Vector3 pointToLook = cameraRay.GetPoint(rayLength);
         Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);

         weaponPivot.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
      }
   }

   void FixedUpdate() {
      PerformMovement();
    }

   void PerformMovement() {
      if(velocity != Vector3.zero) {
         rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
      }
   }

   [Client]
   void Shoot() {
      RaycastHit hit;

      if(Physics.Raycast(weaponPivot.position, weaponPivot.forward, out hit, weapon.range, mask)) {
         if(hit.collider.tag == "Player") {
            CmdPlayerShoot(hit.collider.name, weapon.damage);
         }
      }
   }

   [Command]
   void CmdPlayerShoot(string playerID, float damage) {
      Debug.Log(playerID + " has been shot.");

      Player player = GameManager.GetPlayer(playerID);
      player.RpcTakeDamage(damage);
   }

   public void Move(Vector3 _velocity) {
      velocity = _velocity;
   }
}
