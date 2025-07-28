using System;
using UnityEngine;
using UnityEngine.Pool;

public class PoolTool : MonoBehaviour
{
   private GameObject storePrefab;
   private ObjectPool<GameObject> pool;

   private void Awake()
   {
      storePrefab = Resources.Load<GameObject>("RealDesign/SoldierOnStore_prefab");
      
      pool = new ObjectPool<GameObject>(
         createFunc:()=> Instantiate(storePrefab,transform),
         actionOnGet: obj=>gameObject.SetActive(true),
         actionOnRelease: obj=>obj.SetActive(false),
         actionOnDestroy:obj=>Destroy(obj.gameObject),
         collectionCheck:false,
         defaultCapacity:50,
         maxSize:50
         );

   }
   

   public GameObject GetObjectFromPool()
   {
      return pool.Get();
   }

   public void ReturnObjectToPool(GameObject obj)
   {
      pool.Release(obj);
   }
}
