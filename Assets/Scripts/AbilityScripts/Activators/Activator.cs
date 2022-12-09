using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public abstract class Activator : MonoBehaviour
{
    public Animator Anim;
    
    private float _abilityCoolDown;
    private float _abilityActivationTime;
    private float _maxAbilityDuration;
    private MMF_Player _feedbackPlayer;

    private KeyCode _keyToActivate;

    private string _anticipationFeedbackLabel;
    private MMF_Sound _anticipationSound;
    private MMF_Particles _anticipationParticles;
    private float _anticipationDuration;

    private string _castFeedbackLabel;
    private MMF_Sound _castSound;
    private MMF_Particles _castParticles;
    private float _castDuration;

    private float _deathDuration;

    public void SetPlayer(string name) {
        _feedbackPlayer = transform.Find(name).GetComponent<MMF_Player>();

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
            if (sounds[i].Label == _anticipationFeedbackLabel + "Particles") {
                _anticipationParticles = particles[i];
            } else {
                _castParticles = particles[i];
            }
        }
    }

    public void SetAbilityCooldown(float duration) {
        _abilityCoolDown = duration;
    }
    public void SetMaxAbilityDuration(float duration) {
        _maxAbilityDuration = duration;
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
    
    public abstract void Activate();

    public abstract IEnumerator AnticipationAction();
    public abstract IEnumerator CastAction();
}
