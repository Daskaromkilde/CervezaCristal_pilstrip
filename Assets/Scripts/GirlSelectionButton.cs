using UnityEngine;
using UnityEngine.SceneManagement;

public class GirlSelectionButton : MonoBehaviour
{
    public string girlType;
    public SpriteRenderer spriteRenderer;
    
    private Vector3 originalScale;
    private Vector3 hoverScale;
    
    void Start()
    {
        originalScale = transform.localScale;
        hoverScale = originalScale * 1.1f; // 10% larger on hover
    }
    
    void OnMouseEnter()
    {
        // Scale up sprite on hover for "pop out" effect
        transform.localScale = hoverScale;
       // Debug.Log("Hovering over " + girlType);
    }
    
    void OnMouseExit()
    {
        // Return to original scale
        transform.localScale = originalScale;
    }
    
    void OnMouseDown()
    {
        // Store selected girl type in both PlayerPrefs and GameData for redundancy
        PlayerPrefs.SetString("SelectedGirlType", girlType);
        PlayerPrefs.Save();
        
        // Also store in GameData singleton if it exists
        if (GameData.Instance != null)
        {
            GameData.Instance.SetSelectedGirl(girlType);
        }
        
        Debug.Log("Selected girl type: " + girlType + ", loading game scene...");
        
        // Load the game scene (assuming it's at index 1 in Build Settings)
        SceneManager.LoadScene(1);
    }
}
