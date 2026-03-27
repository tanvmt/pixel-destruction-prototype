using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PixelDestruction.Core
{
    [System.Serializable]
    public class PoolConfig
    {
        public string poolId; 
        public GameObject prefab;
        public int defaultCapacity = 100;
        public int maxSize = 1000;
    }

    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        [Header("Pool Configurations")]
        [SerializeField] private List<PoolConfig> poolConfigs;

        private Dictionary<string, ObjectPool<GameObject>> pools = new Dictionary<string, ObjectPool<GameObject>>();
        
        private Dictionary<GameObject, string> spawnedObjectsTracker = new Dictionary<GameObject, string>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var config in poolConfigs)
            {
                ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                    createFunc: () => {
                        GameObject obj = Instantiate(config.prefab, transform);
                        obj.SetActive(false);
                        return obj;
                    },
                    actionOnGet: (obj) => obj.SetActive(true),
                    actionOnRelease: (obj) => 
                    {
                        obj.SetActive(false);
                        obj.transform.SetParent(transform); 
                    },
                    actionOnDestroy: (obj) => Destroy(obj),
                    collectionCheck: true,
                    defaultCapacity: config.defaultCapacity,
                    maxSize: config.maxSize
                );

                pools.Add(config.poolId, pool); 
            }
        }

        public GameObject Spawn(string poolId, Vector3 position, Quaternion rotation)
        {
            if (!pools.ContainsKey(poolId))
            {
                Debug.LogError($"[PoolManager] Không tìm thấy Pool có tên: {poolId}");
                return null;
            }

            GameObject obj = pools[poolId].Get();
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            spawnedObjectsTracker[obj] = poolId;

            return obj;
        }

        public void Despawn(GameObject obj)
        {
            if (obj == null) return;

            if (spawnedObjectsTracker.TryGetValue(obj, out string poolId))
            {
                if (obj.activeSelf)
                {
                    pools[poolId].Release(obj);
                }
                spawnedObjectsTracker.Remove(obj);
            }
            else
            {
                Debug.LogWarning($"[PoolManager] Object {obj.name} does not belong to any pool. Destroying it directly.");
                Destroy(obj);
            }
        }
        
        public void DespawnAll()
        {
            List<GameObject> activeObjects = new List<GameObject>(spawnedObjectsTracker.Keys);

            int count = 0;
            foreach (GameObject obj in activeObjects)
            {
                if (obj != null && obj.activeSelf)
                {
                    Despawn(obj);
                    count++;
                }
            }
        }
    }
}