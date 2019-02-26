using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour {

   
   public float viewRadius;
   [Range(0, 360)]
   public float viewAngle;
   public float maskCutawayDst = .1f;
   public LayerMask obstacleMask, targetMask;

   [SerializeField]
   private float meshRes = 1f;
   [SerializeField]
   private int edgeResolveIterations = 4;
   [SerializeField]
   private float edgeDstThreshold = 0.5f;

   public List<Transform> visibleTargets = new List<Transform>();

   public MeshFilter viewMeshFilter;
   private Mesh mesh;

   private void Start() {
      mesh = new Mesh();
      mesh.name = "View Mesh";
      viewMeshFilter.mesh = mesh;

      StartCoroutine("FindTargetsWithDelay", .2f);
   }

   IEnumerator FindTargetsWithDelay(float delay) {
      while (true) {
         yield return new WaitForSeconds(delay);
         FindVisiblePlayer();
      }
   }

   private void LateUpdate() {
      DrawFieldOfView();
   }

   private void FindVisiblePlayer() {
      visibleTargets.Clear();
      Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

      for(int i=0; i<targetsInViewRadius.Length; i++) {
         Transform target = targetsInViewRadius[i].transform;
         Vector2 dirToTarget = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);

         if(Vector2.Angle(dirToTarget, transform.right) < viewAngle / 2) {
            float distancePlayer = Vector2.Distance(transform.position, target.position);
            if (!Physics2D.Raycast(transform.position, dirToTarget, distancePlayer, obstacleMask)) {
               visibleTargets.Add(target);
            }
         }
      }

   }

   void DrawFieldOfView() {
      int stepCount = Mathf.RoundToInt(viewAngle * meshRes);
      float stepAngleSize = viewAngle / stepCount;

      List<Vector3> viewPoints = new List<Vector3>();
      ViewCastInfo oldViewCast = new ViewCastInfo();

      for (int i=0;i<=stepCount;i++) {
         float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
         ViewCastInfo newViewCast = ViewCast(angle);

         if(i > 0) {
            bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDstThreshold;
            if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)) {
               EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
               if (edge.pointA != Vector2.zero) {
                  viewPoints.Add(edge.pointA);
               }
               if (edge.pointB != Vector2.zero) {
                  viewPoints.Add(edge.pointB);
               }
            }
         }

         if (newViewCast.normal.y < 0) {
            newViewCast.point += new Vector2(0, maskCutawayDst);
         }
         else {
            newViewCast.point -= newViewCast.normal * 1f;
         }

         viewPoints.Add(newViewCast.point);
         oldViewCast = newViewCast;
         //if(newViewCast.normal.y >= 0)
         //   Debug.DrawLine(transform.position, newViewCast.point);
         //else
         //   Debug.DrawLine(transform.position, newViewCast.point + new Vector2(0,8.5f));
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

   EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
      float minAngle = minViewCast.angle;
      float maxAngle = maxViewCast.angle;
      Vector2 minPoint = Vector2.zero;
      Vector2 maxPoint = Vector2.zero;

      for (int i = 0; i < edgeResolveIterations; i++) {
         float angle = (minAngle + maxAngle) / 2;
         ViewCastInfo newViewCast = ViewCast(angle);

         bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dist - newViewCast.dist) > edgeDstThreshold;
         if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded) {
            minAngle = angle;
            minPoint = newViewCast.point;
         }
         else {
            maxAngle = angle;
            maxPoint = newViewCast.point;
         }
      }

      return new EdgeInfo(minPoint, maxPoint);
   }

   ViewCastInfo ViewCast(float globalAngle) {
      Vector2 dir = DirFromAngle(globalAngle, false);
      RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

      if (hit.collider != null) {
         return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, hit.transform.TransformDirection(hit.normal));
      }
      else {
         return new ViewCastInfo(false, (Vector2)transform.position + dir * viewRadius, viewRadius, globalAngle, Vector2.zero);
      }
   }

   public Vector2 DirFromAngle(float angleDeg, bool global) {
      if(!global) {
         angleDeg += transform.eulerAngles.z;
      }
      return new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));

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

   public struct EdgeInfo {
      public Vector2 pointA;
      public Vector2 pointB;

      public EdgeInfo(Vector3 _pointA, Vector3 _pointB) {
         pointA = _pointA;
         pointB = _pointB;
      }
   }
}
