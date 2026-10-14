using UnityEngine;

public class InstructionsPopup : MonoBehaviour
{
    public GameObject instructionsPanel;
    public GameObject levelIntroPanel;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;

        instructionsPanel.SetActive(true);
        levelIntroPanel.SetActive(false);
    }

    public void CloseInstructions()
    {
        instructionsPanel.SetActive(false);
        levelIntroPanel.SetActive(true);
    }
}