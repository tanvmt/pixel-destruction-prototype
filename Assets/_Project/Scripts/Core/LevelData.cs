using UnityEngine;
using System.Collections.Generic;

namespace PixelDestruction.Core
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "PixelDestruction/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Spawn Settings")]
        [Tooltip("List of objects to drop from the top.")]
        public List<GameObject> objectsToSpawn;
        public float spawnDelay = 2f;

        [Header("Win Condition")]
        [Tooltip("The total number of objects that need to be completely destroyed and touch the bottom.")]
        public int requiredObjectsToDestroy;
        
        [System.Serializable]
        public class PositionData 
        {
            public Vector2 position;
            public GameObject prefab;
        }

        [Header("Layout Data")]
        public List<PositionData> obstacles;
        public List<PositionData> weaponSlots;
    }
}
