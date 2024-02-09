using TMPro;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _lobbyName;
    [SerializeField] TextMeshProUGUI _lobbyCode;

    private void Start()
    {
        var lobby = LobbyOnlineManager.Instance.GetLobby();
        _lobbyName.text = $"Lobby Name: {lobby.Name}";
        _lobbyCode.text = $"Lobby Code: {lobby.LobbyCode}";
    }
}
