using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public GameObject pickupPrefab;
    public int maxPickupCount = 15;
    public float spawnInterval = 2f;
    public float spawnRadius = 35f;

    void Start()
    {
        StartCoroutine(SpawnPickups());
    }

    System.Collections.IEnumerator SpawnPickups()
    {
        yield return new WaitUntil(WaveStart);
        while(LevelManager.isPlaying)
        {
            int pickupCount = GameObject.FindGameObjectsWithTag("Pickup").Length;
            if(pickupCount < maxPickupCount)
            {
                SpawnPickup();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPickup()
    {
        if(pickupPrefab == null)
            return;

        // spawn at random position around spawner
        Vector3 spawnOffset = Random.insideUnitSphere * spawnRadius;
        spawnOffset.y = 0f; // spawn at ground level

        Instantiate(pickupPrefab, transform.position + spawnOffset, Quaternion.identity);
    }
    bool WaveStart()
    {
        return LevelManager.isPlaying;
    }
}
