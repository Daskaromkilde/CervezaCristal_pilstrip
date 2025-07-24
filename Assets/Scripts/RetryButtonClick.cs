using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetryButtonClick : MonoBehaviour
{
    public Gather gatherScript; // Reference to the Gather script
    
    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Debug.Log("RETRY BUTTON: Mouse clicked, checking if over button...");
            
            // Get the RectTransform of this UI element
            RectTransform rectTransform = GetComponent<RectTransform>();
            
            if (rectTransform != null)
            {
                // Convert mouse position to canvas coordinates
                Vector2 mousePosition = Input.mousePosition;
                
                // Check if mouse is within the button's rect bounds
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition, Camera.main))
                {
                    Debug.Log("RETRY BUTTON: Button clicked! Calling RetryGame()");
                    if (gatherScript != null)
                    {
                        gatherScript.RetryGame();
                    }
                    else
                    {
                        Debug.LogError("RETRY BUTTON: gatherScript is null!");
                    }
                }
                else
                {
                    Debug.Log("RETRY BUTTON: Click missed button. Mouse at: " + mousePosition + ", Button bounds not hit");
                }
            }
            else
            {
                Debug.LogError("RETRY BUTTON: No RectTransform found!");
            }
        }
    }
}
