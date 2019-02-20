using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUpObject : MonoBehaviour {

   [SerializeField]
   protected string itemName;

   public bool CanBeDropped = true;

   public bool PersistsOnDeath;

   void Start() {

   }


   void Update() {

   }

   public abstract void Pickup(Player player);
}
