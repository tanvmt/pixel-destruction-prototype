namespace PixelDestruction.Core
{
    public interface IWeapon
    {
        WeaponType GetWeaponType();
        
        void ApplyUpgrade(UpgradeType type, float value);
    }
}
