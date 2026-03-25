using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PixelDestruction.Gameplay;

namespace PixelDestruction.UI
{
    public class LevelProgressBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider levelSlider;
        [SerializeField] private TextMeshProUGUI levelText;

        private void Start()
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.OnLevelLoaded += UpdateUI;
                LevelManager.Instance.OnObjectProcessed += UpdateUI;
            }
        }

        private void OnDestroy()
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.OnLevelLoaded -= UpdateUI;
                LevelManager.Instance.OnObjectProcessed -= UpdateUI;
            }
        }

        private void UpdateUI(int current, int target)
        {
            if (levelSlider != null)
            {
                levelSlider.value = (float)current / target;
            }

            if (levelText != null && LevelManager.Instance != null)
            {
                levelText.text = $"Level {LevelManager.Instance.CurrentLevelIndex + 1}";
            }
        }
    }
}
