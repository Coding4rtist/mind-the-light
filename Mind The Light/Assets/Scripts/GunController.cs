using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunController : MonoBehaviourPun, IPunObservable {

   private PhotonView PV;

   public GameObject gunGO;
   private Gun gun;

   
   //public Transform barrelOffset;
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
         return;
      }

      Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      Vector2 barrelPosition = gun.muzzleTransform.position;

      float dist = Vector2.Distance(mousePos, barrelPosition);
      float distClamped = Mathf.Clamp01((dist - 2f * 10) / 3f);
      barrelPosition = Vector2.Lerp(hitbox.bounds.center, barrelPosition, distClamped);
      Vector2 delta = mousePos - barrelPosition;
      
      gunAngle = Mathf.Atan2(delta.y, delta.x) * 57.29578f;

      if (gunAngle >= 0) {
         if (gunAngle >= lastAngle) {
            // Senso antiorario
            if (gunAngle > 90 + rotationOffset) {
               handPos.x = leftHandX;
               sr.flipY = true;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f * 16, -0.34f * 16);
            }
         }
         else {
            // Senso orario
            if (gunAngle < 90 - rotationOffset) {
               handPos.x = rightHandX;
               sr.flipY = false;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f * 16, 0.4f * 16);
            }
         }
      }
      else {
         if (gunAngle <= lastAngle) {
            // Senso orario
            if (gunAngle <= -90 - rotationOffset) {
               handPos.x = leftHandX;
               sr.flipY = true;
               muzzlePoint.GetChild(0).localPosition = new Vector2(0.7f * 16, -0.34f * 16);
            }
         }
         else {
            // Senso antiorario
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

      gun.transform.localPosition = handPos;
      gun.transform.localRotation = Quaternion.Euler(0, 0, gunAngle);
      lastAngle = gunAngle;

      if (Input.GetMouseButtonDown(0)) {
         var ping = PhotonNetwork.GetPing();
         PV.RPC("RPC_Shoot", RpcTarget.Others, ping);
         gun.Shoot(0);
      }
   }

   void OnDrawGizmos() {
      // Draw a yellow sphere at the transform's position
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(new Vector3(leftHandX, handY), 1);
      Gizmos.DrawSphere(new Vector3(rightHandX, handY), 1);
   }

   [PunRPC]
   protected void RPC_SendGunFlipY(bool flip) {
      sr.flipY = flip;
      oldFlipY = flip;
   }

   [PunRPC]
   protected void RPC_Shoot(int remotePing) {
      var ping = PhotonNetwork.GetPing();
      var delay = (float)(ping / 2 + remotePing / 2);
      gun.Shoot(delay);
   }

   public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
      if (stream.IsWriting) {
         stream.SendNext(sr.flipY);
         stream.SendNext(gunAngle);
      }
      else {
         float handX = ((bool)stream.ReceiveNext()) ? leftHandX : rightHandX;
         gun.transform.localPosition = new Vector2(handX, handY);
         _networkRotation = Quaternion.Euler(0, 0, (float)stream.ReceiveNext());
      }
   }

}
