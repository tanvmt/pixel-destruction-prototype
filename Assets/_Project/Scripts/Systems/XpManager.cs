using UnityEngine;
using System;

namespace PixelDestruction.Systems
{
    public class XpManager : MonoBehaviour
    {
        public static XpManager Instance { get; private set; }

        public int CurrentXp { get; private set; }
        
        public event Action<int> OnXpChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void AddXp(int amount)
        {
            CurrentXp += amount;
            OnXpChanged?.Invoke(CurrentXp);
            Debug.Log($"[XpManager] Gained {amount} XP! Total XP: {CurrentXp}");
        }
    }
}
