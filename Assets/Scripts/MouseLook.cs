using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float minPitch = -90f;
    public float maxPitch = 90f;
    public float debuffSensitivityMultiplier = 0.5f;
    Transform randyPlayer;
    float pitch;
    LevelManager levelManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randyPlayer = transform.parent.transform;
        levelManager = FindAnyObjectByType<LevelManager>();
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        float currentSensitivity = mouseSensitivity;
        if (levelManager && levelManager.IsDebuffActive())
            currentSensitivity *= debuffSensitivityMultiplier;

        float moveX = Input.GetAxis("Mouse X") * currentSensitivity * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * currentSensitivity * Time.deltaTime;

        if (randyPlayer)
            randyPlayer.Rotate(Vector3.up * moveX);

        pitch -= moveY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
}
