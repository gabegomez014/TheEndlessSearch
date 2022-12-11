using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
    [Header("Enemy AI information")]
    public Transform[] Waypoints;
    public float TimeSpentAtWaypoint = 1;
    public float RotationSpeed;

    protected bool _isDead;
    protected bool _patrolDirection; // True = forward, false = backwards
    protected int _currentWayPoint;
    protected int _nextWayPoint;


    protected AIState _state = AIState.Idle;

    protected abstract void Detection();

    protected abstract void Movement();

    public virtual void TakeDamage(int damage) {
        _currentHealth -= damage;
        if (_currentHealth < 0) {
            _isDead = true;
        }
    }

    protected virtual void ResetEnemy() {
        transform.position = Waypoints[0].position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(_dir,0));
        EntityMeshModel.transform.rotation = rot;

    }

    public virtual void Revive() {
        _isDead = false;
    }

    protected abstract void WaypointUpdate();
}
