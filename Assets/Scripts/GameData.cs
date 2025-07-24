using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public string selectedGirlType = "Blonde"; // Default to Blonde
    
    void Awake()
    {
        // Singleton pattern - keep this object alive between scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameData created and persisted between scenes");
        }
        else
        {
            // If another instance exists, destroy this one
            Destroy(gameObject);
        }
    }
    
    public void SetSelectedGirl(string girlType)
    {
        selectedGirlType = girlType;
        Debug.Log("Selected girl type set to: " + girlType);
    }
    
    public string GetSelectedGirl()
    {
        Debug.Log("Retrieved selected girl type: " + selectedGirlType);
        return selectedGirlType;
    }
}
