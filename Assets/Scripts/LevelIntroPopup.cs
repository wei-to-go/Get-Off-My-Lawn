using UnityEngine;
using UnityEngine.UI;

public class LevelIntroPopup : MonoBehaviour
{
    public GameObject introPanel;

    void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;

        LevelManager.gameStarted = false;
        LevelManager.isPlaying = false;

        introPanel.SetActive(true);
    }

    public void StartLevel()
    {
        introPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        LevelManager.gameStarted = true;
        LevelManager.isPlaying = true;

        Time.timeScale = 1f;
    }
}