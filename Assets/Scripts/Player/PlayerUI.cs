using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject _pausePanel;
    [SerializeField] Button _resumeButton;
    [SerializeField] Button _mainMenuButton;

    private bool _isPaused;

    private void Start()
    {
        _pausePanel.SetActive(false);
        _resumeButton.onClick.AddListener(() => { ResumeGame(); });
        _mainMenuButton.onClick.AddListener(() => { BackToMainMenu(); });
        Cursor.lockState = CursorLockMode.Locked;
        _isPaused = false;
    }

    private void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ShowHidePanel(_pausePanel);
        }
    }

    private void ResumeGame()
    {
        ShowHidePanel(_pausePanel);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void BackToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
    }

    void ShowHidePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
        _isPaused = !_isPaused;

        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
