using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

   [SyncVar]
   private bool isDead = false;
   public bool IsDead {
      get { return isDead; }
      protected set { isDead = value; }
   }


   [SerializeField]
   private float maxHealth = 100f;

   [SyncVar]
   private float curHealth;

   public void Setup() {
      SetDefaults();
   }

   void Start() {
        
    }


    void Update() {
        
    }

   [ClientRpc]
   public void RpcTakeDamage(float amount) {
      if (IsDead)
         return;

      curHealth -= amount;
      Debug.Log(transform.name + " now has " + curHealth + "hp.");

      if(curHealth <= 0) {
         Die();
      }
   }

   private void Die() {
      IsDead = true;
      // Disable components
      Debug.Log(transform.name + " is dead.");

      StartCoroutine(Respawn());
   }

   private IEnumerator Respawn() {
      yield return new WaitForSeconds(GameManager.Instance.matchSettings.respawnTime);

      SetDefaults();
      Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
      transform.position = spawnPoint.position;
      transform.rotation = spawnPoint.rotation;
      Debug.Log(transform.name + " respawned.");
   }

   public void SetDefaults() {
      IsDead = false;
      curHealth = maxHealth;
   }
}
