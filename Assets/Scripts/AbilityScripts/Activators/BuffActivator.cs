using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class BuffActivator : Activator
{
    private int _increaseAmount;
    private Stats _statToEffect;

    private void Cast() {
        StartCoroutine(CastAction());
    }

    public void SetIncreaseAmount(int amount) {
        _increaseAmount = amount;
    }

    public void SetStatToEffect(Stats stat) {
        _statToEffect = stat;
    }

    public override void Activate()
    {
        if (!_activated) {
            _activated = true;
            if (_anticipationDuration > 0) {
            StartCoroutine(AnticipationAction());
            } else {
                StartCoroutine(CastAction());
            }
        }
    }

    public override IEnumerator AnticipationAction()
    {
        float currentTime = 0;
        
        Anim.ResetTrigger(_castAnimationParameterName);
        Anim.SetBool(_anticipationAnimationParameterName, true);
        _feedbackPlayer.PlayFeedbacks();
        _anticipationParticles.Mode = MMF_Particles.Modes.Play;
        _anticipationParticles.Play(transform.position);
        _anticipationParticles.Active = true;
        _anticipationParticles.Play(transform.position);
        _anticipationSound.Active = true;
        _anticipationSound.Play(transform.position);

        while (currentTime < _anticipationDuration && ControllingEntity.GetBuffing()) {
            currentTime += Time.deltaTime;
            yield return null;
        }

        _anticipationParticles.Stop(transform.position);
        _anticipationParticles.Mode = MMF_Particles.Modes.Stop;
        _anticipationSound.Stop(transform.position);
        _anticipationSound.Active = false;

         if (ControllingEntity.GetBuffing() == false && currentTime < _anticipationDuration) {
            // Buff Canceled
            _feedbackPlayer.StopFeedbacks();
            Anim.SetBool(_anticipationAnimationParameterName, false);
            _activated = false;
            yield break;
        }

        Cast();
    }

    public override IEnumerator CastAction()
    {
        float currentTime = 0;

        Anim.SetTrigger(_castAnimationParameterName);
        _castParticles.Mode = MMF_Particles.Modes.Play;
        _castParticles.Play(transform.position);
        _castSound.Active = true;
        _castSound.Play(transform.position);

        while(currentTime < _castDuration) {
            currentTime += Time.deltaTime;
            yield return null;
        }


        ControllingEntity.SetBuffing(false);
        ControllingEntity.ManaChange(-_manaCost);

        yield return new WaitForSeconds(_deathDuration);
        _castParticles.Stop(transform.position);
        _castParticles.Mode = MMF_Particles.Modes.Stop;
        _castSound.Stop(transform.position);
        _castSound.Active = false;
        _feedbackPlayer.StopFeedbacks();
        Anim.SetBool(_anticipationAnimationParameterName, false);

        switch(_statToEffect) {
            case Stats.Health:
                ControllingEntity.Heal(_increaseAmount);
                break;
        }

        yield return new WaitForSeconds(_abilityCoolDown);

        _activated = false;
    }
}
