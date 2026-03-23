using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PixelDestruction.Systems;

namespace PixelDestruction.UI
{
    public class UpgradeCardUI : MonoBehaviour
    {
        [Header("UI Data Binding")]
        [SerializeField] private Button selectButton;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private UpgradeData _data;
        private UpgradeUI _parentUI;

        public void Setup(UpgradeData data, UpgradeUI parentUI)
        {
            _data = data;
            _parentUI = parentUI;

            if (titleText != null) titleText.text = data.upgradeName;
            if (descriptionText != null) descriptionText.text = data.description;
            if (iconImage != null && data.icon != null) iconImage.sprite = data.icon;

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(OnClicked);
            }
        }

        private void OnClicked()
        {
            _parentUI.SelectUpgrade(_data);
        }
    }
}
