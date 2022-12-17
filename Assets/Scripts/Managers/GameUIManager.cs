using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance {get; private set;}
    
    public Image HealthBar;
    public Image ManaBar;
    public Image BlackScreen;

    [Header("Black Screen fade information")]
    public float FadeTime;

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

    public IEnumerator BlackScreenFadeIn() {
        float currentTime = 0;

        while (currentTime < FadeTime) {
            currentTime += Time.deltaTime;

            Color screenColor = BlackScreen.color;

            screenColor.a = currentTime / FadeTime;

            BlackScreen.color = screenColor;
            yield return null;
        }

    }

    public IEnumerator BlackScreenFadeOut() {
        float currentTime = FadeTime;

        while (currentTime > 0) {
            currentTime -= Time.deltaTime;

            Color screenColor = BlackScreen.color;

            screenColor.a = currentTime / FadeTime;

            BlackScreen.color = screenColor;
            yield return null;
        }

    }

}
