using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSpawner : MonoBehaviour
{
    public GameObject cervezaCristalBottle;
    public float minSpawnInterval = 0.1f; // Minimum time between spawns
    public float maxSpawnInterval = 0.1f; // Maximum time between spawns
    
    private Coroutine spawningCoroutine; // Reference to the spawning coroutine
    
    void Start()
    {
        // Start the spawning coroutine - independent of frame rate
        spawningCoroutine = StartCoroutine(SpawnBottlesCoroutine());
    }
    
    public void StopSpawning()
    {
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
            spawningCoroutine = null;
            Debug.Log("BottleSpawner: Stopped spawning bottles");
        }
    }
    
    IEnumerator SpawnBottlesCoroutine()
    {
        while (true) // Infinite loop
        {
            // Spawn a bottle
            SpawnBottle();

            // Wait for a random interval (delta time independent)
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
           // Debug.Log("test " + waitTime);
        }
    }

    void SpawnBottle()
    {
    
        
        float x = Random.Range(-90f, 88f); // Adjust to your background's width
        float y = 100f; // Above the background
        Vector3 spawnPos = new Vector3(x, y, 0);
        Instantiate(cervezaCristalBottle, spawnPos, Quaternion.identity);
    }
}

