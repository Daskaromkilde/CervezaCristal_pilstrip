using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    private List<GameObject> girlSelections = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        CreateGirlSelections();
    }

    void CreateGirlSelections()
    {
        // Manually check for girl types by trying to load known folders
        // You can expand this list as you add more girl types
        string[] possibleGirlTypes = { "Blonde", "Brunette", "Redhead", "Asian", "Latina", "Black" };
        List<string> availableGirlTypes = new List<string>();
        
        // Check which girl types actually exist
        foreach (string girlType in possibleGirlTypes)
        {
            Sprite testSprite = Resources.Load<Sprite>("Girls/" + girlType + "/1");
            if (testSprite != null)
            {
                availableGirlTypes.Add(girlType);
                Debug.Log("Found girl type: " + girlType);
            }
        }
        
        Debug.Log("Found " + availableGirlTypes.Count + " girl types total");
        
        // Load TBA sprite for empty slots
        Sprite tbaSprite = Resources.Load<Sprite>("TBA/ToBeAdded");
        
        if (tbaSprite == null)
        {
            Debug.LogError("Could not load TBA sprite! Make sure it's in Resources/TBA/ToBeAdded.png");
            return;
        }
        
        // Hardcoded X positions: -118, -70, -22, 26, 74, 122
        float[] xPositions = { -118f, -70f, -22f, 26f, 74f, 122f };
        
        // Create 6 girl GameObjects
        for (int i = 0; i < 6; i++)
        {
            // Create GameObject
            GameObject girlSelection = new GameObject("GirlSelection_" + i);
            
            // Add SpriteRenderer
            SpriteRenderer sr = girlSelection.AddComponent<SpriteRenderer>();
            
            // Determine which sprite to use
            if (i < availableGirlTypes.Count)
            {
                // Load girl sprite (sprite "1" from the girl type folder)
                Sprite girlSprite = Resources.Load<Sprite>("Girls/" + availableGirlTypes[i] + "/1");
                sr.sprite = girlSprite;
                
                // Make girl clickable
                BoxCollider2D collider = girlSelection.AddComponent<BoxCollider2D>();
                collider.isTrigger = true; // For mouse detection
                
                // Add GirlSelection component for interaction
                GirlSelectionButton button = girlSelection.AddComponent<GirlSelectionButton>();
                button.girlType = availableGirlTypes[i];
                button.spriteRenderer = sr;
                
                Debug.Log("Created clickable girl selection " + i + " with " + availableGirlTypes[i] + " at X position " + xPositions[i]);
            }
            else
            {
                // Use TBA sprite for empty slots (no collider = not clickable)
                sr.sprite = tbaSprite;
                Debug.Log("Created TBA selection " + i + " at X position " + xPositions[i]);
            }
            
            // Set position (X from array, Y = 0, Z = 0) and scale = 1
            girlSelection.transform.position = new Vector3(xPositions[i], 0, 0);
            girlSelection.transform.localScale = Vector3.one; // Scale = 1
            
            // Add to list for future reference
            girlSelections.Add(girlSelection);
        }
        
        Debug.Log("Created 6 selections: " + availableGirlTypes.Count + " girls + " + (6 - availableGirlTypes.Count) + " TBA placeholders");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
