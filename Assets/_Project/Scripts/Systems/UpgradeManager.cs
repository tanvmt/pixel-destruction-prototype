using UnityEngine;
using System.Collections.Generic;
using PixelDestruction.Core;

namespace PixelDestruction.Systems
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        private List<UpgradeData> appliedUpgrades = new List<UpgradeData>();
        private List<IWeapon> activeWeapons = new List<IWeapon>();

        public int ExtraWeaponCapacity { get; private set; } = 0;

        [Header("Upgrade Pool")]
        [SerializeField] private List<UpgradeData> upgradePool = new List<UpgradeData>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void AddUpgrade(UpgradeData newUpgrade)
        {
            if (newUpgrade == null) return;
            
            appliedUpgrades.Add(newUpgrade);

            if (newUpgrade.upgradeType == UpgradeType.ExtraWeaponCapacity)
            {
                ExtraWeaponCapacity += Mathf.RoundToInt(newUpgrade.value);
                Debug.Log($"[UpgradeManager] Extra weapon capacity increased. Total extra capacity: {ExtraWeaponCapacity}");
                return;
            }

            foreach (var weapon in activeWeapons)
            {
                if (weapon.GetWeaponType() == newUpgrade.targetWeapon)
                {
                    weapon.ApplyUpgrade(newUpgrade.upgradeType, newUpgrade.value);
                }
            }
        }

        public List<UpgradeData> GetRandomUpgrades(int count)
        {
            if (upgradePool.Count == 0) return new List<UpgradeData>();

            if (activeWeapons.Count == 0)
            {
                UpgradeData forceCard = upgradePool.Find(upg => upg.upgradeType == UpgradeType.ExtraWeaponCapacity);
                if (forceCard != null)
                {
                    Debug.Log("[UpgradeManager] No active weapons on map! Forcing the appearance of an Extra Weapon Capacity card.");
                    return new List<UpgradeData> { forceCard };
                }
            }
            
            List<UpgradeData> copy = new List<UpgradeData>(upgradePool);

            int totalSlotsOnMap = FindObjectsOfType<WeaponSlot>().Length;

            if (ExtraWeaponCapacity >= totalSlotsOnMap)
            {
                copy.RemoveAll(upg => upg.upgradeType == UpgradeType.ExtraWeaponCapacity);
                Debug.Log("[UpgradeManager] Max physical weapon slots reached. Removed Extra Weapon Capacity card from pool!");
            }

            for (int i = 0; i < copy.Count; i++)
            {
                UpgradeData temp = copy[i];
                int randomIndex = UnityEngine.Random.Range(i, copy.Count);
                copy[i] = copy[randomIndex];
                copy[randomIndex] = temp;
            }

            return copy.GetRange(0, Mathf.Min(count, copy.Count));
        }

        public void RegisterWeapon(IWeapon newWeapon)
        {
            if (!activeWeapons.Contains(newWeapon))
            {
                activeWeapons.Add(newWeapon);
                
                foreach (var upg in appliedUpgrades)
                {
                    if (upg.upgradeType == UpgradeType.ExtraWeaponCapacity) continue;

                    if (newWeapon.GetWeaponType() == upg.targetWeapon)
                    {
                        newWeapon.ApplyUpgrade(upg.upgradeType, upg.value);
                    }
                }
            }
        }

        public void UnregisterWeapon(IWeapon weapon)
        {
            if (activeWeapons.Contains(weapon))
            {
                activeWeapons.Remove(weapon);
            }
        }

        public void ResetAllUpgrades()
        {
            appliedUpgrades.Clear();
            activeWeapons.Clear();
            ExtraWeaponCapacity = 0;
            Debug.Log("[UpgradeManager] Successfully reset all upgrades and capacity for the new level.");
        }
    }
}
