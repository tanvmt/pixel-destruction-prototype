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

            LevelData levelData = levels[index];
            targetObjectsCount = levelData.requiredObjectsToDestroy;
            objectsProcessedCounter = 0;

            foreach (var obs in levelData.obstacles)
            {
                if (obs.prefab != null)
                    Instantiate(obs.prefab, obs.position, Quaternion.identity, environmentContainer);
            }

            foreach (var slot in levelData.weaponSlots)
            {
                if (slot.prefab != null)
                    Instantiate(slot.prefab, slot.position, Quaternion.identity, environmentContainer);
            }

            StartCoroutine(SpawnObjectsRoutine(levelData));
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
