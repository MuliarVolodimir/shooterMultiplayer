using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]

public class Item : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public GameObject itemPrefab;
    
    public WeaponType weaponType;
    public AmmoType ammoType;
    public int magazineAmmoCount;


    public enum ItemType { weapon, healingitem, ammo }
    public enum WeaponType { meleeWeapon, pistol, assaultRifle, sniper, shotGun }
    public enum AmmoType { noAmmo, pistolAmmo, assaultRifleAmmo, sniperAmmo, shotGunAmmo }
}
