using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private float xMin, xMax;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate the world bounds of the camera view
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        
        // Get the min/max x values based on camera position
        xMin = Camera.main.transform.position.x - cameraWidth / 2;
        xMax = Camera.main.transform.position.x + cameraWidth / 2;
        
        Debug.Log("X bounds: " + xMin + " to " + xMax);
    }

    // Update is called once per frame
    void Update()
    {
        // Convert screen position to world position 
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Clamp the x position to stay within camera bounds - Mathf.Clamp(value, min, max)
        float clampedX = Mathf.Clamp(mouseWorldPos.x, xMin, xMax);
        
        // Use only the x component from mouse, keep current y and z
        Vector3 tempV = new Vector3(clampedX, transform.position.y, transform.position.z);
        transform.position = tempV;
        //Debug.Log(transform.position);
    }
}
