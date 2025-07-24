using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    private List<GameObject> girlSelections = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        // Force complete cleanup before creating anything new
        CleanupExistingSelections();
        
        // Create GameData singleton to persist girl selection
        if (GameData.Instance == null)
        {
            GameObject gameDataObject = new GameObject("GameData");
            gameDataObject.AddComponent<GameData>();
        }
        
        // Force garbage collection to ensure clean state
        System.GC.Collect();
        
        CreateGirlSelections();
    }
    
    void CleanupExistingSelections()
    {
        // Find and destroy any existing girl selection objects
        GameObject[] existingSelections = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in existingSelections)
        {
            if (obj.name.StartsWith("GirlSelection_"))
            {
                Debug.Log("BUILD DEBUG: Destroying existing selection: " + obj.name);
                Destroy(obj);
            }
        }
        
        // Clear our list
        girlSelections.Clear();
        
        Debug.Log("BUILD DEBUG: Cleaned up existing selections");
    }

    void CreateGirlSelections()
    {
        Debug.Log("BUILD DEBUG: Starting CreateGirlSelections - SCREEN CHECK MODE");
        
        // Check if girl selections already exist on screen
        GameObject[] existingGirlSelections = new GameObject[6];
        bool foundExistingSelections = false;
        
        for (int i = 0; i < 6; i++)
        {
            GameObject existing = GameObject.Find("GirlSelection_" + i);
            if (existing != null)
            {
                existingGirlSelections[i] = existing;
                foundExistingSelections = true;
                Debug.Log("BUILD DEBUG: Found existing GirlSelection_" + i + " on screen");
            }
        }
        
        if (foundExistingSelections)
        {
            Debug.Log("BUILD DEBUG: Girl selections already exist on screen - using existing ones");
            // Add existing selections to our list
            for (int i = 0; i < 6; i++)
            {
                if (existingGirlSelections[i] != null)
                {
                    girlSelections.Add(existingGirlSelections[i]);
                }
            }
            return; // Exit early - use what's already on screen
        }
        
        Debug.Log("BUILD DEBUG: No existing girl selections found - creating new ones");
        
        // SIMPLE HARDCODED APPROACH - No resource detection, just create what we know
        List<string> availableGirlTypes = new List<string>();
        availableGirlTypes.Add("Blonde");
        availableGirlTypes.Add("Brunette");
        
        Debug.Log("BUILD DEBUG: HARDCODED - Using fixed list: " + string.Join(", ", availableGirlTypes.ToArray()));
        
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
                // SIMPLIFIED SPRITE LOADING - Just load once, no retries during creation
                string spritePath = "Girls/" + availableGirlTypes[i] + "/1";
                Sprite girlSprite = Resources.Load<Sprite>(spritePath);
                
                if (girlSprite != null)
                {
                    sr.sprite = girlSprite;
                    Debug.Log("BUILD DEBUG: SIMPLE LOAD SUCCESS - " + availableGirlTypes[i] + " at path: " + spritePath);
                }
                else
                {
                    // This should never happen for Blonde/Brunette since we bypass detection
                    Debug.LogError("BUILD DEBUG: SIMPLE LOAD FAILED - " + availableGirlTypes[i] + " at path: " + spritePath + " - Using TBA");
                    sr.sprite = tbaSprite;
                }
                
                // Make girl clickable only if sprite loaded successfully
                if (sr.sprite != tbaSprite)
                {
                    Debug.Log("BUILD DEBUG: Adding components to " + availableGirlTypes[i] + " (index " + i + ")");
                    
                    try
                    {
                        BoxCollider2D collider = girlSelection.AddComponent<BoxCollider2D>();
                        collider.isTrigger = true; // For mouse detection
                        Debug.Log("BUILD DEBUG: BoxCollider2D added successfully to " + availableGirlTypes[i]);
                        
                        // Add GirlSelection component for interaction
                        GirlSelectionButton button = girlSelection.AddComponent<GirlSelectionButton>();
                        Debug.Log("BUILD DEBUG: GirlSelectionButton component added to " + availableGirlTypes[i]);
                        
                        button.girlType = availableGirlTypes[i];
                        button.spriteRenderer = sr;
                        Debug.Log("BUILD DEBUG: GirlSelectionButton configured - girlType: " + button.girlType + ", spriteRenderer: " + (button.spriteRenderer != null ? "OK" : "NULL"));
                        
                        Debug.Log("BUILD DEBUG: Created clickable girl selection " + i + " with " + availableGirlTypes[i] + " at X position " + xPositions[i]);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("BUILD DEBUG: EXCEPTION adding components to " + availableGirlTypes[i] + ": " + e.Message);
                    }
                }
            }
            else
            {
                // Use TBA sprite for empty slots (no collider = not clickable)
                sr.sprite = tbaSprite;
                Debug.Log("BUILD DEBUG: Created TBA selection " + i + " at X position " + xPositions[i]);
            }
            
            // Set position (X from array, Y = 0, Z = 0) and scale = 1
            girlSelection.transform.position = new Vector3(xPositions[i], 0, 0);
            girlSelection.transform.localScale = Vector3.one; // Scale = 1
            
            // FORCE SCALE PROTECTION - Ensure scale never becomes 0
            if (girlSelection.transform.localScale.x <= 0 || girlSelection.transform.localScale.y <= 0)
            {
                Debug.LogWarning("BUILD DEBUG: Scale corruption detected for " + girlSelection.name + ", forcing to Vector3.one");
                girlSelection.transform.localScale = Vector3.one;
            }
            
            // Add to list for future reference
            girlSelections.Add(girlSelection);
        }
        
        Debug.Log("BUILD DEBUG: Created 6 selections: " + availableGirlTypes.Count + " girls + " + (6 - availableGirlTypes.Count) + " TBA placeholders");
    }

    // Update is called once per frame
    void Update()
    {
        // Removed verbose debug logging - scale protection is working
    }
}
