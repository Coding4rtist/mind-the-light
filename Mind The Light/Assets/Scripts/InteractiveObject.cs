using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour {

   public abstract void OnEnteredRange(Player interactor);

   public abstract void OnExitRange(Player interactor);

   public abstract void Interact(Player interactor);
}
