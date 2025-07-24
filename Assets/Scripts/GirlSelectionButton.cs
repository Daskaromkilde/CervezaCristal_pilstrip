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
        
        // SCALE PROTECTION - Ensure originalScale is never 0 or invalid
        if (originalScale.x <= 0 || originalScale.y <= 0 || originalScale.z <= 0)
        {
            Debug.LogWarning("GirlSelectionButton: Invalid originalScale detected for " + girlType + ", forcing to Vector3.one");
            originalScale = Vector3.one;
            transform.localScale = originalScale;
        }
        
        hoverScale = originalScale * 1.1f; // 10% larger on hover
    }
    
    void OnMouseEnter()
    {
        // SCALE PROTECTION - Ensure we have valid scales before applying hover effect
        if (hoverScale.x <= 0 || hoverScale.y <= 0 || hoverScale.z <= 0)
        {
            Debug.LogWarning("GirlSelectionButton: Invalid hoverScale for " + girlType + ", recalculating...");
            originalScale = Vector3.one;
            hoverScale = originalScale * 1.1f;
        }
        
        // Scale up sprite on hover for "pop out" effect
        transform.localScale = hoverScale;
    }
    
    void OnMouseExit()
    {
        // SCALE PROTECTION - Ensure we have valid original scale before restoring
        if (originalScale.x <= 0 || originalScale.y <= 0 || originalScale.z <= 0)
        {
            Debug.LogWarning("GirlSelectionButton: Invalid originalScale for " + girlType + ", forcing to Vector3.one");
            originalScale = Vector3.one;
        }
        
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
