using UnityEngine;
using System.Collections;

public class SkyBoxChanger : MonoBehaviour
{
    public Material[] skyOptions;

    void Awake()
    {
        if (skyOptions != null && skyOptions.Length > 0)
            RenderSettings.skybox = skyOptions[0];
            RenderSettings.skybox.SetFloat("_Exposure", 1f);
            DynamicGI.UpdateEnvironment();
    }

    public void UpdateSkybox(int currentWave)
    {
        if (skyOptions == null || currentWave >= skyOptions.Length)
            return;

        StartCoroutine(FadeSkybox(skyOptions[currentWave]));
    }

    IEnumerator FadeSkybox(Material newSkybox)
    {
        float fadeTime = 1f;
        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Exposure",
                Mathf.Lerp(1f, 0f, t / fadeTime));

            yield return null;
        }

        RenderSettings.skybox = newSkybox;
        t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Exposure",
                Mathf.Lerp(0f, 1f, t / fadeTime));

            yield return null;
        }

        DynamicGI.UpdateEnvironment();
    }
}