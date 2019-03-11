using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour {

   
   public float viewRadius;
   [Range(0, 360)]
   public float viewAngle;
   [SerializeField]
   public float viewRes = 1f;
   //public float maskCutawayDst = .1f;
   public bool hasSubFOV = false;
   public float subViewRadius;
   [Range(0, 360)]
   public float subViewAngle;
   [SerializeField]
   public float subViewRes = 1f;

   public LayerMask obstacleMask, targetMask;

   public bool debug = false;


   [SerializeField]
   public int edgeResolveIterations = 4;
   [SerializeField]
   public float edgeDstThreshold = 0.5f;

   public List<Transform> visibleTargets = new List<Transform>();

   private MeshFilter viewMeshFilter;
   private Mesh mesh;

   private Vector2 lastPos;
   private Quaternion lastRot;

   private void Start() {
      viewMeshFilter = GetComponent<MeshFilter>();
      mesh = new Mesh();
      mesh.name = "View Mesh";
      viewMeshFilter.mesh = mesh;

      //StartCoroutine("FindTargetsWithDelay", .2f); // DEBUG

      //StartCoroutine("DrawFieldOfViewWithDelay", .01f); // DEBUG
   }

   IEnumerator FindTargetsWithDelay(float delay) {
      while (true) {
         yield return new WaitForSeconds(delay);
         FindVisiblePlayer();
      }
   }


   IEnumerator DrawFieldOfViewWithDelay(float delay) {
      while (true) {
         yield return new WaitForSeconds(delay);
         if (Vector2.Distance(transform.position, lastPos) >= 0.05f || Quaternion.Angle(transform.rotation, lastRot) >= 0.01f){
            DrawFieldOfView();
            lastPos = transform.position;
            lastRot = transform.rotation;
         }
      }
   }

   private void LateUpdate() {
      //if (Vector2.Distance(transform.position, lastPos) >= 0.05f) {
      //   DrawFieldOfView();
      //   lastPos = transform.position;
      //}

      //if (debug) {
      DrawFieldOfView();
      //}

      //if (Vector2.Distance(transform.position, lastPos) >= 0.05f || Quaternion.Angle(transform.rotation, lastRot) >= 0.01f) {
      //   DrawFieldOfView();
      //   lastPos = transform.position;
      //   lastRot = transform.rotation;
      //}
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
      int stepCount = Mathf.RoundToInt(viewAngle * viewRes);
      int subStepCount = Mathf.RoundToInt(subViewAngle * subViewRes);
      //float stepAngleSize = viewAngle / stepCount;

      List<Vector3> viewPoints = new List<Vector3>();
      ViewCastInfo oldViewCast = new ViewCastInfo();

      float angle = transform.eulerAngles.y - viewAngle / 2;
      int i = 0;

      while (angle <= transform.eulerAngles.y + viewAngle / 2) {
         ViewCastInfo newViewCast;

         if (hasSubFOV && angle > transform.eulerAngles.y - subViewAngle / 2 && angle < transform.eulerAngles.y + subViewAngle / 2) {
            newViewCast = ViewCast(angle, subViewRadius);

            if (i > 0) {
               bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDstThreshold;
               if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)) {
                  EdgeInfo edge = FindEdge(oldViewCast, newViewCast, subViewRadius);
                  if (edge.pointA != Vector2.zero) {
                     viewPoints.Add(edge.pointA);
                  }
                  if (edge.pointB != Vector2.zero) {
                     viewPoints.Add(edge.pointB);
                  }
               }
            }

            angle += subViewAngle / subStepCount;
         }
         else {
            newViewCast = ViewCast(angle, viewRadius);

            if (i > 0) {
               bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDstThreshold;
               if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)) {
                  EdgeInfo edge = FindEdge(oldViewCast, newViewCast, viewRadius);
                  if (edge.pointA != Vector2.zero) {
                     viewPoints.Add(edge.pointA);
                  }
                  if (edge.pointB != Vector2.zero) {
                     viewPoints.Add(edge.pointB);
                  }
               }
            }

            angle += viewAngle / stepCount;
         }

         viewPoints.Add(newViewCast.point);
         oldViewCast = newViewCast;

         //Debug.DrawRay(newViewCast.point, newViewCast.normal);
         //Debug.DrawLine(transform.position, newViewCast.point);

         i++;
      }

      if(viewAngle == 360f) {
         viewPoints.Add(viewPoints[0]);
      }


      //for (int i = 0; i <= stepCount; i++) {
      //   float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
      //   ViewCastInfo newViewCast = ViewCast(angle, viewRadius);

      //   if (i > 0) {
      //      bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDstThreshold;
      //      if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)) {
      //         EdgeInfo edge = FindEdge(oldViewCast, newViewCast, viewRadius);
      //         if (edge.pointA != Vector2.zero) {
      //            viewPoints.Add(edge.pointA);
      //         }
      //         if (edge.pointB != Vector2.zero) {
      //            viewPoints.Add(edge.pointB);
      //         }
      //      }
      //   }

      //   //if (newViewCast.normal.y < 0) {
      //   //   newViewCast.point += new Vector2(0, maskCutawayDst);
      //   //}
      //   //else {
      //   //   newViewCast.point -= newViewCast.normal * 1f;
      //   //}

      //   viewPoints.Add(newViewCast.point);
      //   oldViewCast = newViewCast;
      //   //if (newViewCast.normal.y >= 0)
      //   //   Debug.DrawLine(transform.position, newViewCast.point);
      //   //else
      //   //   Debug.DrawLine(transform.position, newViewCast.point + new Vector2(0, 8.5f));
      //   //Debug.DrawRay(newViewCast.point, newViewCast.normal);
      //}

      int vertexCount = viewPoints.Count + 1;
      Vector3[] vertices = new Vector3[vertexCount];
      int[] triangles = new int[(vertexCount - 2) * 3];

      vertices[0] = Vector3.zero;
      for (i = 0; i < vertexCount - 1; i++) {
         vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
         
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

   EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float radius) {
      float minAngle = minViewCast.angle;
      float maxAngle = maxViewCast.angle;
      Vector2 minPoint = Vector2.zero;
      Vector2 maxPoint = Vector2.zero;

      for (int i = 0; i < edgeResolveIterations; i++) {
         float angle = (minAngle + maxAngle) / 2;
         ViewCastInfo newViewCast = ViewCast(angle, radius);

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

   ViewCastInfo ViewCast(float globalAngle, float radius) {
      Vector2 dir = DirFromAngle(globalAngle, false);
      //RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

      //if (hit.collider != null) {
      //   return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, hit.transform.TransformDirection(hit.normal));
      //}
      //else {
      //   return new ViewCastInfo(false, (Vector2)transform.position + dir * viewRadius, viewRadius, globalAngle, Vector2.zero);
      //}


      // NEW

      Vector2 start = transform.position;
      Vector2 end = start + dir * radius;
      Vector2 point = start;
      Vector2 result = Vector2.zero;
      //int i = 0;

      if(debug) {
         Debug.DrawLine(end - new Vector2(1, 0), end + new Vector2(1, 0), Color.red);
         Debug.DrawLine(end - new Vector2(0, 1), end + new Vector2(0, 1), Color.red);
      }

      // VERSION 1
      //for (int i = 0; i < 3; i++) {
      //   RaycastHit2D hit = Physics2D.Linecast(point, end, obstacleMask);
      //   if (debug) {
      //      Debug.DrawLine(hit.point - new Vector2(1, 0), hit.point + new Vector2(1, 0), Color.cyan);
      //      Debug.DrawLine(hit.point - new Vector2(0, 1), hit.point + new Vector2(0, 1), Color.cyan);
      //   }


      //   if (hit.collider != null) {
      //      point = hit.point + dir * 0.001f;
      //      result = hit.point;
      //      i++;
      //   }
      //   else {
      //      point = end;
      //      result = point;
      //   }
      //}


      // VERSION 2
      RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, radius, obstacleMask);
      if (hit.collider != null) {
         Vector2 normal = hit.transform.TransformDirection(hit.normal);
         Debug.DrawRay(hit.point, hit.normal, Color.green);
         result = hit.point;
         if (normal.y < 0) {
            float cos = (result.x - start.x) / radius;
            if (result.y - start.y <= 1.5f) {
               //result = end;
               //float sin = (result.y - start.y) / radius;
               //result.x = start.x + radius * Mathf.Cos(Mathf.Asin(sin));
               result.y = start.y;
            }
            else {
               if (hit.collider.bounds.size.y > 32f) {
                  result.y = Mathf.Min(result.y + 32f, start.y + radius * Mathf.Sin(Mathf.Acos(cos)));
               }
               else {
                  result.y = Mathf.Min(result.y + hit.collider.bounds.size.y, start.y + radius * Mathf.Sin(Mathf.Acos(cos)));
               }

            }

         }
      }
      else {
         result = end;
      }


      if (debug) {
         Debug.DrawLine(end - new Vector2(1, 0), end + new Vector2(1, 0), Color.red);
         Debug.DrawLine(end - new Vector2(0, 1), end + new Vector2(0, 1), Color.red);

         Debug.DrawLine(result - new Vector2(1, 0), result + new Vector2(1, 0), Color.yellow);
         Debug.DrawLine(result - new Vector2(0, 1), result + new Vector2(0, 1), Color.yellow);
      }


      ViewCastInfo viewCastInfo;

      if (result != Vector2.zero)
         viewCastInfo = new ViewCastInfo(true, result, Vector2.Distance(start, result), globalAngle, Vector2.zero);
      else
         viewCastInfo = new ViewCastInfo(false, end, radius, globalAngle, Vector2.zero);


      return viewCastInfo;
      // NEW
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
