using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private AudioClip loseSound; // Add lose sound reference
    private GameObject glassObject; // Reference to the Glass GameObject
    private GameObject scoreObject; // Reference to the Score GameObject
    private Text scoreText; // Reference to the Text component
    private GameObject gameOverOverlay; // Reference to the game over overlay
    
    // Start is called before the first frame update
    void Start()
    {
        // Reset static variables for complete restart
        MoveObject.isGameOver = false;
        BottleFall.currentFallSpeedBonus = 0f;
        Debug.Log("Reset static variables for complete restart");
        
        // Set up audio
        audioSource = gameObject.AddComponent<AudioSource>();
        plopSound = Resources.Load<AudioClip>("Audio/SFX/plop");
        
        if (plopSound == null)
        {
           // Debug.LogError("Could not load plop.ogg! Make sure it's in Resources/Audio/SFX/plop.ogg");
        }
        
        // Find the bottle spawner for difficulty scaling
        bottleSpawner = FindObjectOfType<BottleSpawner>();
        
        // Find the girl script for sprite changes
        girlScript = FindObjectOfType<Girl>();
        
        // Find the Glass GameObject
        glassObject = GameObject.Find("Glass");
        if (glassObject == null)
        {
          //  Debug.LogError("Could not find Glass GameObject in hierarchy!");
        }
        else
        {
            // Add SpriteRenderer if it doesn't exist
            if (glassObject.GetComponent<SpriteRenderer>() == null)
            {
                glassObject.AddComponent<SpriteRenderer>();
            }
        }
        
        // Find the Score GameObject (check in Score layer)
        scoreObject = GameObject.Find("Score");
        if (scoreObject == null)
        {
            // Try to find it by searching all objects in the scene
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "Score" && obj.activeInHierarchy)
                {
                    scoreObject = obj;
                    break;
                }
            }
        }
        
        if (scoreObject == null)
        {
          //  Debug.LogError("Could not find Score GameObject in hierarchy! Make sure it's named 'Score' and is active.");
        }
        else
        {
            // Get the Text component
            scoreText = scoreObject.GetComponent<Text>();
            if (scoreText == null)
            {
               // Debug.LogError("Score GameObject does not have a Text component!");
            }
            else
            {
               // Debug.Log("Found Score GameObject with Text component!");
                
                // Check if there's a Canvas in the scene
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                   // Debug.Log("No Canvas found! Creating one...");
                    // Create a Canvas GameObject
                    GameObject canvasObject = new GameObject("Canvas");
                    canvas = canvasObject.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = Camera.main; // Use the main camera
                    canvas.planeDistance = 1f; // Close to camera but in front
                    canvas.sortingOrder = 100; // High sorting order to appear on top
                    
                    // Add CanvasScaler for proper scaling with your camera setup
                    CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.referenceResolution = new Vector2(1920, 1080);
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    canvasScaler.matchWidthOrHeight = 0.5f; // Balance between width and height matching
                    
                    // Add GraphicRaycaster for UI interactions
                    canvasObject.AddComponent<GraphicRaycaster>();
                    
                    // Add EventSystem if it doesn't exist (required for UI interactions)
                    if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
                    {
                        GameObject eventSystemObject = new GameObject("EventSystem");
                        eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
                        eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                        Debug.Log("Created EventSystem for UI interactions in Start");
                    }
                    
                    //Debug.Log("Created Canvas with camera-based rendering");
                }
                else
                {
                    // Update existing canvas to work with camera
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = Camera.main;
                    canvas.planeDistance = 1f;
                  //  Debug.Log("Updated existing Canvas to use camera rendering");
                }
                
                // Make sure the Score object is a child of the Canvas
                if (scoreObject.transform.parent != canvas.transform)
                {
                    scoreObject.transform.SetParent(canvas.transform, false);
                  //  Debug.Log("Moved Score object under Canvas");
                }
                
                // Configure the text component for visibility
                scoreText.text = "Score: 0";
                scoreText.fontSize = 48; // Doubled from 24 to 48
                scoreText.color = new Color(1f, 0.84f, 0f, 1f); // Gold color (RGB: 255, 215, 0)
                scoreText.fontStyle = FontStyle.Bold;
                
                // Set anchor and position for specified location (centered anchors)
                RectTransform rectTransform = scoreText.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // Set anchors to center (0.5, 0.5) so (0,0) is dead center of screen
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.anchoredPosition = new Vector2(-750, 180);
                    rectTransform.sizeDelta = new Vector2(400, 100); // Doubled size to accommodate larger text
                 //   Debug.Log("Configured Score text position and size with center anchors");
                }
                
             //   Debug.Log("Score text configured: fontSize=" + scoreText.fontSize + ", color=" + scoreText.color);
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
        
        // Load lose sound
        loseSound = Resources.Load<AudioClip>("Audio/SFX/Lose");
        if (loseSound == null)
        {
            Debug.LogError("Could not load Lose sound! Make sure it's in Resources/Audio/SFX/Lose.ogg");
        }
        
        // Set initial difficulty for level 1
        SetLevelDifficulty(1);
        
        // Set initial glass sprite for level 1
        UpdateGlassSprite(1);
        
        // Update initial score display
        UpdateScoreDisplay();
        
       // Debug.Log("Beer crate ready to catch bottles!");
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
            
            // Update score display
            UpdateScoreDisplay();
            
            // Log the catch
          //  Debug.Log("Bottle caught! Total: " + bottlesCaught);
            
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
        
      //  Debug.Log("ADVANCING TO LEVEL " + newLevel + "!");
        
        // Change girl sprite BEFORE freezing the game
        if (girlScript != null)
        {
            girlScript.currentSprite = newLevel - 1; // Level 2 = sprite 1, Level 3 = sprite 2
          //  Debug.Log("Changed girl to sprite " + (newLevel - 1) + " for level " + newLevel);
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
        
       // Debug.Log("Level " + newLevel + " started!");
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
        
        //Debug.Log("Set difficulty for level " + level + ": Fall speed bonus " + BottleFall.currentFallSpeedBonus + 
             //    ", Spawn intervals " + bottleSpawner?.minSpawnInterval + "-" + bottleSpawner?.maxSpawnInterval);
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
           // Debug.Log("Updated glass to level " + level + " sprite: " + spritePath);
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
       // Debug.Log("Bottle missed! Total missed: " + bottlesMissed);
        
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
               // Debug.Log("Removed " + heartName + " - " + (maxMissedBottles - missedCount) + " lives remaining");
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
        
        // Play lose sound effect
        if (loseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(loseSound);
            Debug.Log("Playing lose sound effect");
        }
        
        // Disable player movement to allow UI mouse input
        MoveObject.isGameOver = true;
        Debug.Log("Disabled player movement for UI interaction");
        
        // Stop bottle spawning instead of freezing the entire game
        if (bottleSpawner != null)
        {
            bottleSpawner.StopSpawning();
            Debug.Log("Stopped bottle spawner");
        }
        
        // Destroy any remaining bottles in the scene
        GameObject[] bottles = GameObject.FindGameObjectsWithTag("Bottle");
        foreach (GameObject bottle in bottles)
        {
            Destroy(bottle);
        }
        Debug.Log("Destroyed " + bottles.Length + " remaining bottles");
        
        // Create game over overlay
        CreateGameOverOverlay();
    }
    
    void CreateGameOverOverlay()
    {
        // Find or create canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("GameOverCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 0.1f; // Very close to camera
            canvas.sortingOrder = 1000; // Very high sorting order to appear on top
            
            // Add CanvasScaler for proper scaling
            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
            
            // Add GraphicRaycaster for button interactions
            canvasObject.AddComponent<GraphicRaycaster>();
            
            // Add EventSystem if it doesn't exist (required for UI interactions)
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("Created EventSystem for UI interactions");
            }
            
            //Debug.Log("Created Game Over Canvas with high sorting order");
        }
        else
        {
            // Temporarily increase sorting order for game over
            canvas.sortingOrder = 1000;
            
            // Add EventSystem if it doesn't exist (required for UI interactions)
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("Created EventSystem for UI interactions");
            }
            
          //  Debug.Log("Updated existing Canvas sorting order for game over");
        }
        
        // Create overlay GameObject
        gameOverOverlay = new GameObject("GameOverOverlay");
        gameOverOverlay.transform.SetParent(canvas.transform, false);
        
        // Add Image component for semi-transparent background
        Image backgroundImage = gameOverOverlay.AddComponent<Image>();
        backgroundImage.color = new Color(1f, 1f, 1f, 0.8f); // Semi-transparent white
        
        // Set sorting layer to Score
        Canvas overlayCanvas = gameOverOverlay.AddComponent<Canvas>();
        overlayCanvas.overrideSorting = true;
        overlayCanvas.sortingLayerName = "Score";
        overlayCanvas.sortingOrder = 100;
        
        // Set to cover entire screen
        RectTransform overlayRect = gameOverOverlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        
        // Create "YOU LOST!" text
        GameObject lostText = new GameObject("LostText");
        lostText.transform.SetParent(gameOverOverlay.transform, false);
        Text lostTextComponent = lostText.AddComponent<Text>();
        lostTextComponent.text = "YOU LOST!";
        lostTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lostTextComponent.fontSize = 72;
        lostTextComponent.color = Color.red;
        lostTextComponent.fontStyle = FontStyle.Bold;
        lostTextComponent.alignment = TextAnchor.MiddleCenter;
        
        // Set sorting layer for lost text
        Canvas lostTextCanvas = lostText.AddComponent<Canvas>();
        lostTextCanvas.overrideSorting = true;
        lostTextCanvas.sortingLayerName = "Score";
        lostTextCanvas.sortingOrder = 101;
        
        RectTransform lostTextRect = lostText.GetComponent<RectTransform>();
        lostTextRect.anchorMin = new Vector2(0.5f, 0.5f);
        lostTextRect.anchorMax = new Vector2(0.5f, 0.5f);
        lostTextRect.anchoredPosition = new Vector2(0, 100);
        lostTextRect.sizeDelta = new Vector2(600, 100);
        
        // Create "SCORE: X" text
        GameObject finalScoreText = new GameObject("FinalScoreText");
        finalScoreText.transform.SetParent(gameOverOverlay.transform, false);
        Text finalScoreComponent = finalScoreText.AddComponent<Text>();
        int finalScore = bottlesCaught * 100;
        finalScoreComponent.text = "SCORE: " + finalScore;
        finalScoreComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        finalScoreComponent.fontSize = 48;
        finalScoreComponent.color = new Color(1f, 0.84f, 0f, 1f); // Same gold as score
        finalScoreComponent.fontStyle = FontStyle.Bold;
        finalScoreComponent.alignment = TextAnchor.MiddleCenter;
        
        // Set sorting layer for final score text
        Canvas finalScoreCanvas = finalScoreText.AddComponent<Canvas>();
        finalScoreCanvas.overrideSorting = true;
        finalScoreCanvas.sortingLayerName = "Score";
        finalScoreCanvas.sortingOrder = 102;
        
        RectTransform finalScoreRect = finalScoreText.GetComponent<RectTransform>();
        finalScoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        finalScoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        finalScoreRect.anchoredPosition = new Vector2(0, 0);
        finalScoreRect.sizeDelta = new Vector2(400, 80);
        
        // Create "RETRY" button with screen space click detection
        GameObject retryButton = new GameObject("RetryButton");
        retryButton.transform.SetParent(gameOverOverlay.transform, false);
        
        Image buttonImage = retryButton.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Dark semi-transparent background
        
        // Add a simple click handler script
        RetryButtonClick clickHandler = retryButton.AddComponent<RetryButtonClick>();
        clickHandler.gatherScript = this; // Pass reference to call RetryGame()
        
        // Set sorting layer for retry button
        Canvas buttonCanvas = retryButton.AddComponent<Canvas>();
        buttonCanvas.overrideSorting = true;
        buttonCanvas.sortingLayerName = "Score";
        buttonCanvas.sortingOrder = 103;
        
        RectTransform buttonRect = retryButton.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0, -100);
        buttonRect.sizeDelta = new Vector2(200, 60);
        
        Debug.Log("Created retry button with screen space click detection");
        
        // Create button text
        GameObject buttonText = new GameObject("ButtonText");
        buttonText.transform.SetParent(retryButton.transform, false);
        Text buttonTextComponent = buttonText.AddComponent<Text>();
        buttonTextComponent.text = "RETRY";
        buttonTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonTextComponent.fontSize = 36;
        buttonTextComponent.color = Color.white;
        buttonTextComponent.fontStyle = FontStyle.Bold;
        buttonTextComponent.alignment = TextAnchor.MiddleCenter;
        
        // Set sorting layer for button text
        Canvas buttonTextCanvas = buttonText.AddComponent<Canvas>();
        buttonTextCanvas.overrideSorting = true;
        buttonTextCanvas.sortingLayerName = "Score";
        buttonTextCanvas.sortingOrder = 104;
        
        RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
       // Debug.Log("Game over overlay created with final score: " + finalScore);
    }
    
    public void RetryGame()
    {
        Debug.Log("RETRY BUTTON: RetryGame() method called!");
        Debug.Log("RETRY BUTTON: Performing complete restart...");
        
        // Reset all static variables
        MoveObject.isGameOver = false;
        BottleFall.currentFallSpeedBonus = 0f;
        
        // Force garbage collection to clean up any lingering objects
        System.GC.Collect();
        
        // Ensure Time.timeScale is restored
        Time.timeScale = 1f;
        
        Debug.Log("RETRY BUTTON: Loading scene 0...");
        SceneManager.LoadScene(0); // Load the starting screen (scene 0)
    }
    
    // Alternative nuclear restart method if needed
    public void NuclearRestart()
    {
        Debug.Log("NUCLEAR RESTART: Restarting entire application...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            int score = bottlesCaught * 100;
            scoreText.text = "Score: " + score;

        }
        else
        {
            Debug.LogError("scoreText is null! Cannot update score display.");
        }
    }
}
