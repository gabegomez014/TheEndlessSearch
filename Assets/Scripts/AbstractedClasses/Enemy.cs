using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public abstract class Enemy : Entity
{
    [Header("Enemy AI Sight information")]
    public Transform SightPosition;
    public float SightDistance;

    [Header("Enemy AI Waypoint information")]
    public Transform[] Waypoints;
    public float TimeSpentAtWaypoint = 1;
    
    [Header("Enemy AI Movement information")]
    public float RotationSpeed;
    public float AttackMovementSpeed;
    public float AttackDistance;

    public float MaxChaseDistance;
    public float PauseBeforeChasing;
    public float SleepTime;
    public float StunTime;
    public float DeathTime;
    [Header("FX Players")]
    public MMF_Player WalkPlayer;
    public MMF_Player ChaseStartPlayer;
    public MMF_Player ChasePlayer;
    public MMF_Player HitPlayer;
    public MMF_Player DeathPlayer;
    public MMF_Player SleepPlayer;

    protected bool _patrolDirection; // True = forward, false = backwards
    protected bool _isRotating;
    protected int _currentWayPoint;
    protected int _nextWayPoint;
    protected Vector3 _chaseStartPosition;
    protected PlayerController _player;


    protected AIState _state = AIState.Idle;

    protected abstract void Detection();

    protected abstract void Chasing();

    protected abstract void Attacking();

    protected abstract void Sleeping();

    public override void TakeDamage(int damage) {
        _currentHealth -= damage;
        if (_currentHealth < 0) {
            Debug.Log("Dead");
            _collider.enabled = false;
            StartCoroutine(Dying());
            _state = AIState.Dead;
        } else {
            StartCoroutine(Hit());
        }
    }

    protected virtual void ResetEnemy() {
        _collider.enabled = true;
        _isRotating = false;
        _state = AIState.Idle;
        transform.position = Waypoints[0].position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(_dir,0));
        EntityMeshModel.transform.rotation = rot;

    }

    public virtual void Revive() {
        _state = AIState.Idle;
    }

    protected virtual IEnumerator Hit() {
        AIState currentState = _state;
        _state = AIState.Hit;
        yield return new WaitForSeconds(StunTime);
        _state = currentState;
    }

    protected abstract void WaypointUpdate();

    protected abstract IEnumerator Chase(bool startFlag = true);

    protected abstract IEnumerator Patrol();

    protected abstract IEnumerator Attack();

    protected abstract IEnumerator Dying();

    protected abstract IEnumerator Sleep();
}
