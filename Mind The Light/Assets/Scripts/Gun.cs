using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

   public float aimAngle;

   private const float speed = 20f * 16f;


   //public string Name;
   public Actor owner;
   public float TimeBetweenShots = 0.2f; // ms
   public float reloadTime = 0.3f;
   public int maxAmmoPerMag = 9;

   public int currentAmmoInMag;
   public float damage = 10f;


   //public Action<Player, Gun> OnAutoReload;
   //public Action<Player, Gun, bool> OnReloadPressed;
   //public Action<Player, Gun> OnFinishAttack;
   //public Action<Player, Gun> OnPostFired;
   //public Action<Player, Gun> OnAmmoChanged;

   public bool isShooting = false;
   public bool isReloading = false;

   //public bool AutoRepeat;

   public AudioClip[] soundsShoot;
   public AudioClip soundReload;

   public Sprite normalSprite;
   public Sprite shootSprite;

   public GameObject projectilePrefab;
   public GameObject shotFlashPrefab;
   public Transform muzzleTransform;

   private float nextShotTime;
   private SpriteRenderer sr;
   private AudioSource audioSource;

   private ShakeSettings shakeSettings = new ShakeSettings(0f, 2f, 20f * 16, 0.5f, 0.5f, 0.5f);

   private void Awake() {
      sr = GetComponent<SpriteRenderer>();
      audioSource = GetComponent<AudioSource>();

      owner = transform.parent.GetComponent<Actor>();
      SetDefaults();
   }

   public AttackResult Shoot(float delay) {
      if (!isReloading && !isShooting && currentAmmoInMag > 0) {
         isShooting = true;
         currentAmmoInMag--;
         HUD.Instance.UpdateMagazine(currentAmmoInMag);
         sr.sprite = shootSprite;
         audioSource.PlayOneShot(soundsShoot[currentAmmoInMag % 2]);

         // Projectile
         //GameObject projectileGO = Instantiate(projectilePrefab, muzzleTransform.position, muzzleTransform.rotation);
         //GameObject projectileGO = Photon.Pun.PhotonNetwork.InstantiateSceneObject(System.IO.Path.Combine(Consts.PHOTON_FOLDER, "Projectile"), muzzleTransform.position, muzzleTransform.rotation, 0);
         GameObject projectileGO = PoolManager.Instance.GetPooledObject("Projectile", muzzleTransform.position, Quaternion.identity);
         Projectile proj = projectileGO.GetComponent<Projectile>();
         Vector2 direction = new Vector2(Mathf.Cos(aimAngle * Mathf.Deg2Rad), Mathf.Sin(aimAngle * Mathf.Deg2Rad));
         proj.Setup(owner, direction * speed, delay, damage);

         // Particles
         //GameObject shotFlashGO = Instantiate(shotFlashPrefab, muzzleTransform.position, muzzleTransform.rotation);
         GameObject shotFlashGO = PoolManager.Instance.GetPooledObject("Shot-Flash", muzzleTransform.position, muzzleTransform.rotation);
         shotFlashGO.transform.eulerAngles = new Vector3(0, 0, shotFlashGO.transform.eulerAngles.z + 90);
         Particle shotFlash = shotFlashGO.GetComponent<Particle>();
         shotFlash.Play("shot-flash");

         StartCoroutine(AnimateShoot());
         return AttackResult.Success;
      }

      if (!isReloading && !isShooting && currentAmmoInMag == 0) {
         Reload();
         return AttackResult.Reload;
      }

      return AttackResult.OnCooldown;
   }

   private IEnumerator AnimateShoot() {


      yield return new WaitForSeconds(TimeBetweenShots * 2 / 3);


      sr.sprite = normalSprite;

      yield return new WaitForSeconds(TimeBetweenShots / 3);
      isShooting = false;
   }

   public bool Reload() {
      if(currentAmmoInMag == maxAmmoPerMag || isReloading) {
         return false;
      }

      isReloading = true;
      sr.sprite = shootSprite;
      audioSource.PlayOneShot(soundReload);

      StartCoroutine(AnimateReload());
      return true;
   }

   private IEnumerator AnimateReload() {
      yield return new WaitForSeconds(1.2f);

      float reloadSpeed = 1f / reloadTime;
      float percent = 0;
      while(percent < 1) {
         percent += Time.deltaTime * reloadSpeed;
         yield return null;
      }

      isReloading = false;
      sr.sprite = normalSprite;
      currentAmmoInMag = maxAmmoPerMag;
      HUD.Instance.UpdateMagazine(currentAmmoInMag);
   }

   public void DoScreenShake() {
      Vector2 dir = Quaternion.Euler(0f, 0f, aimAngle) * Vector2.right;
      shakeSettings.angle = aimAngle;
      owner.p.myCamera.DoGunScreenShake(shakeSettings);
   }

   public void SetDefaults() {
      currentAmmoInMag = maxAmmoPerMag;
      HUD.Instance.UpdateMagazine(currentAmmoInMag);
   }

}

public enum AttackResult {
   Success,
   OnCooldown,
   Reload,
   Empty,
   Fail
}
