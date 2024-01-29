using Unity.Netcode;
using UnityEngine;

public class MeleeWeapon : NetworkBehaviour, IItem
{
    [Header("WeaponSettings")]
    [SerializeField] float _fireRate = 2;
    [SerializeField] float _weaponRange = 1f;
    [SerializeField] int _damage;

    [SerializeField] Camera _fpsCam;

    [Space(10)]
    [Header("WeaponEffects")]
    [SerializeField] GameObject _muzzlePrefab;
    [SerializeField] float _timeToDestroy;

    private float _nextFire;

    public bool Action()
    {
        if (Time.time >= _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            Attack();

            return true;
        }
        return false;
    }

    private void Attack()
    {
        Vector3 rayStart = _fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        if (Physics.Raycast(rayStart, _fpsCam.transform.forward, out hit, _weaponRange))
        {
            Debug.Log(hit.transform.gameObject.name);
        }
    }
}
