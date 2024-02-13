using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlineLobbyMenuUI : MonoBehaviour
{
    [Header("Create Lobby")]
    [SerializeField] TMP_InputField _lobbyNameInput;
    [SerializeField] Toggle _isPrivateToggle;
    [SerializeField] Button _createLobbyButton;

    [Space(10)]
    [Header("Quik Join")]
    [SerializeField] Button _quickJoinButton;

    [Space(10)]
    [Header("Join by Code")]
    [SerializeField] TMP_InputField _joinCodeInputField;
    [SerializeField] Button _codeJoinButton;

    [Space(10)]
    [Header("RefreshLobbies")]
    [SerializeField] Button _showAllLobbiesButton;

    [Space(10)]
    [Header("Lobby Prefab")]
    [SerializeField] GameObject _lobbyContent;
    [SerializeField] GameObject _lobbyItemPrefab;

    [Space(10)]
    [SerializeField] Button _mainMenuButton;


    private void Start()
    {
        _createLobbyButton.onClick.AddListener(() => { CreateLobby(_lobbyNameInput.text, 4, _isPrivateToggle.isOn); });
        _quickJoinButton.onClick.AddListener(() => { QuickJoinToLobby(); });
        _codeJoinButton.onClick.AddListener(() => { JoinToLobbyByCode(_joinCodeInputField.text); });
        _showAllLobbiesButton.onClick.AddListener(() => { RefreshLobby(); });
        _mainMenuButton.onClick.AddListener(() => { BackToMainMenu(); });
    }

    private void BackToMainMenu()
    {
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
    }

    private void Update()
    {
        RefreshLobby();
    }

    private void CreateLobby(string tlobbyName, int playerCount, bool isPrivate)
    {
        LobbyOnlineManager.Instance.CreateLobby(tlobbyName, playerCount, isPrivate);
    }

    private void RefreshLobby()
    {
        LobbyOnlineManager.Instance.RefreshLobbies(_lobbyItemPrefab, _lobbyContent);
    }

    private void JoinToLobbyByCode(string joinCode)
    {
        LobbyOnlineManager.Instance.JoinToLobbyByCode(joinCode);
    }

    private void QuickJoinToLobby()
    {
         LobbyOnlineManager.Instance.QuickJoinToLobby();
    }

}
