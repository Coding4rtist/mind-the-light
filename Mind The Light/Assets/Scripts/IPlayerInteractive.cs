using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInteractive {

   float GetDistanceToPoint(Vector2 point);

   void OnEnteredRange(Player interactor);

   void OnExitRange(Player interactor);

   void Interact(Player interactor);

   string GetAnimationState(Player interactor, out bool shouldBeFlipped);

   float GetOverrideMaxDistance();
}
