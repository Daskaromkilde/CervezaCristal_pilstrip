using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Girl : MonoBehaviour
{
    public string girlType = "Blonde"; // Can be changed at runtime or set in Inspector
    public Sprite[] girlSprites; // Will hold the 3 sprites for current girl type
    public int currentSprite = 0; // Which sprite to show (0, 1, or 2)
    
    private Vector3 calculatedScale; // Store the calculated scale to use for all sprites
    
    // Start is called before the first frame update
    void Start()
    {
        LoadGirlSprites();
        CalculateScaleOnce();
        ShowCurrentSprite();
    }

    void LoadGirlSprites()
    {
        // Load the sprites from Assets/Girls/{girlType}/
        girlSprites = new Sprite[3];
        string basePath = "Girls/" + girlType + "/";
        
        girlSprites[0] = Resources.Load<Sprite>(basePath + "1");
        girlSprites[1] = Resources.Load<Sprite>(basePath + "2");
        girlSprites[2] = Resources.Load<Sprite>(basePath + "3");
        
        // Debug to check if sprites loaded
       // Debug.Log("Loading girl type: " + girlType);
        for (int i = 0; i < girlSprites.Length; i++)
        {
            if (girlSprites[i] == null)
                Debug.LogError("Failed to load " + girlType + " sprite " + (i + 1) + " from path: " + basePath + (i + 1));
            else
                Debug.Log("Loaded " + girlType + " sprite " + (i + 1));
        }
    }
    
    void CalculateScaleOnce()
    {
        // Calculate scale once based on camera and target height
        // This will be the same for all sprites since they have same PPU
        Camera cam = Camera.main;
        float cameraHeight = cam.orthographicSize * 2f;
        float targetHeight = cameraHeight; // Full height like background
        
        // Use first sprite to calculate scale (all sprites have same PPU so same scale)
        if (girlSprites[0] != null)
        {
            Vector2 spriteSize = girlSprites[0].bounds.size;
            float scaleValue = targetHeight / spriteSize.y;
            calculatedScale = new Vector3(scaleValue, scaleValue, 1f);
            
            //Debug.Log("Calculated scale for " + girlType + ": " + calculatedScale);
        }
        else
        {
            Debug.LogError("Cannot calculate scale - no sprites loaded!");
            calculatedScale = Vector3.one; // Fallback to no scaling
        }
    }
    
    public void ShowCurrentSprite()
    {
        if (girlSprites != null && girlSprites.Length > currentSprite && girlSprites[currentSprite] != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = girlSprites[currentSprite];
            
            // Apply the pre-calculated scale
            transform.localScale = calculatedScale;
            
            //Debug.Log("Showing " + girlType + " sprite " + (currentSprite + 1));
        }
        else
        {
            Debug.LogError("Cannot show sprite " + (currentSprite + 1) + " - sprite is null or array is empty");
        }
    }
    
    // Method to change girl type at runtime
    public void ChangeGirlType(string newGirlType)
    {
        girlType = newGirlType;
        LoadGirlSprites();
        CalculateScaleOnce();
        currentSprite = 0; // Reset to first sprite
        ShowCurrentSprite();
        Debug.Log("Changed to girl type: " + newGirlType);
    }
    
    // Update is called once per frame
    void Update() // TODO THIS NEEDS TO BE REMOVED BEFORE SHIP
    {
        // Change sprite with number keys for testing
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        { 
            currentSprite = 0; 
            ShowCurrentSprite(); 
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        { 
            currentSprite = 1; 
            ShowCurrentSprite(); 
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) 
        { 
            currentSprite = 2; 
            ShowCurrentSprite(); 
        }
    }
}

