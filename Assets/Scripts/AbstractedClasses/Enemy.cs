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
    public float MinimumDistanceFromEnemy;
    public float PauseBeforeChasing;
    public MMF_Player WalkPlayer;
    public MMF_Player ChaseStartPlayer;
    public MMF_Player ChasePlayer;

    protected bool _patrolDirection; // True = forward, false = backwards
    protected bool _isRotating;
    protected int _currentWayPoint;
    protected int _nextWayPoint;
    protected PlayerController _player;


    protected AIState _state = AIState.Idle;

    protected abstract void Detection();

    protected abstract void Chasing();

    public virtual void TakeDamage(int damage) {
        _currentHealth -= damage;
        if (_currentHealth < 0) {
            _state = AIState.Dead;
        }
    }

    protected virtual void ResetEnemy() {
        _isRotating = false;
        _state = AIState.Idle;
        transform.position = Waypoints[0].position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(_dir,0));
        EntityMeshModel.transform.rotation = rot;

    }

    public virtual void Revive() {
        _state = AIState.Idle;
    }

    protected abstract void WaypointUpdate();
}
