using UnityEngine;
using UnityEngine.UI;
using PixelDestruction.Systems;

namespace PixelDestruction.UI
{
    public class XpProgressBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider xpBar;

        private void Start()
        {
            if (xpBar != null) xpBar.value = 0f;

            if (XpManager.Instance != null)
            {
                XpManager.Instance.OnXpProgressChanged += HandleXpProgressChanged;
                XpManager.Instance.OnUpgradeReady += HandleUpgradeReady;
            }
        }

        private void OnDestroy()
        {
            if (XpManager.Instance != null)
            {
                XpManager.Instance.OnXpProgressChanged -= HandleXpProgressChanged;
                XpManager.Instance.OnUpgradeReady -= HandleUpgradeReady;
            }
        }

        private void HandleXpProgressChanged(float progressRatio)
        {
            if (xpBar != null)
            {
                xpBar.value = progressRatio;
            }
        }

        private void HandleUpgradeReady()
        {
            
        }
    }
}
