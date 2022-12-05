using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance {get; private set;}
    
    public Image HealthBar;
    public Image ManaBar;

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void UpdateHealth(float percent) {
        HealthBar.fillAmount = percent;
    }

    public void UpdateMana(float percent) {
        ManaBar.fillAmount = percent;
    }

}
