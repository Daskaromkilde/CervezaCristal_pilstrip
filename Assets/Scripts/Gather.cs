using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gather : MonoBehaviour
{
    public int bottlesCaught = 0;
    public int bottlesMissed = 0;
    public int maxMissedBottles = 3; // Game over after 3 missed bottles
    
    // Start is called before the first frame update
    void Start()
    {
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
            // Increase the score
            bottlesCaught++;
            
            // Log the catch
            Debug.Log("Bottle caught! Total: " + bottlesCaught);
            
            // Destroy the bottle
            Destroy(other.gameObject);
        }
    }
    
    // Called by BottleFall when a bottle reaches the bottom
    public void BottleMissed()
    {
        bottlesMissed++;
        Debug.Log("Bottle missed! Total missed: " + bottlesMissed);
        
        // Check if game should end
        if (bottlesMissed >= maxMissedBottles)
        {
            GameOver();
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
