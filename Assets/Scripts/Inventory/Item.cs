using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]

public class Item : ScriptableObject
{
    public string _name;
    public ItemType _itemType;
    public GameObject _itemPrefab;
    
    public WeaponType _weaponType;
    public AmmoType _ammoType;
    public int _magazineAmmoCount;


    public enum ItemType { weapon, healingitem, ammo, other }
    public enum WeaponType { meleeWeapon, pistol, assaultRifle, sniper, shootGun }
    public enum AmmoType { pistolAmmo, assaultRifleAmmo, sniperAmmo, shootGunAmmo }
}
