using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BottleFall : MonoBehaviour
{
    private float backgroundMinY;
    private SpriteRenderer bottleRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        bottleRenderer = GetComponent<SpriteRenderer>();
        
        // Find the background by name at runtime
        GameObject background = GameObject.Find("placeholderBG");
        SpriteRenderer backgroundRenderer = background.GetComponent<SpriteRenderer>();
        backgroundMinY = backgroundRenderer.bounds.min.y;
    }

    // Update is called once per frame
    void Update()
    {
        float bottleMinY = bottleRenderer.bounds.min.y;
        if (bottleMinY < backgroundMinY)
        {
            Destroy(gameObject);
            //maybe add counter for destroyed bottle? or this
            // maybe only is PER bottle, better in spawner maybe
        }

    }
}
