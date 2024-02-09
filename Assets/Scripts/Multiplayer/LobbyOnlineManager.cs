using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Netcode;

public class LobbyOnlineManager : NetworkBehaviour
{

    public static LobbyOnlineManager Instance { get; private set; }

    [Header("Create Lobby")]
    [SerializeField] TMP_InputField _lobbyNameInput;
    [SerializeField] Toggle _isPrivateToggle;
    [SerializeField] Button _createLobbyButton;

    [Space(10)]
    [Header("Quik Join")]
    [SerializeField] Button _quickJoinButton;

    [Space(10)]
    [Header("Join by Code")]
    [SerializeField] TMP_InputField _codeInputField;
    [SerializeField] Button _codeJoinButton;

    [Space(10)]
    [Header("RefreshLobbies")]
    [SerializeField] Button _showAllLobbiesButton;

    [Space(10)]
    [Header("Lobby Prefab")]
    [SerializeField] GameObject _lobbyContent;
    [SerializeField] GameObject _lobbyItemPrefab;


    private Lobby _currentLobby;
    private float _lobbyHeartBeat;
    private bool _isRefreshing;
    private float _nextLobbyUpdate;
    private float _updateRate = 15;
    private string _playerID;

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
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        _createLobbyButton.onClick.AddListener(() => { CreateLobby(); });
        _quickJoinButton.onClick.AddListener(() => { QuickJoinToLobby(); });
        _codeJoinButton.onClick.AddListener(() => { JoinToLobbyByCode(); });
        _showAllLobbiesButton.onClick.AddListener(() => { ShowLobbies(); });

        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            _playerID = AuthenticationService.Instance.PlayerId;
            Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        LobbyHeartBeatTimer();
        RefreshLobbies();
    }

    public string GetPlayerID()
    {
        return _playerID;
    }

    public Lobby GetLobby()
    {
        return _currentLobby;
    }

    private void RefreshLobbies()
    {
        if (Time.time >= _nextLobbyUpdate && AuthenticationService.Instance.IsSignedIn)
        {
            _nextLobbyUpdate = Time.time + _updateRate;
            ShowLobbies();
        }
    }

    private async void LobbyHeartBeatTimer()
    {
        if (CanHeartBeat())
        {
            _lobbyHeartBeat -= Time.deltaTime;
            if (_lobbyHeartBeat < 0f)
            {
                float maxHeartBeat = 15;
                _lobbyHeartBeat = maxHeartBeat;

                await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
                Debug.Log($"{_currentLobby.Name} LobbyHeartBeat, Players: {_currentLobby.Players.Count}/{_currentLobby.MaxPlayers}, LobbyCode {_currentLobby.LobbyCode}");
            }
        }
    }

    private bool CanHeartBeat()
    {
        if (_currentLobby != null && _currentLobby.HostId == _playerID)
        {
            return true;
        }
        return false;
    }

    private async void CreateLobby()
    {
        try
        {
            string name = _lobbyNameInput.text;
            int maxPlayer = 4;

            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = _isPrivateToggle.isOn;

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayer, options);
            _currentLobby = lobby;

            Debug.Log($"create lobby: {lobby.Name}, {lobby.MaxPlayers}, IsPrivate {lobby.IsPrivate}");
            _lobbyNameInput.text = lobby.LobbyCode;

            //NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        catch (LobbyServiceException ex)
        {

            Debug.Log(ex);
        }
    }

    private async void JoinToLobbyByCode()
    {
        try
        {
            string lobbyCode = _codeInputField.text;
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            Debug.Log($"Joined by code {lobby.LobbyCode} to private {lobby.Name} lobby, Players: {lobby.Players.Count}");
            //NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobbyByID(string lobbyID)
    {
        try
        {
            _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);

            Debug.Log($"Joined to {_currentLobby.Name}, {_currentLobby.Players.Count}/{_currentLobby.MaxPlayers}");
            //NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void QuickJoinToLobby()
    {
        try
        {
            // Lobby Filter(gamemode tupe, player count etc.)

            /*QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
            options.Filter = new List<QueryFilter>()
            {
                 new QueryFilter(
                 field: QueryFilter.FieldOptions.MaxPlayers,
                 op: QueryFilter.OpOptions.GE,
                 value: "10")
            };*/

            // Quick-join a random lobby
            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log($"Joined to {lobby.Name}, PlayerCount: {lobby.Players.Count}, LobbyID: {lobby.Id}");
            //NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void ShowLobbies()
    {
        if (_isRefreshing) { return; }

        _isRefreshing = true;

        try
        {
            var options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0")
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            // destroing all lobbies from list
            foreach (Transform child in _lobbyContent.transform)
            {
                Destroy(child.gameObject);
            }
            // adding new lobbies to list
            foreach (Lobby lobby in lobbies.Results)
            {
                var lobbyInstance = Instantiate(_lobbyItemPrefab, _lobbyContent.transform);
                lobbyInstance.GetComponentInChildren<TextMeshProUGUI>().text = $"{lobby.Name}  {lobby.Players.Count}/{lobby.MaxPlayers}";
                lobbyInstance.GetComponentInChildren<Button>().onClick.AddListener(() => JoinLobbyByID(lobby.Id));
            }

            // for debuging
            Debug.Log($"Lobbies found:{lobbies.Results.Count}");
            foreach (Lobby lobby in lobbies.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            _isRefreshing = false;
            throw;
        }

        _isRefreshing = false;
    }
}
