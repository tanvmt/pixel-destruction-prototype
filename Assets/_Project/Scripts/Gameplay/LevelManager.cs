using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PixelDestruction.Core;
using System;

namespace PixelDestruction.Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private List<LevelData> levels;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform environmentContainer;

        public LevelData CurrentLevelData { get; private set; }
        public int CurrentLevelIndex => currentLevelIndex;

        public event Action<int, int> OnLevelLoaded;
        public event Action<int, int> OnObjectProcessed;

        private int currentLevelIndex = 0;
        private int objectsProcessedCounter = 0;
        private int objectsSpawnedCounter = 0;
        private int targetObjectsCount = 0;

        private int currentSpawnId = 0;
        private Dictionary<int, int> activeSpawnsTracker = new Dictionary<int, int>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
            LoadLevel(0);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameStateChanged -= GameManager_OnGameStateChanged;
        }

        private void GameManager_OnGameStateChanged(GameState state)
        {
            if (state == GameState.Playing)
            {
                LoadLevel(currentLevelIndex);
            }
        }

        public void LoadLevel(int index)
        {
            if (index < 0 || index >= levels.Count) return;

            ClearCurrentLevel();

            CurrentLevelData = levels[index];
            targetObjectsCount = CurrentLevelData.requiredObjectsToDestroy;
            objectsProcessedCounter = 0;
            objectsSpawnedCounter = 0;
            
            currentSpawnId = 0;
            activeSpawnsTracker.Clear();

            OnLevelLoaded?.Invoke(0, targetObjectsCount);

            foreach (var obs in CurrentLevelData.obstacles)
            {
                if (obs.prefab != null)
                {
                    GameObject spawned = Instantiate(obs.prefab, environmentContainer);
                    spawned.transform.localPosition = new Vector3(obs.position.x, obs.position.y, 0f);
                }
            }

            StartCoroutine(SpawnObjectsRoutine(CurrentLevelData));
        }

        private IEnumerator SpawnObjectsRoutine(LevelData data)
        {
            foreach (var texture in data.texturesToSpawn)
            {
                if (data.maxConcurrentObjects > 0)
                {
                    while (objectsSpawnedCounter - objectsProcessedCounter >= data.maxConcurrentObjects)
                    {
                        yield return null;
                    }
                }

                if (texture != null && spawnPoint != null && PixelSpawner.Instance != null)
                {
                    PixelSpawner.Instance.SpawnObjectFromTexture(texture, spawnPoint.position);
                    objectsSpawnedCounter++;
                }
                yield return new WaitForSeconds(data.spawnDelay);
            }
        }

        private void ClearCurrentLevel()
        {
            if (environmentContainer != null)
            {
                foreach (Transform child in environmentContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void RegisterObjectProcessed()
        {
            objectsProcessedCounter++;
            OnObjectProcessed?.Invoke(objectsProcessedCounter, targetObjectsCount);
            CheckWinCondition();
        }

        public int GetNewSpawnId()
        {
            currentSpawnId++;
            activeSpawnsTracker[currentSpawnId] = 1;
            return currentSpawnId;
        }

        public void RegisterFragment(int spawnId)
        {
            if (activeSpawnsTracker.ContainsKey(spawnId))
            {
                activeSpawnsTracker[spawnId]++;
            }
        }

        public void UnregisterFragment(int spawnId)
        {
            if (activeSpawnsTracker.ContainsKey(spawnId))
            {
                activeSpawnsTracker[spawnId]--;

                if (activeSpawnsTracker[spawnId] <= 0)
                {
                    activeSpawnsTracker.Remove(spawnId);
                    RegisterObjectProcessed();
                }
            }
        }

        private void CheckWinCondition()
        {
            if (objectsProcessedCounter >= targetObjectsCount)
            {
                GameManager.Instance.UpdateGameState(GameState.LevelComplete);
            }
        }

        public void NextLevel()
        {
            currentLevelIndex++;
            GameManager.Instance.UpdateGameState(GameState.Playing);
        }
    }
}
