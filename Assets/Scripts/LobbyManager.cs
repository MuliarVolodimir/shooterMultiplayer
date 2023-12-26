using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    private Lobby _hostLobby;
    private float _lobbyHeartBeat;
    [SerializeField] Button _createLobby;
    [SerializeField] Button _lobbiesList;
    private async void Start()
    {
        _createLobby.onClick.AddListener(() => { CreateLobby(); });
        _lobbiesList.onClick.AddListener(() => { ListLobbies(); });

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        LobbyHeartBeatTimer();
    }
    private async void LobbyHeartBeatTimer()
    {
        if (_hostLobby != null)
        {
            _lobbyHeartBeat -= Time.deltaTime;
            if (_lobbyHeartBeat < 0f)
            {
                float maxHeartBeat = 15;
                _lobbyHeartBeat = maxHeartBeat;

                await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
            }
        }
    }
    private async void CreateLobby()
    {
        try
        {
            string name = "mylobby";
            int maxPlayer = 4;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayer);

            _hostLobby = lobby;

            Debug.Log($"create lobby: {lobby.Name}, {lobby.MaxPlayers}");
        }
        catch (LobbyServiceException ex)
        {

            Debug.Log(ex);
        }
    }
    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log($"Lobbies found:{queryResponse.Results.Count}");
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException ex)
        {

            Debug.Log(ex);
        }
    }
}
