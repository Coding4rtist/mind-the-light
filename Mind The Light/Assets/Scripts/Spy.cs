using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spy : Actor {

   private const float MAX_SPEED =  3.5f * 16;

   [SerializeField]
   private float maxHealth = 80f;
   private float curHealth;
   private bool isFlashing;

   public AudioClip takeDmgSound;
   public AudioClip deathSound;

   private Coroutine respawnCoroutine;
   private GameObject fovGO;

   private List<TargetObject> objectsStolen = new List<TargetObject>();

   float freezeTime;
   bool freeze = false;
   public bool Freeze {
      set {
         freeze = value;
         if (freeze) {
            freezeTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
         }
      }
   }

   public int ObjectsStolen {
      get { return objectsStolen.Count; }
   }



   private new void Awake() {
      base.Awake();

      fovGO = transform.GetChild(2).gameObject;
   }

   public override void SetDefaults() {
      if (PhotonNetwork.IsMasterClient)
         p.PV.RPC("RPC_SetDefaults", RpcTarget.All);
   }

   [PunRPC]
   public override void RPC_SetDefaults() {
      base.RPC_SetDefaults();

      if (respawnCoroutine != null) {
         StopCoroutine(respawnCoroutine);
         respawnCoroutine = null;
      }

      fovGO.SetActive(p.PV.IsMine);

      maxSpeed = MAX_SPEED;
      isDead = false;
      curHealth = maxHealth;
      anim.SetBool("Dead", false);
      objectsStolen = new List<TargetObject>();
      HUD.Instance.UpdateHealthBar(curHealth / maxHealth);
   }

   private new void Update() {
      if (freeze) {
         AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
         anim.Play(stateInfo.fullPathHash, 0, freezeTime);
      }

      base.Update();

      if(Input.GetKey(KeyCode.Q) && velocity.magnitude == 0f) {
         if(!freeze) {
            freeze = true;
            p.PV.RPC("RPC_Freeze", RpcTarget.Others, true);
         }
      }
      if(Input.GetKeyUp(KeyCode.Q) || velocity.magnitude != 0f) {
         freeze = false;
         p.PV.RPC("RPC_Freeze", RpcTarget.Others, false);
      }
   }

   [PunRPC]
   public void RPC_Freeze(bool value) {
      freeze = value;
   }

   public void OnSpyHit(Actor damager, float damage) {
      if (isDead)
         return;

      if (PhotonNetwork.IsMasterClient)
         p.PV.RPC("RPC_OnSpyHit", RpcTarget.All, damager.p.NickName, damage);
   }

   private void Die(string killerName) {
      isDead = true;

      velocity = Vector2.zero;

      // Disable components
      Debug.Log(transform.name + " is dead.");
      Chat.Instance.KillText(killerName, p.NickName);

      audioS.PlayOneShot(deathSound);
      anim.SetBool("Dead", true);

      foreach(TargetObject target in objectsStolen) {
         target.Replace();
      }
      objectsStolen.Clear();

      respawnCoroutine = StartCoroutine(Respawn());
   }

   private IEnumerator Respawn() {
      yield return new WaitForSeconds(3f);

      SetDefaults();
      int rand = Random.Range(0, GameManager.Instance.spawnPointsSpies.Length);
      Teleport(GameManager.Instance.spawnPointsSpies[rand].position);
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

   [PunRPC]
   private void RPC_OnSpyHit(string damagerName, float damage) {
      if (isDead)
         return;

      curHealth -= damage;
      HUD.Instance.UpdateHealthBar(curHealth / maxHealth);
      Debug.Log(transform.name + " now has " + curHealth + "hp.");

      if (curHealth <= 0) {
         Die(damagerName);
      }
      else {
         audioS.PlayOneShot(takeDmgSound);
         if (!isFlashing) {
            StartCoroutine(HitFlashAnim());
         }
      }
   }

   public void StealObject(TargetObject target) {
      objectsStolen.Add(target);
      maxSpeed -= maxSpeed * 10f / 100f;
      HUD.Instance.UpdateObjectsStolen(objectsStolen.Count);
   }
}
