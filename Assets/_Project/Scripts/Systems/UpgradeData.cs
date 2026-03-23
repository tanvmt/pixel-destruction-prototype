using UnityEngine;
using PixelDestruction.Core;

namespace PixelDestruction.Systems
{
    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "PixelDestruction/UpgradeData")]
    public class UpgradeData : ScriptableObject
    {
        [Header("UI Info")]
        public string upgradeName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Upgrade Logic")]
        public WeaponType targetWeapon;
        public UpgradeType upgradeType;
        
        public float value;
    }
}
