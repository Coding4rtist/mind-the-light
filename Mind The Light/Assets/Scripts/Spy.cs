using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spy : Actor {

   private bool isDead = false;
   public bool IsDead {
      get { return isDead; }
      protected set { isDead = value; }
   }

   [SerializeField]
   private float maxHealth = 100f;

   private float curHealth;

   private bool isFlashing;

   public void Setup() {
      SetDefaults();
   }

   private new void Awake() {
      base.Awake();

      Setup();
   }

   public void OnSpyHit(float damage) {
      if (isDead)
         return;

      if(PhotonNetwork.IsMasterClient)
         p.PV.RPC("RPC_OnSpyHit", RpcTarget.All, damage);
   }

   private void Die() {
      isDead = true;
      // Disable components
      Debug.Log(transform.name + " is dead.");

      StartCoroutine(Respawn());
   }

   private IEnumerator Respawn() {
      yield return new WaitForSeconds(3f);

      SetDefaults();
      //Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
      //transform.position = spawnPoint.position;
      //transform.rotation = spawnPoint.rotation;
      //Debug.Log(transform.name + " respawned.");
   }

   private IEnumerator HitFlashAnim() {
      isFlashing = true;

      sr.material.SetFloat("_FlashAmount", 0.9f);
      yield return new WaitForSeconds(0.15f);
      sr.material.SetFloat("_FlashAmount", 0f);
      yield return new WaitForSeconds(0.5f);
      
      isFlashing = false;
   }

   public void SetDefaults() {
      isDead = false;
      curHealth = maxHealth;
   }


   [PunRPC]
   private void RPC_OnSpyHit(float damage) {
      curHealth -= damage;
      Debug.Log(transform.name + " now has " + curHealth + "hp.");

      if (curHealth <= 0) {
         Die();
      }
      else {
         if (!isFlashing) {
            StartCoroutine(HitFlashAnim());
         }
      }
   }
}
