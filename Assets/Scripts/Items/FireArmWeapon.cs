using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

public class FireArmWeapon : NetworkBehaviour, IItem
{
    [Header("WeaponSettings")]
    [SerializeField] float _fireRate = 0.25f;
    [SerializeField] float _weaponRange = 50f;
    [SerializeField] int _damage;

    [Space(10)]
    [Header("Weapon`s Bullet")]
    [SerializeField] Item _bulletItem;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] float _bulletLiveTime;
    [SerializeField] Transform _bulletSpawnPoint;
    [SerializeField] Camera _fpsCam;

    [Space(10)]
    [Header("WeaponEffects")]
    [SerializeField] GameObject _muzzlePrefab;
    [SerializeField] float _timeToDestroy;

    private float _nextFire;

    public void Action()
    {
        if (Time.time >= _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            if (InventorySingletone.Instance.FindExistingItemSlotIndex(_bulletItem) != -1)
            {
                InventorySingletone.Instance.RemoveItem(_bulletItem, 1, true);
                TestShoot();
            } 
            else
            {
                Debug.Log("No Ammo");
            }
        }
    }

    private void TestShoot()
    {
        Vector3 rayStart = _fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        if (Physics.Raycast(rayStart, _fpsCam.transform.forward, out hit, _weaponRange))
        {
            if (hit.transform.GetComponent<Character>())
            {
                var character = hit.transform.GetComponent<Character>();
                character.Takedamage(_damage);
            }
            Debug.Log(hit.transform.gameObject.name);
        }
    }

    public Item GetBulletType()
    {
        if (_bulletItem == null)
        {
            return null;
        }
        return _bulletItem;
    }

    [ServerRpc]
    void ShootLogicServerRpc()
    {
        Vector3 rayStart = _fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        GameObject bullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();

        if (Physics.Raycast(rayStart, _fpsCam.transform.forward, out hit, _weaponRange))
        {
            bullet.transform.DOMove(hit.point, _bulletLiveTime);
        }
        else
        {
            bullet.transform.DOMove(rayStart + (_fpsCam.transform.forward * _weaponRange), _bulletLiveTime);
        }

        SpawnMuzzle();
        Destroy(bullet.gameObject, _bulletLiveTime * 2);
    }

    private void SpawnMuzzle()
    {
        GameObject spawnedMuzzle = Instantiate(_muzzlePrefab, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
        spawnedMuzzle.GetComponent<NetworkObject>().Spawn();

        Destroy(spawnedMuzzle, _timeToDestroy);
    }
}
