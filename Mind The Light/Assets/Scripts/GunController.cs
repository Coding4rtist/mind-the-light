using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;


public class GunController : MonoBehaviourPun, IPunObservable {

   private PhotonView PV;

   public GameObject gunGO;
   [HideInInspector]
   public Gun gun;


   public Transform fovTransform;
   private Transform muzzlePoint;
   private BoxCollider2D hitbox;
   private SpriteRenderer sr;

   private float rightHandX = 0.35f * 16;
   private float leftHandX = -0.35f * 16;
   private float handY = 0.25f * 16;

   [SerializeField]
   private float rotationOffset = 20f;

   public float gunAngle;
   private float lastAngle = 0;
   private Vector2 handPos;
   private bool oldFlipY;

   private Vector2 fovDir;

   private Quaternion _networkRotation= Quaternion.identity;

   void Awake() {
      hitbox = GetComponent<BoxCollider2D>();
      PV = GetComponent<PhotonView>();
      gun = GetComponentInChildren<Gun>();

      sr = gunGO.GetComponent<SpriteRenderer>();
      muzzlePoint = gunGO.GetComponentInChildren<Transform>();

      handPos = new Vector2(rightHandX, handY);
   }

  
   private void Update() {
      if (!PV.IsMine) {
         gun.transform.localRotation = Quaternion.Lerp(gun.transform.localRotation, _networkRotation, Time.deltaTime * 300f);
         fovTransform.right = fovDir;
         return;
      }

      if(HUD.Paused || !GameManager.Instance.roundStarted) {
         return;
      }

      Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

      gunAngle = HandleAimRotation(mousePos);
      gun.aimAngle = gunAngle;

      gun.transform.localPosition = handPos;
      gun.transform.localRotation = Quaternion.Lerp(gun.transform.localRotation, Quaternion.Euler(0f, 0f, gunAngle), Time.deltaTime * 32f);

      fovDir = mousePos - (Vector2)transform.position;
      fovTransform.right = fovDir;


      if (gunAngle >= 0) {
         if (gunAngle > lastAngle) {
            // Senso antiorario
            if (gunAngle > 90 + rotationOffset) {
               handPos.x = leftHandX;
               sr.flipY = true;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f * 16, -0.34f * 16);
            }
         }
         else if (gunAngle < lastAngle) {
            // Senso orario
            if (gunAngle < 90 - rotationOffset) {
               handPos.x = rightHandX;
               sr.flipY = false;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f * 16, 0.4f * 16);
            }
         }
      }
      else {
         if (gunAngle < lastAngle) {
            // Senso orario
            lastAngle = gunAngle;
            if (gunAngle <= -90 - rotationOffset) {
               handPos.x = leftHandX;
               sr.flipY = true;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f * 16, -0.34f * 16);
            }
         }
         else if (gunAngle > lastAngle) {
            // Senso antiorario
            lastAngle = gunAngle;
            if (gunAngle > -90 + rotationOffset) {
               handPos.x = rightHandX;
               sr.flipY = false;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f * 16, 0.4f * 16);
            }
         }
      }
      
      

      if (sr.flipY != oldFlipY) {
         PV.RPC("RPC_SendGunFlipY", RpcTarget.Others, sr.flipY);
         oldFlipY = sr.flipY;
      }

      if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
         var ping = PhotonNetwork.GetPing();
         AttackResult result = gun.Shoot(0);
         if(result == AttackResult.Success || result == AttackResult.Reload) {
            PV.RPC("RPC_Shoot", RpcTarget.Others, ping);
            //if(result == AttackResult.Success) {
            //   gun.DoScreenShake();
            //}
         }
            
      }

      if(Input.GetKeyDown(KeyCode.R)) {
         bool success = gun.Reload();
         if (success)
            PV.RPC("RPC_Reload", RpcTarget.Others);
      }

      lastAngle = gunAngle;
   }

   [PunRPC]
   protected void RPC_SendGunFlipY(bool flip) {
      sr.flipY = flip;
      oldFlipY = flip;
      muzzlePoint.GetChild(0).localPosition = flip ? new Vector2(0.7f * 16, -0.34f * 16) : new Vector2(0.7f * 16, 0.4f * 16);
   }

   [PunRPC]
   protected void RPC_Shoot(int remotePing) {
      var ping = PhotonNetwork.GetPing();
      var delay = (float)(ping / 2 + remotePing / 2);
      gun.Shoot(delay);
   }

   [PunRPC]
   protected void RPC_Reload() {
      gun.Reload();
   }

   public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
      if (stream.IsWriting) {
         stream.SendNext(sr.flipY);
         stream.SendNext(gunAngle);
         stream.SendNext(fovDir);
      }
      else {
         float handX = ((bool)stream.ReceiveNext()) ? leftHandX : rightHandX;
         gun.transform.localPosition = new Vector2(handX, handY);
         float angle = (float)stream.ReceiveNext();
         _networkRotation = Quaternion.Euler(0, 0, angle);
         gun.aimAngle = angle;
         fovDir = (Vector2)stream.ReceiveNext();
      }
   }

   private float HandleAimRotation(Vector2 aimPoint) {
      Vector2 center = hitbox.bounds.center;
      float dist = Vector2.Distance(aimPoint, center);
      float clampedDist = Mathf.Clamp01((dist - 8f) / 16f);
      Vector2 val = Vector2.Lerp(center, gun.muzzleTransform.position, clampedDist);
      //Debug.DrawLine(val - Vector2.up * 3f, val + Vector2.up * 3f);
      //Debug.DrawLine(val - Vector2.right * 3f, val + Vector2.right * 3f);
      Vector2 delta = aimPoint - val;
      //Debug.DrawLine(aimPoint, val, Color.red);
      return Mathf.Atan2(delta.y, delta.x) * 57.29578f;
   }

}
