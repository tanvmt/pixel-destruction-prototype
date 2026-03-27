using UnityEngine;
using System;

namespace PixelDestruction.Systems
{
    public class XpManager : MonoBehaviour
    {
        public static XpManager Instance { get; private set; }

        [Header("XP Thresholds")]
        [SerializeField] private int baseRequiredXp = 100;
        [SerializeField] private float xpMultiplierPerUpgrade = 1.5f;

        public int CurrentXp { get; private set; }
        public int RequiredXp { get; private set; }

        public event Action<float> OnXpProgressChanged;
        public event Action OnUpgradeReady;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            RequiredXp = baseRequiredXp;
        }

        public void AddXp(int amount)
        {
            CurrentXp += amount;

            while (CurrentXp >= RequiredXp)
            {
                CurrentXp -= RequiredXp;
                TriggerUpgrade();
            }

            float progress = (float)CurrentXp / RequiredXp;
            Debug.Log($"[XpManager] Progress: {progress}, CurrentXp: {CurrentXp}, RequiredXp: {RequiredXp}");
            OnXpProgressChanged?.Invoke(progress);
        }

        private void TriggerUpgrade()
        {
            RequiredXp = Mathf.RoundToInt(RequiredXp * xpMultiplierPerUpgrade);

            OnUpgradeReady?.Invoke();
        }
        
        public void ResetXp()
        {
            CurrentXp = 0;
            RequiredXp = baseRequiredXp;
            OnXpProgressChanged?.Invoke(0f);
        }
    }
}
