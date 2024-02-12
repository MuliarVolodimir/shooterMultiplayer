using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System;

public class LobbyOnlineManager : NetworkBehaviour
{

    public static LobbyOnlineManager Instance { get; private set; }

    private const string KEY_RELAYL_JOIN_CODE = "KEY_RELAY_JOIN";

    private Lobby _currentLobby;
    private float _lobbyHeartBeat;
    private bool _isRefreshing;
    private float _nextLobbyUpdate;
    private float _updateRate = 15;
    private string _playerID;

    private int _playerCount = 4;
    private float _lobbyUpdateTime = 4;

    public event Action OnCreateLobbyStarted;
    public event Action OnCreateLobbyFailed;
    public event Action OnTryJoin;
    public event Action OnJoinFailed;


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
    }

    //Private voids
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

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_playerCount - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
        return default;
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation join = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return join;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async void JoinLobbyByID(string lobbyID)
    {
        OnTryJoin?.Invoke();
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
            {
                Player = GetPlayer()
            };
            var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, options);
            _currentLobby = lobby;

            string relayJoinKey = lobby.Data[KEY_RELAYL_JOIN_CODE].Value;
            JoinAllocation join = await JoinRelay(relayJoinKey);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(join, "dtls"));

            NetworkManager.Singleton.StartClient();
            //SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyScene);

            Debug.Log($"Joined to {lobby.Name}, {lobby.Players.Count}/{lobby.MaxPlayers}");
        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke();
            Debug.Log(e);
        }
    }

    private async void ShowLobbies(GameObject lobbyItemPrefab, GameObject lobbyContent)
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
            foreach (Transform child in lobbyContent.transform)
            {
                Destroy(child.gameObject);
            }
            // adding new lobbies to list
            foreach (Lobby lobby in lobbies.Results)
            {
                var lobbyInstance = Instantiate(lobbyItemPrefab, lobbyContent.transform);
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

    private Player GetPlayer()
    {
        string playerName = PlayerPrefs.GetString("Name");
        if (playerName == null || playerName == "")
        {
            playerName = _playerID;
        }

        Player player = new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {"PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName) }
            }
        };

        return player;
    }

    //Public voids
    public string GetPlayerID()
    {
        return _playerID;
    }

    public Lobby GetLobby()
    {
        return _currentLobby;
    }

    public void RefreshLobbies(GameObject lobbyItemPrefab, GameObject lobbyContent)
    {
        if (Time.time >= _nextLobbyUpdate && AuthenticationService.Instance.IsSignedIn)
        {
            _nextLobbyUpdate = Time.time + _updateRate;
            ShowLobbies(lobbyItemPrefab, lobbyContent);
        }
    }

    public void VisualizeRoomDetails(GameObject playerInfoPrefab, GameObject playerContent)
    {
        for (int i = 0; i < playerContent.transform.childCount; i++)
        {
            Destroy(playerContent.transform.GetChild(i).gameObject);
        }

        if (Instance.IsinLobby())
        {
            Lobby currentLobby = GetLobby();
            var playerID = GetPlayerID();
            foreach (Player player in currentLobby.Players)
            {
                GameObject newPlayerInfo = Instantiate(playerInfoPrefab, playerContent.transform);
                newPlayerInfo.GetComponentInChildren<TextMeshProUGUI>().text = player.Data["PlayerName"].Value;

                if (Instance.IsHost() && player.Id != playerID)
                {
                    Button kickBtn = newPlayerInfo.GetComponentInChildren<Button>(true);

                    kickBtn.onClick.AddListener(() => { KickPlayer(player.Id); });
                    kickBtn.gameObject.SetActive(true);
                }
            }
        }
    }

    public async void HandleRoomUpdate(GameObject playerInfoPrefab, GameObject playerContent)
    {
        if (_currentLobby != null)
        {
            _lobbyUpdateTime -= Time.deltaTime;
            if (_lobbyUpdateTime <= 0)
            {
                _lobbyUpdateTime = 2f;
                try
                {
                    if (IsinLobby())
                    {
                        _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
                        VisualizeRoomDetails(playerInfoPrefab, playerContent);
                    }
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                    if ((e.Reason == LobbyExceptionReason.Forbidden || e.Reason == LobbyExceptionReason.LobbyNotFound))
                    {
                        LeaveLobby();
                    }
                }
            }
        }
    }

    public async void CreateLobby(string lobbyName, int MaxPlayer, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke();
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = GetPlayer()  
            };
            options.IsPrivate = isPrivate;

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MaxPlayer, options);
            _currentLobby = lobby;

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions 
            {
                Data = new Dictionary<string, DataObject> 
                {
                    {KEY_RELAYL_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            Debug.Log($"create lobby: {lobby.Name}, {lobby.MaxPlayers}, IsPrivate {lobby.IsPrivate}");
            lobbyName = lobby.LobbyCode;

            NetworkManager.Singleton.StartHost();
            SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyScene);
        }
        catch (LobbyServiceException ex)
        {
            OnCreateLobbyFailed?.Invoke();
            Debug.Log(ex);
        }
    }

    public async void JoinToLobbyByCode(string joinCode)
    {
        OnTryJoin?.Invoke();
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions()
            {
                Player = GetPlayer()
            };
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode, options);
            _currentLobby = lobby;

            string relayJoinKey = lobby.Data[KEY_RELAYL_JOIN_CODE].Value;
            JoinAllocation join = await JoinRelay(relayJoinKey);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(join, "dtls"));
            NetworkManager.Singleton.StartClient();

            Debug.Log($"Joined by code {lobby.LobbyCode} to private {lobby.Name} lobby, Players: {lobby.Players.Count}");
        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke();
            Debug.Log(e);
        }
    }

    public async void QuickJoinToLobby()
    {
        OnTryJoin?.Invoke();
        try
        {
            // Quick-join a random lobby
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions()
            {
                Player = GetPlayer()
            };
            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            _currentLobby = lobby;

            string relayJoinKey = lobby.Data[KEY_RELAYL_JOIN_CODE].Value;
            JoinAllocation join = await JoinRelay(relayJoinKey);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(join, "dtls"));
            NetworkManager.Singleton.StartClient();

            Debug.Log($"Joined to {lobby.Name}, PlayerCount: {lobby.Players.Count}, LobbyID: {lobby.Id}");
        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke();
            Debug.Log("failed to quick conection");
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, _playerID);
            if (_currentLobby.Players.Count <= 0)
            {
                await LobbyService.Instance.DeleteLobbyAsync(_currentLobby.Id);
                _currentLobby = null;
            }

            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer(string playerID)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, playerID);
            _currentLobby = null;

            if (!IsHost())
            {
                NetworkManager.Singleton.Shutdown();
                SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
            }     
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void SetPlayerName(TMP_InputField playerNameIF)
    {
        playerNameIF.onValueChanged.AddListener(delegate
        {
            PlayerPrefs.SetString("Name", playerNameIF.text);
        });

        playerNameIF.text = PlayerPrefs.GetString("Name");
    }

    public bool IsinLobby()
    {
        foreach (Player _player in _currentLobby.Players)
        {
            if (_player.Id == _playerID)
            {
                return true;
            }
        }
        _currentLobby = null;
        return false;
    }

    public new bool IsHost()
    {
        if (_currentLobby != null && _currentLobby.HostId == _playerID)
        {
            return true;
        }
        return false;
    }
}
