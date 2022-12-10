
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string AbilityName = "New Ability";
    public int ManaCost;
    public float AbilityBaseCoolDown = 1f;
    public float AbilityActivationTime;
    public KeyCode KeyToActivate; 
    public float MaxAbilityDuration;
    public string FeedbacksPlayerName;
    [Header("Ability Anticipation information")]
    public string AnticipationFeedbackLabel;
    public string AnticipationAnimationParameterName;
    public float AnticipationDuration;

    [Header("Ability Casting information")]
    public string CastFeedbackLabel;
    public string CastAnimationParameterName;
    public float CastDuration;

    [Header("Ability Death information")]
    public float DeathDuration;

    [Header("Ability Sprite and UI information")]
    public Sprite AbilitySprite;

    public abstract void Initialize(GameObject obj);
    public abstract void TriggerAbility();
}
