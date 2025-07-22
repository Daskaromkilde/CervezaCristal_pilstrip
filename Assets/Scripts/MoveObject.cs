using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private float xMin, xMax;
    private float paddingPercentage = 0.86f;
    public GameObject background; // Assign your background in the inspector

    // Start is called before the first frame update
    void Start()
    {
        // Get the background's bounds
        Renderer backgroundRenderer = background.GetComponent<Renderer>();
        Bounds backgroundBounds = backgroundRenderer.bounds;
        xMax = Mathf.Floor(backgroundBounds.max.x * paddingPercentage);
        xMin = xMax * -1;
        Debug.Log("X bounds: " + xMin + " to " + xMax);
    }

    // Update is called once per frame
    void Update()
    {
        // Convert screen position to world position 
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Mathf.Clamp(value, min, max)
        float clampedX = Mathf.Clamp(mouseWorldPos.x, xMin, xMax);
        
        // Use only the x component from mouse, keep current y and z
        Vector3 tempV = new Vector3(clampedX, transform.position.y, transform.position.z);
        transform.position = tempV;
        //Debug.Log(transform.position);
    }
}
