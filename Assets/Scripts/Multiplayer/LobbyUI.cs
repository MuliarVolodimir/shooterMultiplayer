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

    private float _LobbyUpdateTimer = 2f;
    private void Start()
    {
        var lobby = LobbyOnlineManager.Instance.GetLobby();
        _lobbyName.text = $"Lobby Name: {lobby.Name}";
        _lobbyCode.text = $"Lobby Code: {lobby.LobbyCode}";
        _playerCount.text = $"Players: {lobby.Players.Count}/{lobby.MaxPlayers}";

        _leaveLobbyButton.onClick.AddListener(() => { LeaveLobby(); } );
        _startGameButton.onClick.AddListener(() => { StartGame(); } );
        EnterLobby();
    }

    private void Update()
    {
        HandleRoomUpdate();
    }

    private void EnterLobby()
    {
        foreach (Player player in LobbyOnlineManager.Instance.GetLobby().Players)
        {
            Debug.Log("Player: " + player.Data["PlayerName"].Value);
        }
        VisualizeRoomDetails();
    }

    private void LeaveLobby()
    {
        LobbyOnlineManager.Instance.LeaveLobby();
    }

    private void StartGame()
    {
        SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
    }

    private void VisualizeRoomDetails()
    {
        for (int i = 0; i < _playerContent.transform.childCount; i++)
        {
            Destroy(_playerContent.transform.GetChild(i).gameObject);
        }
        if (LobbyOnlineManager.Instance.IsinLobby())
        {
            Lobby currentLobby = LobbyOnlineManager.Instance.GetLobby();
            var playerID = LobbyOnlineManager.Instance.GetPlayerID();
            foreach (Player player in currentLobby.Players)
            {
                GameObject newPlayerInfo = Instantiate(_playerInfoPrefab, _playerContent.transform);
                newPlayerInfo.GetComponentInChildren<TextMeshProUGUI>().text = player.Data["PlayerName"].Value;

                if (LobbyOnlineManager.Instance.IsHost() && player.Id != playerID)
                {
                    Button kickBtn = newPlayerInfo.GetComponentInChildren<Button>(true);
                    kickBtn.onClick.AddListener(() => { LobbyOnlineManager.Instance.KickPlayer(player.Id); });
                    kickBtn.gameObject.SetActive(true);
                }
            }
        }
    }

    private async void HandleRoomUpdate()
    {
        Lobby currentLobby = LobbyOnlineManager.Instance.GetLobby();

        if (currentLobby != null)
        {
            _LobbyUpdateTimer -= Time.deltaTime;
            if (_LobbyUpdateTimer <= 0)
            {
                _LobbyUpdateTimer = 2f;
                try
                {
                    if (LobbyOnlineManager.Instance.IsinLobby())
                    {
                        currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                        VisualizeRoomDetails();
                    }

                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                    if ((e.Reason == LobbyExceptionReason.Forbidden || e.Reason == LobbyExceptionReason.LobbyNotFound))
                    {
                        currentLobby = null;
                        LobbyOnlineManager.Instance.LeaveLobby();
                    }
                }
            }
        }

    }
}
