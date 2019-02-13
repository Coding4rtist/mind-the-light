using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

   public string Name;
   public float Damage;
   public float TimeBetweenShots = 0.1f; // ms
   public bool AutoRepeat;
   public int BulletPerMagazine;
   public float ReloadTime;
   public AudioClip soundShoot;
   public AudioClip soundReload;

   public GameObject projectilePrefab;
   public GameObject shotFlashPrefab;
   public Transform muzzleTransform;

   private float nextShotTime;

   void Start() {

   }


   void Update() {

   }

   public void Shoot() {
      if (Time.time > nextShotTime) {
         nextShotTime = Time.time + TimeBetweenShots;
         GameObject projectileGO = Instantiate(projectilePrefab, muzzleTransform.position, muzzleTransform.rotation);
         Projectile proj = projectileGO.GetComponent<Projectile>();
         proj.SetSpeed(20f);

         // Particles
         GameObject shotFlashGO = Instantiate(shotFlashPrefab, muzzleTransform.position, muzzleTransform.rotation);
         shotFlashGO.transform.eulerAngles = new Vector3(0, 0, shotFlashGO.transform.eulerAngles.z + 90);
         Particle shotFlash = shotFlashGO.GetComponent<Particle>();
         shotFlash.Play("shot-flash");
      }
   }
}
