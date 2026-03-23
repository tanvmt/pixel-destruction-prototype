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

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void AddUpgrade(UpgradeData newUpgrade)
        {
            if (newUpgrade == null) return;
            
            appliedUpgrades.Add(newUpgrade);
            Debug.Log($"[UpgradeManager] Đã nhận Nâng cấp: {newUpgrade.upgradeName}");

            if (newUpgrade.upgradeType == UpgradeType.ExtraWeaponCapacity)
            {
                ExtraWeaponCapacity += Mathf.RoundToInt(newUpgrade.value);
                Debug.Log($"[UpgradeManager] Số lần được xây vũ khí tăng lên. Giới hạn cộng thêm: {ExtraWeaponCapacity}");
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
            Debug.Log("[UpgradeManager] Đã Reset toàn bộ Nâng Cấp cho màn chơi mới.");
        }
    }
}
