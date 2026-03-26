using UnityEngine;
using System.Collections.Generic;

namespace PixelDestruction.Core
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "PixelDestruction/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Spawn Settings")]
        public Vector3 spawnPosition = new Vector3(0, 8, 0);
        [Tooltip("Maximum number of objects allowed to exist on screen at the same time. Helps prevent game lag. (0 = No limit)")]
        public int maxConcurrentObjects = 3;
        [Tooltip("List of objects to drop from the top.")]
        public List<Texture2D> texturesToSpawn;
        public float spawnDelay = 2f;

        [Header("Win Condition")]
        [Tooltip("The total number of objects that need to be completely destroyed and touch the bottom.")]
        public int requiredObjectsToDestroy;
        
        [System.Serializable]
        public class ObjectData 
        {
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
            public GameObject prefab;
        }

        [Header("Layout Data")]
        public List<ObjectData> obstacles;
        public List<ObjectData> walls;

        [Header("Weapon Placement")]
        public GameObject allowedWeaponPrefab;
    }
}
