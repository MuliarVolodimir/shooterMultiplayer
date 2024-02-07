using Unity.Netcode;
using UnityEngine;

public class SpawnPoint : NetworkBehaviour
{
    [SerializeField] GameObject _playerPrefab;

    public void Spawn(GameObject obj)
    {
        obj.transform.position = transform.position;
    }
}
