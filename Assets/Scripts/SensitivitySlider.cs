using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    public Slider slider;
    public float minSensitivity = 20f;
    public float maxSensitivity = 300f;

    void Start()
    {
        // load saved value and default to current MouseLook sensitivity if none saved
        float saved = PlayerPrefs.GetFloat("MouseSensitivity", 100f);
        slider.minValue = minSensitivity;
        slider.maxValue = maxSensitivity;
        slider.value = saved;

        slider.onValueChanged.AddListener(OnSliderChanged);
        ApplySensitivity(saved);
    }

    void OnSliderChanged(float value)
    {
        ApplySensitivity(value);
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
    }

    void ApplySensitivity(float value)
    {
        MouseLook ml = FindAnyObjectByType<MouseLook>();
        if (ml != null)
            ml.mouseSensitivity = value;
    }
}