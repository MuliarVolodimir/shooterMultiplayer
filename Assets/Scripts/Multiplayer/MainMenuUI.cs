using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button _localGameButton;
    [SerializeField] Button _onlineGameButton;
    [SerializeField] Button _mainMenuButton;
    [SerializeField] Button _settingsbutton;
    [SerializeField] Button _closeSettingsButton;
    [SerializeField] Button _quitbutton;

    [SerializeField] List<Button> _closeButtons;

    [SerializeField] GameObject _onlineGamePanel;
    [SerializeField] GameObject _localGamePanel;
    [SerializeField] GameObject _settingsPanel;

    private void Start()
    {
        InitCloseButtons();

        _localGameButton.onClick.AddListener(() => { ShowHidePanel(_localGamePanel); });
        _onlineGameButton.onClick.AddListener(() => { ShowHidePanel(_onlineGamePanel); });
        _settingsbutton.onClick.AddListener(() => { ShowHidePanel(_settingsPanel); });

        _quitbutton.onClick.AddListener(Quit);

        HideAllPanels();
    }

    private void InitCloseButtons()
    {
        for (int i = 0; i < _closeButtons.Count; i++)
        {
            _closeButtons[i].onClick.AddListener(HideAllPanels);
        }
    }

    private void HideAllPanels()
    {
        _onlineGamePanel.SetActive(false);
        _localGamePanel.SetActive(false);
        _settingsPanel.SetActive(false);
    }

    void ShowHidePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    void Quit()
    {
        Application.Quit();
    }

}
