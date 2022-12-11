using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public abstract class Activator : MonoBehaviour
{
    public Animator Anim;
    public Entity ControllingEntity;

    protected bool _activated;

    protected string _enemyTag;

    protected int _manaCost;
    
    protected float _abilityCoolDown;
    protected float _abilityActivationTime;
    protected float _maxAbilityDuration;
    protected float _castDuration;

    protected float _deathDuration;
    protected MMF_Player _feedbackPlayer;

    protected KeyCode _keyToActivate;

    protected string _anticipationFeedbackLabel;
    protected string _anticipationAnimationParameterName;
    protected MMF_Sound _anticipationSound;
    protected MMF_Particles _anticipationParticles;
    protected float _anticipationDuration;

    protected string _castFeedbackLabel;
    protected string _castAnimationParameterName;
    protected MMF_Sound _castSound;
    protected MMF_Particles _castParticles;

    public void SetFeedbackPlayer(string name) {
        _feedbackPlayer = transform.Find(name).GetComponent<MMF_Player>();

        if (_feedbackPlayer == null) {
            Debug.LogWarning("Please specify the name of the Feedback for this ability");
        }

        List<MMF_Sound> sounds = _feedbackPlayer.GetFeedbacksOfType<MMF_Sound>();
        List<MMF_Particles> particles = _feedbackPlayer.GetFeedbacksOfType<MMF_Particles>();
        
        for (int i = 0; i < sounds.Count; i++) {
            if (sounds[i].Label == _anticipationFeedbackLabel + "Sound") {
                _anticipationSound = sounds[i];
            } else {
                _castSound = sounds[i];
            }
        }

        for (int i = 0; i < particles.Count; i++) {
            if (particles[i].Label == _anticipationFeedbackLabel + "Particles") {
                _anticipationParticles = particles[i];
            } else {
                _castParticles = particles[i];
            }
        }
    }

    public void SetAnticipationParameterName(string name) {
        _anticipationAnimationParameterName = name;
    }

    public void SetCastParameterName(string name) {
        _castAnimationParameterName = name;
    }

    public void SetAnticipationFeedbackLabel(string name) {
        _anticipationFeedbackLabel = name;
    }

    public void SetCastFeedbackLabel(string name) {
        _castFeedbackLabel = name;
    }

    public void SetAbilityCooldown(float duration) {
        _abilityCoolDown = duration;
    }
    public void SetMaxAbilityDuration(float duration) {
        _maxAbilityDuration = duration;
    }

    public void SetKeyToActivate(KeyCode key) {
        _keyToActivate = key;
    }

    public void SetAnticipationDuration(float duration) {
        _anticipationDuration = duration;
    }

    public void SetCastDuration(float duration) {
        _castDuration = duration;
    }

    public void SetDeathDuration(float duration) {
        _deathDuration = duration;
    }

    public void SetManaCost(int cost) {
        _manaCost = cost;
    }

    public void SetEnemyTag(string tag) {
        _enemyTag = tag;
    }

    public bool GetActivated() {
        return _activated;
    }
    
    public abstract void Activate();

    public abstract IEnumerator AnticipationAction();
    public abstract IEnumerator CastAction();
}
