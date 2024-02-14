using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _lobbyName;
    [SerializeField] TextMeshProUGUI _lobbyCode;
    [SerializeField] TextMeshProUGUI _playerCount;

    [SerializeField] GameObject _playerContent;
    [SerializeField] GameObject _playerInfoPrefab;

    [SerializeField] Button _leaveLobbyButton;
    [SerializeField] Button _startGameButton;

    private float _lobbyUpdateTime = 3f;
    private Lobby _currentLobby;

    private void Start()
    {
        _currentLobby = LobbyOnlineManager.Instance.GetLobby();
        _lobbyName.text = $"Lobby Name: {_currentLobby.Name}";
        _lobbyCode.text = $"Lobby Code: {_currentLobby.LobbyCode}";
        _playerCount.text = $"Players: {_currentLobby.Players.Count}/{_currentLobby.MaxPlayers}";

        _leaveLobbyButton.onClick.AddListener(() => { LeaveLobby(); } );

        if (LobbyOnlineManager.Instance.IsHost())
            _startGameButton.onClick.AddListener(() => { StartGame(); });
        else
            Destroy(_startGameButton.gameObject);
        EnterLobby();
    }

    private void Update()
    {
        HandleLobbyUpdate(_playerInfoPrefab, _playerContent);
    }
    private void VisualizeRoomDetails(GameObject playerInfoPrefab, GameObject playerContent)
    {
        for (int i = 0; i < playerContent.transform.childCount; i++)
        {
            Destroy(playerContent.transform.GetChild(i).gameObject);
        }

        if (LobbyOnlineManager.Instance.IsinLobby())
        {
            Lobby lobby = LobbyOnlineManager.Instance.GetLobby();
            var playerID = LobbyOnlineManager.Instance.GetPlayerID();

            foreach (Player player in lobby.Players)
            {
                GameObject newPlayerInfo = Instantiate(playerInfoPrefab, playerContent.transform);
                newPlayerInfo.GetComponentInChildren<TextMeshProUGUI>().text = player.Data["PlayerName"].Value;

                if (LobbyOnlineManager.Instance.IsHost() && player.Id != playerID)
                {
                    Button kickBtn = newPlayerInfo.GetComponentInChildren<Button>(true);

                    kickBtn.onClick.AddListener(() => { LobbyOnlineManager.Instance.KickPlayer(player.Id); });
                    kickBtn.gameObject.SetActive(true);
                }
            }
            _playerCount.text = $"Players: {lobby.Players.Count}/{lobby.MaxPlayers}";
        }
    }

    private async void HandleLobbyUpdate(GameObject playerInfoPrefab, GameObject playerContent)
    {
        if (_currentLobby != null)
        {
            _lobbyUpdateTime -= Time.deltaTime;
            if (_lobbyUpdateTime <= 0)
            {
                _lobbyUpdateTime = 2f;
                try
                {
                    if (LobbyOnlineManager.Instance.IsinLobby())
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

    private void EnterLobby()
    {
        foreach (Player player in LobbyOnlineManager.Instance.GetLobby().Players)
        {
            VisualizeRoomDetails(_playerInfoPrefab, _playerContent);
            Debug.Log("Player: " + player.Data["PlayerName"].Value);
        }
    }

    private void LeaveLobby()
    {
        LobbyOnlineManager.Instance.LeaveLobby();
    }

    private void StartGame()
    {
        SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
    }
}
