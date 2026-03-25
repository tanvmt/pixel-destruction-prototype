using UnityEngine;
using PixelDestruction.Core;
using PixelDestruction.Systems;

namespace PixelDestruction.Gameplay
{
    public class WeaponPlacementManager : MonoBehaviour
    {
        public static WeaponPlacementManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && Time.timeScale > 0f)
            {
                HandlePlacementClick();
            }
        }

        private void HandlePlacementClick()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            Collider2D hit = Physics2D.OverlapPoint(worldPos);
            if (hit != null)
            {
                WeaponSlot slot = hit.GetComponent<WeaponSlot>();
                if (slot != null && !slot.IsOccupied)
                {
                    TryPlaceWeapon(slot);
                }
            }
        }

        private void TryPlaceWeapon(WeaponSlot slot)
        {
            WeaponSlot[] allSlots = FindObjectsOfType<WeaponSlot>();
            int placedWeaponsCount = 0;
            
            foreach (var s in allSlots)
            {
                if (s.IsOccupied) placedWeaponsCount++;
            }

            int maxAllowed = UpgradeManager.Instance.ExtraWeaponCapacity;

            if (placedWeaponsCount < maxAllowed)
            {
                GameObject prefabToBuild = LevelManager.Instance?.CurrentLevelData?.allowedWeaponPrefab;

                if (prefabToBuild == null)
                {
                    Debug.LogWarning("[Placement] Weapon Prefab is empty or not set by LevelData");
                    return;
                }

                bool success = slot.EquipWeapon(prefabToBuild);
                if (success)
                {
                    Debug.Log($"[Placement] Placed successfully! Used {placedWeaponsCount + 1} / {maxAllowed} limit.");
                }
            }
            else
            {
                Debug.LogWarning($"[Placement] You have placed {placedWeaponsCount} weapons. Maximum limit is {maxAllowed}. Need to upgrade to place more!");
            }
        }
    }
}
