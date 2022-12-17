using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionManager : MonoBehaviour
{
    public static SectionManager Instance {get; private set;}

    public Transform Player;

    private int _currentSection;

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

    public void SectionChange(int section, Transform entryPoint) {
        StartCoroutine(ChangeSection(section, entryPoint));
    }

    IEnumerator ChangeSection(int section, Transform entryPoint) {
        StartCoroutine(GameUIManager.Instance.BlackScreenFadeIn());
        yield return new WaitForSeconds(GameUIManager.Instance.FadeTime);

        this.transform.GetChild(_currentSection).gameObject.SetActive(false);
        this.transform.GetChild(section).gameObject.SetActive(true);

        _currentSection = section;

        Player.transform.position = entryPoint.position;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(GameUIManager.Instance.BlackScreenFadeOut());

    }
}
