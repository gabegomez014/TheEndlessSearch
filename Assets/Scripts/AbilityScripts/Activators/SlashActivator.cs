using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class SlashActivator : Activator
{
    private float _slashDistance;
    private int _damage;


    private void Start() {
        _activated = false;
    }

    public void SetSlashDistance(float distance) {
        _slashDistance = distance;
    }
    public void SetDamage(int damage) {
        _damage = damage;
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

        _castParticles.Mode = MMF_Particles.Modes.Play;
        _castSound.Active = true;
        _castSound.Play(transform.position);
        _castParticles.Play(transform.position);
        Anim.SetTrigger(_castAnimationParameterName);

        Physics.Raycast(transform.position, new Vector3(ControllingEntity.GetDirection(), 0), out hit, _slashDistance);
        if (hit.transform && hit.transform.tag == _enemyTag) {
            // Hit the enemy once we got some enemies going
            // Also spawn Hit effect
            Entity enemy = hit.transform.GetComponent<Entity>();
            enemy.TakeDamage(_damage);
        }

        yield return new WaitForSeconds(_deathDuration);
        _castParticles.Stop(transform.position);
        _castSound.Stop(transform.position);
        _castParticles.Mode = MMF_Particles.Modes.Stop;
        _castSound.Active = false;
        _feedbackPlayer.StopFeedbacks();

        yield return new WaitForSeconds(_abilityCoolDown);
        _activated = false;
    }
}
