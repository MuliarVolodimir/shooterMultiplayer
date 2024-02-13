using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShooterGameManager : NetworkBehaviour
{
    [SerializeField] GameObject _playerPrefab;

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
    }

    private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (IsServer)
        {
            foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                GameObject player = Instantiate(_playerPrefab);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
                SpawnerSystem.Instance.Despawn(_playerPrefab);
            }
        }
    }

    public override void OnDestroy()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
    }
}
