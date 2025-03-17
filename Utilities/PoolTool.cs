using UnityEngine;
using UnityEngine.Pool;

public class PoolTool : MonoBehaviour
{
   public GameObject storePrefab;
   private ObjectPool<GameObject> pool;

   private void Awake()
   {
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
