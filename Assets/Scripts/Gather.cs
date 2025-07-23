using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gather : MonoBehaviour
{
    public int bottlesCaught = 0;
    public int bottlesMissed = 0;
    public int maxMissedBottles = 3; // Game over after 3 missed bottles
    
    private AudioSource audioSource;
    private AudioClip plopSound;
    private BottleSpawner bottleSpawner;
    
    [Header("Difficulty Scaling")]
    public int bottlesPerLevel = 40; // Advance level every 40 bottles
    private int currentLevel = 1; // Start at level 1
    private Girl girlScript; // Reference to girl for sprite changes
    private AudioClip beerPourSound;
    private AudioClip level3Sound;
    private GameObject glassObject; // Reference to the Glass GameObject
    
    // Start is called before the first frame update
    void Start()
    {
        // Set up audio
        audioSource = gameObject.AddComponent<AudioSource>();
        plopSound = Resources.Load<AudioClip>("Audio/SFX/plop");
        
        if (plopSound == null)
        {
            Debug.LogError("Could not load plop.ogg! Make sure it's in Resources/Audio/SFX/plop.ogg");
        }
        
        // Find the bottle spawner for difficulty scaling
        bottleSpawner = FindObjectOfType<BottleSpawner>();
        
        // Find the girl script for sprite changes
        girlScript = FindObjectOfType<Girl>();
        
        // Find the Glass GameObject
        glassObject = GameObject.Find("Glass");
        if (glassObject == null)
        {
            Debug.LogError("Could not find Glass GameObject in hierarchy!");
        }
        else
        {
            // Add SpriteRenderer if it doesn't exist
            if (glassObject.GetComponent<SpriteRenderer>() == null)
            {
                glassObject.AddComponent<SpriteRenderer>();
            }
        }
        
        // Load beer pour sound
        beerPourSound = Resources.Load<AudioClip>("Audio/SFX/beerPour");
        if (beerPourSound == null)
        {
            Debug.LogError("Could not load beerPour sound! Make sure it's in Resources/Audio/SFX/beerPour");
        }
        
        // Load level3 sound
        level3Sound = Resources.Load<AudioClip>("Audio/SFX/level3");
        if (level3Sound == null)
        {
            Debug.LogError("Could not load level3 sound! Make sure it's in Resources/Audio/SFX/level3.ogg");
        }
        
        // Set initial difficulty for level 1
        SetLevelDifficulty(1);
        
        // Set initial glass sprite for level 1
        UpdateGlassSprite(1);
        
        Debug.Log("Beer crate ready to catch bottles!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // This method is called when another collider enters this trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is a bottle
        if (other.CompareTag("Bottle")) // Make sure your bottles have "Bottle" tag
        {
            // Play the plop sound
            if (plopSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(plopSound);
            }
            
            // Increase the score
            bottlesCaught++;
            
            // Log the catch
            Debug.Log("Bottle caught! Total: " + bottlesCaught);
            
            // Check if we should advance to next level
            CheckLevelAdvancement();
            
            // Destroy the bottle
            Destroy(other.gameObject);
        }
    }
    
    void CheckLevelAdvancement()
    {
        // Check if we should advance to next level (every 40 bottles)
        if (bottlesCaught == bottlesPerLevel && currentLevel == 1)
        {
            // Advance to level 2
            AdvanceToLevel(2);
        }
        else if (bottlesCaught == bottlesPerLevel * 2 && currentLevel == 2)
        {
            // Advance to level 3
            AdvanceToLevel(3);
        }
        // Level 3 is the max level - no more advancement
    }
    
    void AdvanceToLevel(int newLevel)
    {
        currentLevel = newLevel;
        
        Debug.Log("ADVANCING TO LEVEL " + newLevel + "!");
        
        // Change girl sprite BEFORE freezing the game
        if (girlScript != null)
        {
            girlScript.currentSprite = newLevel - 1; // Level 2 = sprite 1, Level 3 = sprite 2
            Debug.Log("Changed girl to sprite " + (newLevel - 1) + " for level " + newLevel);
        }
        
        // Start coroutine to handle the transition with blend effect
        StartCoroutine(LevelTransition(newLevel));
    }
    
    IEnumerator LevelTransition(int newLevel)
    {
        // 1. Play level advancement sound FIRST
        if (level3Sound != null && audioSource != null)
        {
            audioSource.PlayOneShot(level3Sound);
        }
        
        // 2. Play beer pour sound
        if (beerPourSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(beerPourSound);
        }
        
        // 3. THEN freeze the game
        Time.timeScale = 0f;
        
        // 4. Change glass and girl sprites DURING freeze
        UpdateGlassSprite(newLevel);
        yield return StartCoroutine(BlendToNewSprite());
        
        // Wait remaining time (total 3 seconds including fade time)
        yield return new WaitForSecondsRealtime(2f); // 3 seconds total - 1 second for fade = 2 seconds
        
        // Set new difficulty
        SetLevelDifficulty(newLevel);
        
        // Resume the game
        Time.timeScale = 1f;
        
        Debug.Log("Level " + newLevel + " started!");
    }
    
    IEnumerator BlendToNewSprite()
    {
        if (girlScript == null) yield break;
        
        SpriteRenderer girlRenderer = girlScript.GetComponent<SpriteRenderer>();
        if (girlRenderer == null) yield break;
        
        // Fade out current sprite
        float fadeTime = 0.5f;
        Color originalColor = girlRenderer.color;
        
        // Fade out (use unscaled time to work during freeze)
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            girlRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // Change to new sprite
        girlScript.ShowCurrentSprite();
        
        // Fade in new sprite (use unscaled time to work during freeze)
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            girlRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // Ensure fully visible
        girlRenderer.color = originalColor;
    }
    
    void SetLevelDifficulty(int level)
    {
        switch (level)
        {
            case 1:
                // Level 1: Fall speed 50, spawn intervals 0.25-1.0 (was level 2's intervals)
                BottleFall.currentFallSpeedBonus = 0f; // 50 total (50 + 0)
                if (bottleSpawner != null)
                {
                    bottleSpawner.minSpawnInterval = 0.25f;
                    bottleSpawner.maxSpawnInterval = 1.0f;
                }
                break;
                
            case 2:
                // Level 2: Fall speed 100, spawn intervals 0.25-0.5 (was level 3's intervals)
                BottleFall.currentFallSpeedBonus = 50f; // 100 total (50 + 50)
                if (bottleSpawner != null)
                {
                    bottleSpawner.minSpawnInterval = 0.25f;
                    bottleSpawner.maxSpawnInterval = 0.5f;
                }
                break;
                
            case 3:
                // Level 3: Fall speed 150, spawn intervals 0.1-0.3 (even harder!)
                BottleFall.currentFallSpeedBonus = 100f; // 150 total (50 + 100)
                if (bottleSpawner != null)
                {
                    bottleSpawner.minSpawnInterval = 0.1f;
                    bottleSpawner.maxSpawnInterval = 0.3f;
                }
                break;
        }
        
        Debug.Log("Set difficulty for level " + level + ": Fall speed bonus " + BottleFall.currentFallSpeedBonus + 
                 ", Spawn intervals " + bottleSpawner?.minSpawnInterval + "-" + bottleSpawner?.maxSpawnInterval);
    }
    
    void UpdateGlassSprite(int level)
    {
        if (glassObject == null) return;
        
        SpriteRenderer glassRenderer = glassObject.GetComponent<SpriteRenderer>();
        if (glassRenderer == null) return;
        
        // Load the appropriate glass sprite
        string spritePath = "Glass/l" + level;
        Sprite glassSprite = Resources.Load<Sprite>(spritePath);
        
        if (glassSprite != null)
        {
            glassRenderer.sprite = glassSprite;
            Debug.Log("Updated glass to level " + level + " sprite: " + spritePath);
        }
        else
        {
            Debug.LogError("Could not load glass sprite: " + spritePath + ". Make sure it exists in Resources/Glass/");
        }
    }
    
    // Called by BottleFall when a bottle reaches the bottom
    public void BottleMissed()
    {
        bottlesMissed++;
        Debug.Log("Bottle missed! Total missed: " + bottlesMissed);
        
        // Remove heart sprite based on how many bottles missed
        RemoveHeart(bottlesMissed);
        
        // Check if game should end
        if (bottlesMissed >= maxMissedBottles)
        {
            GameOver();
        }
    }
    
    void RemoveHeart(int missedCount)
    {
        string heartName = "";
        
        switch (missedCount)
        {
            case 1:
                heartName = "heart3"; // Remove heart3 first
                break;
            case 2:
                heartName = "heart2"; // Then heart2
                break;
            case 3:
                heartName = "heart1"; // Finally heart1
                break;
        }
        
        if (!string.IsNullOrEmpty(heartName))
        {
            GameObject heart = GameObject.Find(heartName);
            if (heart != null)
            {
                heart.SetActive(false); // Hide the heart
                Debug.Log("Removed " + heartName + " - " + (maxMissedBottles - missedCount) + " lives remaining");
            }
            else
            {
                Debug.LogError("Could not find heart: " + heartName);
            }
        }
    }
    
    void GameOver()
    {
        Debug.Log("GAME OVER! You missed " + maxMissedBottles + " bottles!");
        Debug.Log("Final Score - Caught: " + bottlesCaught + ", Missed: " + bottlesMissed);
        
        // Stop the game
        Time.timeScale = 0f; // Pauses the game
        
        // Optional: Load game over scene or restart
        // SceneManager.LoadScene("GameOverScene");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart current scene
    }
}
