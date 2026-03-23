using UnityEngine;
using System.Collections.Generic;
using PixelDestruction.Systems;

namespace PixelDestruction.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        [Header("UI Structure")]
        [SerializeField] private GameObject upgradePanel; 
        [SerializeField] private Transform cardContainer;
        [SerializeField] private UpgradeCardUI cardPrefab;

        [Header("Game Design Settings")]
        [SerializeField] private int optionsCount = 2;

        private List<UpgradeCardUI> spawnedCards = new List<UpgradeCardUI>();

        private void Start()
        {
            if (upgradePanel != null) upgradePanel.SetActive(false);

            if (XpManager.Instance != null)
            {
                XpManager.Instance.OnUpgradeReady += ShowUpgradeMenu;
            }
        }

        private void OnDestroy()
        {
            if (XpManager.Instance != null)
            {
                XpManager.Instance.OnUpgradeReady -= ShowUpgradeMenu;
            }
        }

        private void ShowUpgradeMenu()
        {
            Time.timeScale = 0f;
            if (upgradePanel != null) upgradePanel.SetActive(true);

            foreach (var card in spawnedCards)
            {
                if (card != null) Destroy(card.gameObject);
            }
            spawnedCards.Clear();

            List<UpgradeData> choices = UpgradeManager.Instance.GetRandomUpgrades(optionsCount);
            
            foreach (var choice in choices)
            {
                UpgradeCardUI newCard = Instantiate(cardPrefab, cardContainer);
                newCard.Setup(choice, this);
                spawnedCards.Add(newCard);
            }
        }

        public void SelectUpgrade(UpgradeData data)
        {
            UpgradeManager.Instance.AddUpgrade(data);
            
            if (upgradePanel != null) upgradePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
