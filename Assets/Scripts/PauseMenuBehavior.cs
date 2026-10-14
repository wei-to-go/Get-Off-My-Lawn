using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    public GameObject PauseMenuPanel;
    public GameObject SettingsPanel;
    public GameObject InfoPanel;
    bool isGamePaused;

    void Start()
    {
        isGamePaused = false;
        if (PauseMenuPanel != null) PauseMenuPanel.SetActive(false);
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
        if (InfoPanel != null) InfoPanel.SetActive(false);
    }

    void Update()
    {
        if (!LevelManager.isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            if (isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        LevelManager.isPlaying = true;
        isGamePaused = false;
        PauseMenuPanel.SetActive(false);
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
        if (InfoPanel != null) InfoPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        LevelManager.isPlaying = false;
        isGamePaused = true;
        PauseMenuPanel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OpenSettings()
    {
        PauseMenuPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void OpenInfo()
    {
        PauseMenuPanel.SetActive(false);
        InfoPanel.SetActive(true);
    }

    public void BackToPause()
    {
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
        if (InfoPanel != null) InfoPanel.SetActive(false);
        PauseMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}