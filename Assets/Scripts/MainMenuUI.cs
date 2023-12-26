using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button _playButton;
    [SerializeField] Button _mainMenuButton;
    [SerializeField] Button _settingsbutton;
    [SerializeField] Button _closeSettingsButton;
    [SerializeField] Button _quitbutton;

    [SerializeField] GameObject _lobbyPanel;
    [SerializeField] GameObject _settingsPanel;

    private void Start()
    {
        _lobbyPanel.SetActive(false);
        _settingsPanel.SetActive(false);

        _playButton.onClick.AddListener(ShowHideLobby);
        _mainMenuButton.onClick.AddListener(ShowHideLobby);

        _settingsbutton.onClick.AddListener(ShowHideSettings);
        _closeSettingsButton.onClick.AddListener(ShowHideSettings);

        _quitbutton.onClick.AddListener(Quit);
    }
    void ShowHideLobby()
    {
        _lobbyPanel.SetActive(!_lobbyPanel.activeSelf);
    }
    void ShowHideSettings()
    {
        _settingsPanel.SetActive(!_settingsPanel.activeSelf);
    }
    void Quit()
    {
        Application.Quit();
    }

}
