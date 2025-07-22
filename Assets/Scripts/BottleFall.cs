using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BottleFall : MonoBehaviour
{
    private float backgroundMinY;
    private SpriteRenderer bottleRenderer;
    private Gather gatherScript;
    
    // Start is called before the first frame update
    void Start()
    {
        bottleRenderer = GetComponent<SpriteRenderer>();
        
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
