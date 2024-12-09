using System.Collections.Generic;
using UnityEngine;

namespace UnityHelper
{
    public class PoolingSystem : Singleton<PoolingSystem>
    {
        [SerializeField]
        private PoolObjectSO _poolObjectData;
        
        private Dictionary<PoolKey,ObjectInPool> _objectsInPool = new Dictionary<PoolKey, ObjectInPool>();

        public void Pull(PoolKey poolKey,GameObject gObj)
        {
            if (!_objectsInPool.ContainsKey(poolKey))
            {
                _objectsInPool.Add(poolKey,new ObjectInPool());
            }
            _objectsInPool[poolKey].Pull(gObj);
        }
        
        public GameObject Push(PoolKey poolKey)
        {
            GameObject objectPush = null;
            if (_objectsInPool.ContainsKey(poolKey) && !_objectsInPool[poolKey].IsEmpty)
            {
                objectPush = _objectsInPool[poolKey].Push();
                
            }else
            {
                var index = _poolObjectData.GetPoolIndex(poolKey);
                if (index < 0)
                    return null;
                objectPush                                    = Instantiate(_poolObjectData.GetPrefab(poolKey));   
                objectPush.AddComponent<PoolObject>().poolKey = poolKey;
                objectPush.SetActive(false);
            }
            return objectPush;
        }
        
        private class ObjectInPool
        {
            List<GameObject> _objectsInPool = new List<GameObject>();
            
            public bool IsEmpty => _objectsInPool.Count == 0;
            
            public void Pull(GameObject gObj)
            {
                gObj.SetActive(false);
                _objectsInPool.Add(gObj);
            }

            public GameObject Push()
            {
                var index      = UnityEngine.Random.Range(0, _objectsInPool.Count);
                var objectPush = _objectsInPool[index];
                _objectsInPool.RemoveAt(index);

                return objectPush;
            }
        
        }
    }
    
    public static class PoolingSystemExtension
    {
        public static bool Pull(this GameObject gObj)
        {
            if (!gObj.TryGetComponent(out PoolObject poolObject))
            {
                return false;
            }

            PoolingSystem.Instance.Pull(poolObject.poolKey,gObj);
            return true;
        }
        
        public static GameObject Push(this MonoBehaviour gObj, PoolKey poolKey)
        {
            return PoolingSystem.Instance.Push(poolKey);
        }
    }
    
}