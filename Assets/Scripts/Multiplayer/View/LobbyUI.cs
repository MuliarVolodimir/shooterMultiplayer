using System.Threading.Tasks;
using TMPro;
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
        HandleLobbyUpdate();
    }

    private void HandleLobbyUpdate()
    {
        LobbyOnlineManager.Instance.HandleRoomUpdate(_playerInfoPrefab, _playerContent);
        var lobby = LobbyOnlineManager.Instance.GetLobby();
        _playerCount.text = $"Players: {lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    private void EnterLobby()
    {
        foreach (Player player in LobbyOnlineManager.Instance.GetLobby().Players)
        {
            LobbyOnlineManager.Instance.VisualizeRoomDetails(_playerInfoPrefab, _playerContent);
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
