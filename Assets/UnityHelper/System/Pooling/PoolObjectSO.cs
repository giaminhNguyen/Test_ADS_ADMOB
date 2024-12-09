using System;
using UnityEngine;

namespace UnityHelper
{
    [CreateAssetMenu(fileName = "PoolObjectSO", menuName = "DataSO/PoolObjectSO")]
    public class PoolObjectSO : ScriptableObject
    {
        public PoolInfo[] poolObjects;
        
        public int GetPoolIndex(PoolKey poolKey)
        {
            var index = Array.FindIndex(poolObjects, x => x.poolKey == poolKey);
            return index;
        }
        
        public GameObject GetPrefab(PoolKey poolKey)
        {
            var index = GetPoolIndex(poolKey);
            return GetPrefab(index);
        }
        
        public GameObject GetPrefab(int index)
        {
            return index < 0 || index >= poolObjects.Length ? null : poolObjects[index].GetPrefab();
        }
        
    }
    
    [Serializable]
    public struct PoolInfo
    {
        public PoolKey      poolKey;
        public GameObject[] prefab;
        
        public GameObject GetPrefab()
        {
            var index = UnityEngine.Random.Range(0, prefab.Length);
            return prefab[index];
        }
        
    }
    
}