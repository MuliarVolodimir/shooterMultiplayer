using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField] float _fireRate = 0.25f;
    [SerializeField] float _weaponRange = 50f;

    [SerializeField] Transform _bulletSpawnPoint;
    [SerializeField] TrailRenderer _trail;
    [SerializeField] Camera _fpsCam;

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
                ShootLogic();

                ShootLogicServerRpc();
            }
        }
    }
    [ServerRpc]
    void ShootLogicServerRpc()
    {
        Vector3 rayOrigin = _fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        TrailRenderer trail = Instantiate(_trail);

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
        TrailRenderer trail = NetworkManager.SpawnManager.SpawnedObjects[trailId].GetComponent<TrailRenderer>();

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
    void ShootLogic()
    {
        RaycastHit hit;

        TrailRenderer trail = Instantiate(_trail);
        Vector3 rayOrigin = _fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        trail.transform.position = _bulletSpawnPoint.position;

        if (Physics.Raycast(rayOrigin, _fpsCam.transform.forward, out hit, _weaponRange))
        {
            StartCoroutine(MoveTrail(trail, hit.point));
        }
        else
        {
            StartCoroutine(MoveTrail(trail, rayOrigin + (_fpsCam.transform.forward * _weaponRange)));
        }
        SpawnMuzzle();
    }
    private void SpawnMuzzle()
    {
        GameObject spawnedMuzzle = Instantiate(_muzzlePrefab, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
        spawnedMuzzle.transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
        Destroy(spawnedMuzzle, _timeToDestroy);
    }
    IEnumerator MoveTrail(TrailRenderer trail, Vector3 targetPosition)
    {
        float time = 0f;
        float duration = trail.time;

        while (time < duration)
        {
            trail.transform.position = Vector3.Lerp(_bulletSpawnPoint.position, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Destroy(trail.gameObject, duration);
    }
}