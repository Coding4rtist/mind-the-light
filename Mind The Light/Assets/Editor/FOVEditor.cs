using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(FOV))]
public class FOVEditor : Editor {

   private void OnSceneGUI() {
      FOV fov = (FOV)target;
      Handles.color = Color.white;
      Handles.DrawWireArc(fov.transform.position, Vector3.forward, Vector2.right, 360, fov.viewRadius);
      Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
      Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

      Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
      Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

      if(fov.hasSubFOV) {
         Handles.color = Color.yellow;
         Handles.DrawWireArc(fov.transform.position, Vector3.forward, Vector2.right, 360, fov.subViewRadius);
         Vector3 subViewAngleA = fov.DirFromAngle(-fov.subViewAngle / 2, false);
         Vector3 subViewAngleB = fov.DirFromAngle(fov.subViewAngle / 2, false);
         Handles.DrawLine(fov.transform.position, fov.transform.position + subViewAngleA * fov.subViewRadius);
         Handles.DrawLine(fov.transform.position, fov.transform.position + subViewAngleB * fov.subViewRadius);
      }

      Handles.color = Color.red;
      foreach (Transform visibleTarget in fov.visibleTargets) {
         Handles.DrawLine(fov.transform.position, visibleTarget.position);
      }
   }

   public override void OnInspectorGUI() {
      //base.OnInspectorGUI();
      FOV fov = (FOV)target;

      fov.viewRadius = EditorGUILayout.FloatField("View Radius", fov.viewRadius);
      fov.viewAngle = EditorGUILayout.Slider("View Angle", fov.viewAngle, 0f, 360f);
      fov.viewRes = EditorGUILayout.FloatField("View Resolution", fov.viewRes);

      fov.hasSubFOV = EditorGUILayout.BeginToggleGroup("Has Sub FOV", fov.hasSubFOV);
      fov.subViewRadius = EditorGUILayout.FloatField("SubView Radius", fov.subViewRadius);
      fov.subViewAngle = EditorGUILayout.Slider("SubView Angle", fov.subViewAngle, 0f, fov.viewAngle);
      fov.subViewRes = EditorGUILayout.FloatField("SubView Resolution", fov.subViewRes);
      EditorGUILayout.EndToggleGroup();

      //fov.obstacleMask = EditorGUILayout.MaskField("Obstacle Mask", fov.obstacleMask);
      LayerMask tempMask = EditorGUILayout.MaskField("Obstacle Mask", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(fov.obstacleMask), InternalEditorUtility.layers);
      fov.obstacleMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

      fov.edgeResolveIterations = EditorGUILayout.IntField("Edge Resolve Iterations", fov.edgeResolveIterations);
      fov.edgeDstThreshold = EditorGUILayout.FloatField("Edge Dst Threshold", fov.edgeDstThreshold);

      fov.debug = EditorGUILayout.Toggle("Debug", fov.debug);

      if (GUI.changed)
         EditorUtility.SetDirty(target);
   }
}