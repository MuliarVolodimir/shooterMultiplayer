using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [Header("WeaponSettings")]

    [SerializeField] float _fireRate = 0.25f;
    [SerializeField] float _weaponRange = 50f;
    [SerializeField] Transform _bulletSpawnPoint;
    [SerializeField] Camera _fpsCam;

    [Space(10)]
    [Header("WeaponEffects")]

    [SerializeField] TrailRenderer _trail;
    [SerializeField] GameObject _muzzlePrefab;
    [SerializeField] private float _scaleFactor;
    [SerializeField] private float _timeToDestroy;

    private float _nextFire;

    void Update()
    {
        Shoot();
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
        Shootsss();   
    }
    void Shootsss()
    {
        Vector3 rayOrigin = _fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        TrailRenderer trail = Instantiate(_trail, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);

        NetworkObject trailNetObj = trail.GetComponent<NetworkObject>();
        trailNetObj.Spawn();

        if (Physics.Raycast(rayOrigin, _fpsCam.transform.forward, out hit, _weaponRange))
        {
            MoveTrailServerRpc(trailNetObj.NetworkObjectId, _bulletSpawnPoint.position, hit.point);
        }
        else
        {
            MoveTrailServerRpc(trailNetObj.NetworkObjectId, _bulletSpawnPoint.position, rayOrigin + (_fpsCam.transform.forward * _weaponRange));
        }
        SpawnMuzzle();
    }
    [ServerRpc]
    void MoveTrailServerRpc(ulong trailId, Vector3 start, Vector3 end)
    {
        StartCoroutine(MoveTrailServer(trailId, start, end));
    }
    IEnumerator MoveTrailServer(ulong trailId, Vector3 start, Vector3 end)
    {
        TrailRenderer trail = NetworkManager.SpawnManager.SpawnedObjects[trailId].GetComponentInChildren<TrailRenderer>();

        float time = 0f;
        float duration = trail.time;

        while (time < duration)
        {
            trail.transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(trail.gameObject, duration);
    }
    private void SpawnMuzzle()
    {
        GameObject spawnedMuzzle = Instantiate(_muzzlePrefab, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
        spawnedMuzzle.GetComponent<NetworkObject>().Spawn();

        Destroy(spawnedMuzzle, _timeToDestroy);
    }
}