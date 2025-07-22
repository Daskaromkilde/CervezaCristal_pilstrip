using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSpawner : MonoBehaviour
{
    public GameObject cervezaCristalBottle;
    public float spawnInterval = 1.0f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBottle();
        }
    }

    void SpawnBottle()
    {
    
        
        float x = Random.Range(-80f, 80f); // Adjust to your background's width
        float y = 100f; // Above the background
        Vector3 spawnPos = new Vector3(x, y, 0);
        Instantiate(cervezaCristalBottle, spawnPos, Quaternion.identity);
    }
}

