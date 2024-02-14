using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnerSystem : NetworkBehaviour, ISpawnerSystem
{
    public static SpawnerSystem Instance;

    [SerializeField] List<SpawnPoint> _spawnPoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Despawn(GameObject obj)
    {
        int spawnerIndex = Random.Range(0, _spawnPoints.Count);
        _spawnPoints[spawnerIndex].Spawn(obj);
    }
}
