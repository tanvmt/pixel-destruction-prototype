using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelDestruction.Core
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private List<LevelData> levels;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform environmentContainer;

        public LevelData CurrentLevelData { get; private set; }

        private int currentLevelIndex = 0;
        private int objectsProcessedCounter = 0;
        private int targetObjectsCount = 0;

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
            foreach (var objPrefab in data.objectsToSpawn)
            {
                if (objPrefab != null && spawnPoint != null)
                {
                    Instantiate(objPrefab, spawnPoint.position, Quaternion.identity);
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
            CheckWinCondition();
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
