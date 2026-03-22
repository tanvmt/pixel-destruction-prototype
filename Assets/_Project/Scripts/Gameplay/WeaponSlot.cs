using UnityEngine;

namespace PixelDestruction.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class WeaponSlot : MonoBehaviour
    {
        [Header("Slot Status")]
        [SerializeField] private bool isOccupied = false;
        
        [SerializeField] private GameObject currentWeapon;

        public bool IsOccupied => isOccupied;

        public bool EquipWeapon(GameObject weaponPrefab)
        {
            if (isOccupied || weaponPrefab == null)
            {
                Debug.LogWarning("[WeaponSlot] Slot is occupied or weapon prefab is null");
                return false;
            }

            currentWeapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity, transform);
            isOccupied = true;
            
            return true;
        }

        public void ClearSlot()
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }
            isOccupied = false;
        }
    }
}
