using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnhancedFov : MonoBehaviour {

   public Tilemap tilemap;

   public float viewRadius;
   [Range(0, 360)]
   public float viewAngle;
   public LayerMask obstacleMask, targetMask;

   [SerializeField]
   private float meshRes = 1f;

   private MeshFilter viewMeshFilter;
   private Mesh mesh;

   private void Awake() {
      viewMeshFilter = GetComponent<MeshFilter>();
      mesh = new Mesh();
      mesh.name = "View Mesh";
      viewMeshFilter.mesh = mesh;
   }

   void Start() {
      Debug.Log(tilemap.WorldToCell(transform.position));
   }

   private void LateUpdate() {
      DrawFieldOfView();
   }

   private void DrawFieldOfView() {
      int stepCount = Mathf.RoundToInt(meshRes);
      float stepAngleSize = viewAngle / stepCount;

      List<Vector3> viewPoints = new List<Vector3>();
      ViewCastInfo oldViewCast = new ViewCastInfo();

      for (int i = 0; i <= stepCount; i++) {
         float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
         ViewCastInfo newViewCast = ViewCast(angle);

         viewPoints.Add(newViewCast.point);
         oldViewCast = newViewCast;

         //if (newViewCast.normal.y >= 0)
         //   Debug.DrawLine(transform.position, newViewCast.point);
         //else
         //   Debug.DrawLine(transform.position, newViewCast.point + new Vector2(0, 8.5f));
         //Debug.DrawRay(newViewCast.point, newViewCast.normal);
      }

      int vertexCount = viewPoints.Count + 1;
      Vector3[] vertices = new Vector3[vertexCount];
      int[] triangles = new int[(vertexCount - 2) * 3];

      vertices[0] = Vector3.zero;
      for (int i = 0; i < vertexCount - 1; i++) {
         vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]); // Change here


         if (i < vertexCount - 2) {
            triangles[i * 3] = i + 2;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = 0;
         }
      }

      mesh.Clear();
      mesh.vertices = vertices;
      mesh.triangles = triangles;
      mesh.RecalculateNormals();
   }

   public Vector2 DirFromAngle(float angleDeg, bool global) {
      if (!global) {
         angleDeg += transform.eulerAngles.z;
      }
      return new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));
   }

   ViewCastInfo ViewCast(float globalAngle) {
      Vector2 dir = DirFromAngle(globalAngle, false);
      RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

      Debug.Log(hit.collider.bounds);

      if (hit.collider != null) {
         Debug.DrawLine(transform.position, hit.point, Color.red);
         return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, hit.transform.TransformDirection(hit.normal));
      }
      else {
         Debug.DrawLine(transform.position, (Vector2)transform.position + dir * viewRadius, Color.blue);
         return new ViewCastInfo(false, (Vector2)transform.position + dir * viewRadius, viewRadius, globalAngle, Vector2.zero);
      }
   }

   public struct ViewCastInfo {
      public bool hit;
      public Vector2 point;
      public float dist;
      public float angle;
      public Vector2 normal;

      public ViewCastInfo(bool _hit, Vector2 _point, float _dist, float _angle, Vector2 _normal) {
         hit = _hit;
         point = _point;
         dist = _dist;
         angle = _angle;
         normal = _normal;
      }
   }
}
