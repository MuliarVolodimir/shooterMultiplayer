using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

public class Weapon : NetworkBehaviour
{
    [Header("WeaponSettings")]
    [SerializeField] float _fireRate = 0.25f;
    [SerializeField] float _weaponRange = 50f;

    [Space(10)]
    [Header("Weapon`s Bullet")]
    [SerializeField] GameObject _bullet;
    [SerializeField] float _bulletLiveTime;
    [SerializeField] Transform _bulletSpawnPoint;
    [SerializeField] Camera _fpsCam;

    [Space(10)]
    [Header("WeaponEffects")]
    [SerializeField] GameObject _muzzlePrefab;
    [SerializeField] private float _timeToDestroy;

    private float _nextFire;

    void Update()
    {
        //Shoot();
    }

    private void Shoot()
    {
        if (Time.time >= _nextFire)
        {
            _nextFire = Time.time + _fireRate;

            if (Input.GetButton("Fire1"))
            {
                ShootLogicServerRpc();
            }
        }
    }

    [ServerRpc]
    void ShootLogicServerRpc()
    {
        Vector3 rayStart = _fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        GameObject bullet = Instantiate(_bullet, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
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