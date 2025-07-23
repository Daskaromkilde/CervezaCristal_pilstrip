using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BottleFall : MonoBehaviour
{
    private float backgroundMinY;
    private SpriteRenderer bottleRenderer;
    private Gather gatherScript;
    private Rigidbody2D rb;
    
    public float baseFallSpeed = 50f; // Base falling speed
    public static float currentFallSpeedBonus = 0f; // Static bonus that increases over time
    
    // Start is called before the first frame update
    void Start()
    {
        bottleRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        // Calculate actual fall speed with bonus
        float actualFallSpeed = baseFallSpeed + currentFallSpeedBonus;
        
        // Set initial downward velocity for immediate max speed
        if (rb != null)
        {
            rb.velocity = new Vector2(0, -actualFallSpeed);
        }
        
        Debug.Log("Bottle falling at speed: " + actualFallSpeed + " (base: " + baseFallSpeed + " + bonus: " + currentFallSpeedBonus + ")");
        
        // Find the background by name at runtime
        GameObject background = GameObject.Find("placeholderBG");
        SpriteRenderer backgroundRenderer = background.GetComponent<SpriteRenderer>();
        backgroundMinY = backgroundRenderer.bounds.min.y;
        
        // Find the Gather script to track missed bottles
        gatherScript = FindObjectOfType<Gather>();
    }

    // Update is called once per frame
    void Update()
    {
        float bottleMinY = bottleRenderer.bounds.min.y;
        if (bottleMinY < backgroundMinY)
        {
            // Bottle was missed - inform the Gather script
            if (gatherScript != null)
            {
                gatherScript.BottleMissed();
            }
            
            Destroy(gameObject);
        }
    }
}
