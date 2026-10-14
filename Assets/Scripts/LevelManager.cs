using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public TMP_Text cortisolText;
    public TMP_Text waveText;
    public Slider cortisolSlider;
    public TMP_Text timerText;
    public GameObject winPanel;
    public GameObject losePanel;
    public TMP_Text killCounterText;
    public AudioClip backgroundMusic;
    public Image cortisolOverlay;

    public GameObject player;
    public Vector3 spawnPos;


    public static bool gameStarted = false;


    public static bool isPlaying = true;
    public float winTime = 120f;
    public float cortisolBaseRate = 1.5f;
    public float cortisolChangeRate = 0f;
    public float debuffThreshold = 50f;

    public float timeRemaining;
    private int killCount = 0;
    private AudioSource audioSource;
    Vector3 waveTextInitialScale;

    float cortisolLevel = 0f;

    void Start()
    {
        //new levelmanager in the new level needs to find the UI objects that persisted from the previous level
        player = GameObject.FindGameObjectWithTag("Player");
        waveTextInitialScale = waveText.transform.localScale;
        cortisolLevel = 0f;
        timeRemaining = winTime;
        gameStarted = false;
        isPlaying = false;
        Time.timeScale = 0f;
        timerText.enabled = true;
        //killCount = 0; decide if we want to reset the kill count every level

        if (backgroundMusic)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = 0.75f;
            audioSource.Play();
        }

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        if (killCounterText != null)
            killCounterText.text = "Kills: " + killCount;
        
        if (waveText != null)
            waveText.gameObject.SetActive(false);
        
        if (cortisolOverlay != null)
            cortisolOverlay.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted)
            return;

        if(isPlaying)
        {
            UpdateTimer();
            UpdateCortisol();
            UpdateCortisolEffect();
            CheckGameOver();
        }
    }

    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeRemaining <= 0f)
        {
            LoseGame();
        }
    }

    void UpdateCortisol()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

        
        
        if(!isMoving)
        {
            cortisolLevel += cortisolChangeRate * Time.deltaTime; // still affected by sun dwellers
        }
        else
        {
            cortisolLevel += (cortisolBaseRate + cortisolChangeRate) * Time.deltaTime;
        }

        cortisolLevel = Mathf.Clamp(cortisolLevel, 0f, 100f);
    }

    void UpdateCortisolEffect()
    {
        if (cortisolText != null)
        {
            cortisolText.color = IsDebuffActive() ? Color.red : Color.white;
        }

        if (cortisolSlider != null)
            cortisolSlider.value = cortisolLevel;

        if (cortisolOverlay != null)
        {
            if (cortisolLevel >= debuffThreshold)
            {
                cortisolOverlay.gameObject.SetActive(true);
                float alpha = Mathf.InverseLerp(debuffThreshold, 100f, cortisolLevel) * 0.4f;
                cortisolOverlay.color = new Color(1f, 0f, 0f, alpha);
            }
            else
            {
                cortisolOverlay.gameObject.SetActive(false);
            }
        }
    }

    void CheckGameOver()
    {
        if (cortisolLevel >= 100f)
        {
            LoseGame();
        }
    }

    public void WinLevel()
    {
        isPlaying = false;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        timerText.enabled = false;
        if (audioSource) audioSource.Stop();

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void LoseGame()
    {
        isPlaying = false;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        timerText.enabled = false;
        if (audioSource) audioSource.Stop();

        if (losePanel != null)
            losePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        ResetPersistentObjects();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void RestartGame()
    {
        ResetPersistentObjects();
        WeaponSwitcher.Instance.ResetWeaponLockState();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        ResetPersistentObjects();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void changeCortisolRate(float amount)
    {
        cortisolChangeRate += amount;
        cortisolChangeRate = Mathf.Clamp(cortisolChangeRate, 0f, 10f); //max change rate
    }

    public void changeCortisolLevel(float amt)
    {
        cortisolLevel += amt;
        cortisolLevel = Mathf.Clamp(cortisolLevel, 0f, 100f);
    }

    public bool IsDebuffActive()
    {
        return cortisolLevel >= debuffThreshold;
    }

    public void AddKill()
    {
        killCount++;

        if (killCounterText != null)
            killCounterText.text = "Kills: " + killCount;
    }

    public void SetWaveText(string text)
    {
        if (waveText != null)
            waveText.text = text;
    }

    void ResetPersistentObjects()
    {
        foreach(WeaponSwitcher.Weapon w in WeaponSwitcher.Instance.weapons)
        {
            w.currentMagRounds = w.maxMagRounds;
        }

        CharacterController controller = player.GetComponent<CharacterController>();
        controller.enabled = false;
        player.transform.position = spawnPos;
        controller.enabled = true;
        ShootBullet sb = GameObject.FindAnyObjectByType<ShootBullet>();
        sb.GetLevelManager();
    }

    public IEnumerator ShowWaveText(string text)
    {
        if (waveText == null)
            yield break;

        yield return new WaitForSeconds(1f); // wait after game starts / wave changes

        waveText.gameObject.SetActive(true);
        waveText.text = text;
        waveText.alpha = 0f;

        float duration = 0.4f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            waveText.alpha = progress;
            waveText.transform.localScale = Vector3.Lerp(waveTextInitialScale, waveTextInitialScale * 1.2f, progress);

            yield return null;
        }

        yield return new WaitForSeconds(1.2f);

        duration = 0.4f;
        t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            waveText.alpha = 1f - progress;
            waveText.transform.localScale = Vector3.Lerp(waveTextInitialScale * 1.2f, waveTextInitialScale * 0.5f, progress);

            yield return null;
        }

        waveText.gameObject.SetActive(false);
    }
}