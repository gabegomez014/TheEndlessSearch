using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfEnemy : Enemy
{
    protected override void Start() {
        base.Start();
        ResetEnemy();
        _patrolDirection = true;

        _currentWayPoint = 0;
        _nextWayPoint = 1;

        if (Waypoints.Length > 0) {
            StartCoroutine(Patrol());
        }
    }
    // private void Update() {
    //     if (_state == AIState.Idle) {
    //         Detection();
    //         Movement();
    //     }
    // }
    protected override void Detection()
    {
        
    }

    protected override void Movement()
    {
    }

    protected override void WaypointUpdate() {
        _currentWayPoint = _nextWayPoint;

        if (_patrolDirection) {
            _nextWayPoint += 1;

            if (_nextWayPoint >= Waypoints.Length) {
                _nextWayPoint = Waypoints.Length - 2;
                _patrolDirection = false;
            }
        } else {
            _nextWayPoint -= 1;

            if (_nextWayPoint <= -1) {
                _nextWayPoint = 1;
                _patrolDirection = true;
            }
        }

        _dir = (Waypoints[_nextWayPoint].position - Waypoints[_currentWayPoint].position).normalized.x;
        if (_dir <= -1) {
            _dir = -1;
        } else if (_dir >= 1) {
            _dir = 1;
        }
    }

    private IEnumerator Patrol() {

        _dir = (Waypoints[_nextWayPoint].position - Waypoints[_currentWayPoint].position).normalized.x;
        if (_dir <= -1) {
            _dir = -1;
        } else if (_dir >= 1) {
            _dir = 1;
        }

        Quaternion rot = Quaternion.LookRotation(new Vector3(_dir,0));
        EntityMeshModel.transform.rotation = rot;
        _anim.SetBool("Walking", true);
        
        while(_state == AIState.Idle) {
            transform.position = transform.position + new Vector3(_dir * MovementSpeed, 0, 0) * Time.deltaTime;

            if (Vector3.Distance(transform.position, Waypoints[_nextWayPoint].position) < 0.5f) {

                _anim.SetBool("Walking", false);
                WaypointUpdate();
                yield return new WaitForSeconds(TimeSpentAtWaypoint);
                rot = Quaternion.LookRotation(new Vector3(_dir, 0));
                while (EntityMeshModel.transform.rotation != rot) {
                    EntityMeshModel.transform.rotation = Quaternion.Slerp(EntityMeshModel.transform.rotation, rot, RotationSpeed * Time.deltaTime);
                    yield return null;
                }
                _anim.SetBool("Walking", true);
            }

            yield return null;
        }
    }
}
