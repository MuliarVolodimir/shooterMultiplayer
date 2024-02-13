using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionMessageUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _messageTxt;
    [SerializeField] Button _closeUIButton;
    [SerializeField] GameObject _messagePanel;

    void Start()
    {
        Hide();
        _closeUIButton.onClick.AddListener(() => { Hide(); });

        LobbyOnlineManager.Instance.OnCreateLobbyStarted += OnCreateLobbyStarted;
        LobbyOnlineManager.Instance.OnCreateLobbyFailed += OnCreateLobbyFailed;
        LobbyOnlineManager.Instance.OnTryJoin += OnTryJoin;
        LobbyOnlineManager.Instance.OnJoinFailed += OnJoinFailed;
    }

    private void OnJoinFailed()
    {
        ShowMessage("//Faild to Join!//");
    }

    private void OnTryJoin()
    {
        ShowMessage("//Connecting...//");
    }

    private void OnCreateLobbyFailed()
    {
        ShowMessage("//Failed to create Lobby//");
    }

    private void OnCreateLobbyStarted()
    {
        ShowMessage("//Creating Lobby...//");
    }

    private void ShowMessage(string message)
    {
        _messagePanel.SetActive(true);
        _messageTxt.text = message;
    }

    private void Hide()
    {
        _messagePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        LobbyOnlineManager.Instance.OnCreateLobbyStarted -= OnCreateLobbyStarted;
        LobbyOnlineManager.Instance.OnCreateLobbyFailed -= OnCreateLobbyFailed;
        LobbyOnlineManager.Instance.OnTryJoin -= OnTryJoin;
        LobbyOnlineManager.Instance.OnJoinFailed -= OnJoinFailed;
    }
}
