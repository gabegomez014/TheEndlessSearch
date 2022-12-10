using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashActivator : Activator
{
    private float _slashDistance;
    private int _damage;
    private bool _activated;


    private void Start() {
        _activated = false;
    }

    public void SetSlashDistance(float distance) {
        _slashDistance = distance;
    }
    public void SetDamage(int damage) {
        _damage = damage;
    }

    public bool GetActivated() {
        return _activated;
    }

    public override void Activate()
    {
        if (!_activated) {
            _activated = true;
            _feedbackPlayer.PlayFeedbacks();
            if (_anticipationDuration > 0) {
                StartCoroutine(AnticipationAction());
            } else {
                StartCoroutine(CastAction());
            }
        }
    }

    public override IEnumerator AnticipationAction()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator CastAction()
    {
        RaycastHit hit;
        
        _castSound.Play(transform.position);
        _castParticles.Play(transform.position);
        Anim.SetTrigger(_castAnimationParameterName);

        Physics.Raycast(transform.position, new Vector3(Player.GetDirection(), 0), out hit, _slashDistance);
        if (hit.transform && hit.transform.tag == "Enemy") {
            // Hit the enemy once we got some enemies going
            // Also spawn Hit effect
        }

        yield return new WaitForSeconds(_deathDuration);

        _activated = false;
    }
}
