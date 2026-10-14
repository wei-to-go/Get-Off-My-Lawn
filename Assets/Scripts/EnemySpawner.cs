using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount = 5;
        public float spawnInterval = 2f;
        public float[] speedRange = new float[2];
        public float[] cortisolRateRange = new float[2];
        public float[] maxProximityDistanceRange = new float[2];
        public float[] maxHPRange = new float[2];
    }
    public Wave[] waves;
    public GameObject enemyPrefab;
    public AudioClip waveSFX;
    public float spawnRadius = 20f;
    int currentWave = 0;
    public float waveBuffer = 2f;
    private LevelManager levelManager;
    private SkyBoxChanger skybox;
    private AudioSource audioSource;
    float lastWaveStart;

    void Start()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
        skybox = FindAnyObjectByType<SkyBoxChanger>();
        if (waveSFX)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = waveSFX;
            audioSource.loop = false;
            audioSource.volume = 1f;
        }
        StartCoroutine(WaveManager());
    }
    System.Collections.IEnumerator WaveManager()
    {
        yield return new WaitUntil(WaveStart);
        SunBehavior sun = FindAnyObjectByType<SunBehavior>();
        if (sun != null)
            sun.SetVulnerable(false);

        while (currentWave < waves.Length)
        {
            lastWaveStart = Time.time;
            if(currentWave > 0)
                skybox.UpdateSkybox(currentWave); //first wave doesn't lerp
            if(levelManager)
                StartCoroutine(levelManager.ShowWaveText("WAVE " + (currentWave + 1).ToString()));

            if (audioSource != null)
                audioSource.Play();
            yield return new WaitForSeconds(waveBuffer);
            yield return StartCoroutine(SpawnEnemies(waves[currentWave]));
            yield return new WaitUntil(WaveOver);

            // Sun becomes vulnerable between waves
            if (sun != null && SceneManager.GetActiveScene().buildIndex == 2)
            {
                if (levelManager)
                    StartCoroutine(levelManager.ShowWaveText("THE SUN IS VULNERABLE!"));

                sun.SetVulnerable(true);
                yield return new WaitForSeconds(10f);
                sun.SetVulnerable(false);

                if (levelManager)
                    StartCoroutine(levelManager.ShowWaveText("THE SUN RECOVERS!"));
                yield return new WaitForSeconds(2f);
            }
            currentWave++;
        }
        levelManager.WinLevel();
    }
    System.Collections.IEnumerator SpawnEnemies(Wave w)
        {
            for(int i=0; i < w.enemyCount; i++)
            {
                SpawnEnemy(waves[currentWave]);
                yield return new WaitForSeconds(w.spawnInterval);
            }
        }
    void SpawnEnemy(Wave w)
    {
        if(enemyPrefab == null)
            return;

        Vector3 spawnOffset = Random.insideUnitSphere * spawnRadius;
        spawnOffset.y = 2f;

        GameObject enemy = Instantiate(enemyPrefab, transform.position + spawnOffset, Quaternion.identity);
        SunDwellerBehavior SunDweller = enemy.GetComponent<SunDwellerBehavior>();
        SunDweller.UpdateStats(Random.Range(w.speedRange[0], w.speedRange[1]), Random.Range(w.cortisolRateRange[0], w.cortisolRateRange[1]), 
            Random.Range(w.maxProximityDistanceRange[0], w.maxProximityDistanceRange[1]), Random.Range(w.maxHPRange[0], w.maxHPRange[1]));
    }

    bool WaveOver()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }

    bool WaveStart()
    {
        return LevelManager.isPlaying;
    }
}