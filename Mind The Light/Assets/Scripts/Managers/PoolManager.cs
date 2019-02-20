using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem {
   public GameObject objectToPool;
   public int amountToPool;
   public bool shouldExpand;
}

public class PoolManager : MonoBehaviour {

   public static PoolManager Instance;
   public List<ObjectPoolItem> itemsToPool;
   public List<GameObject> pooledObjects;

   void Awake() {
      Instance = this;
   }

   // Use this for initialization
   void Start() {
      pooledObjects = new List<GameObject>();
      foreach (ObjectPoolItem item in itemsToPool) {
         GameObject container = new GameObject("Pooled Objects: " + item.objectToPool.tag);
         container.transform.SetParent(transform);
         for (int i = 0; i < item.amountToPool; i++) {
            GameObject obj = Instantiate(item.objectToPool, container.transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
         }
      }
   }

   public GameObject GetPooledObject(string tag) {
      for (int i = 0; i < pooledObjects.Count; i++) {
         if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag) {
            return pooledObjects[i];
         }
      }
      foreach (ObjectPoolItem item in itemsToPool) {
         if (item.objectToPool.tag == tag) {
            if (item.shouldExpand) {
               GameObject obj = Instantiate(item.objectToPool);
               obj.SetActive(false);
               pooledObjects.Add(obj);
               return obj;
            }
         }
      }
      return null;
   }

   public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation) {
      GameObject go = GetPooledObject(tag);
      if (go == null)
         return null;

      go.transform.position = position;
      go.transform.rotation = rotation;
      go.SetActive(true);
      return go;
   }
}