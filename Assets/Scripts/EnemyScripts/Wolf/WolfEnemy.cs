using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class WolfEnemy : Enemy
{

    public SlashAbility attack;

    protected override void Start() {
        base.Start();
        ResetEnemy();
        _patrolDirection = true;
        _isRotating = false;   

        _currentWayPoint = 0;
        _nextWayPoint = 1;

        if (Waypoints.Length > 0) {
            StartCoroutine(Patrol());
        }
    }
    private void Update() {
        if ((_state == AIState.Idle || _state == AIState.Sleep) && !_isRotating) {
            Detection();
        } else if (_state == AIState.Chase) {
            Chasing();
        } else if (_state == AIState.Attack) {
            Attacking();
        }

        if (_state == AIState.Sleep) {
            Sleeping();
        }
    }
    protected override void Detection()
    {
        RaycastHit hit;
        Physics.Raycast(SightPosition.position, new Vector3(_dir, 0), out hit, SightDistance);

        if (hit.transform && hit.transform.tag == EnemyTag) {
            _chaseStartPosition = this.transform.position;
            _player = hit.transform.GetComponent<PlayerController>();
            _state = AIState.Chase;
            StartCoroutine(Chase());
        }
    }

    protected override void Chasing()
    {
        if (Vector3.Distance(this.transform.position, _player.transform.position) <= AttackDistance) {
            _state = AIState.Attack;
            StartCoroutine(Attack());
        } else if (Vector3.Distance(this.transform.position, _chaseStartPosition) >= MaxChaseDistance) {
            Debug.Log("Sleep");
            _state = AIState.Sleep;
            _player = null;
            StartCoroutine(Sleep());
        }
    }

    protected override void Attacking()
    {
        if (Vector3.Distance(this.transform.position, _player.transform.position) > AttackDistance) {
            _state = AIState.Chase;
            _anim.SetTrigger("AttackChase");
            StartCoroutine(Chase(false));
        }
    }

    protected override void Sleeping()
    {
        if (Vector3.Distance(this.transform.position, Waypoints[_currentWayPoint].position) < 0.5f) {
            _state = AIState.Idle;
            StartCoroutine(Patrol());
        }
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

    protected override IEnumerator Hit() {
        AIState currentState = _state;
        _state = AIState.Hit;
        _anim.SetTrigger("Hit");
        HitPlayer.PlayFeedbacks();
        yield return new WaitForSeconds(StunTime);
        HitPlayer.StopFeedbacks();
        _state = AIState.Attack;
    }

    protected override IEnumerator Chase(bool startFlag = true) {


        if (startFlag) {
            ChaseStartPlayer.PlayFeedbacks();
            _anim.SetTrigger("ChaseStart");

            yield return new WaitForSeconds(PauseBeforeChasing);
        }
        
        _anim.SetBool("Chasing", true);
        ChaseStartPlayer.StopFeedbacks();
        ChasePlayer.PlayFeedbacks();

        while (_state == AIState.Chase) {
             _dir = (_player.transform.position - this.transform.position).normalized.x;
            if (_dir <= -1) {
                _dir = -1;
            } else if (_dir >= 1) {
                _dir = 1;
            }

            Quaternion rot = Quaternion.LookRotation(new Vector3(_dir,0));

            _isRotating = true;
            while (EntityMeshModel.transform.rotation != rot && _state == AIState.Chase) {
                EntityMeshModel.transform.rotation = Quaternion.Slerp(EntityMeshModel.transform.rotation, rot, RotationSpeed * Time.deltaTime);
                yield return null;
            }
            _isRotating = false;

            transform.position = transform.position + new Vector3(_dir * AttackMovementSpeed, 0, 0) * Time.deltaTime;
            yield return null;
        }

        _anim.SetBool("Chasing", false);
        ChasePlayer.StopFeedbacks();

        yield return null;
    }

    protected override IEnumerator Patrol() {

        _dir = (Waypoints[_nextWayPoint].position - Waypoints[_currentWayPoint].position).normalized.x;
        if (_dir <= -1) {
            _dir = -1;
        } else if (_dir >= 1) {
            _dir = 1;
        }

        Quaternion rot = Quaternion.LookRotation(new Vector3(_dir,0));
        EntityMeshModel.transform.rotation = rot;
        _anim.SetBool("Walking", true);

        if (!WalkPlayer.IsPlaying) {
            WalkPlayer.PlayFeedbacks();
        }
        
        while(_state == AIState.Idle) {
            transform.position = transform.position + new Vector3(_dir * MovementSpeed, 0, 0) * Time.deltaTime;

            if (Vector3.Distance(transform.position, Waypoints[_nextWayPoint].position) < 0.5f) {

                WalkPlayer.StopFeedbacks();
                _anim.SetBool("Walking", false);
                WaypointUpdate();
                yield return new WaitForSeconds(TimeSpentAtWaypoint);
                rot = Quaternion.LookRotation(new Vector3(_dir, 0));
                _isRotating = true;
                while (EntityMeshModel.transform.rotation != rot) {
                    EntityMeshModel.transform.rotation = Quaternion.Slerp(EntityMeshModel.transform.rotation, rot, RotationSpeed * Time.deltaTime);
                    yield return null;
                }
                _isRotating = false;

                if (_state != AIState.Idle) {
                    yield break;
                }

                _anim.SetBool("Walking", true);
                WalkPlayer.PlayFeedbacks();
            }
            yield return null;
        }

        WalkPlayer.StopFeedbacks();
        _anim.SetBool("Walking", false);
    }

    protected override IEnumerator Attack() {

        _anim.SetBool("Attacking", true);
        while (_state == AIState.Attack) {
            _dir = (_player.transform.position - this.transform.position).normalized.x;
            if (_dir <= -1) {
                _dir = -1;
            } else if (_dir >= 1) {
                _dir = 1;
            }

            Quaternion rot = Quaternion.LookRotation(new Vector3(_dir,0));

            _isRotating = true;
            while (EntityMeshModel.transform.rotation != rot && _state == AIState.Attack) {
                EntityMeshModel.transform.rotation = Quaternion.Slerp(EntityMeshModel.transform.rotation, rot, RotationSpeed * Time.deltaTime);
                yield return null;
            }
            _isRotating = false;

            yield return null;
        }
        yield return null;
    }

    protected override IEnumerator Dying()
    {
        _anim.SetTrigger("Dead");
        DeathPlayer.PlayFeedbacks();
        yield return new WaitForSeconds(DeathTime);
        DeathPlayer.StopFeedbacks();
    }

    protected override IEnumerator Sleep() {
        _anim.SetBool("Attacking", false);
        _anim.SetBool("Chasing", false);
        ChasePlayer.StopFeedbacks();
        ChaseStartPlayer.StopFeedbacks();

        _anim.SetTrigger("Sleep");
        SleepPlayer.PlayFeedbacks();

        yield return new WaitForSeconds(SleepTime);

        SleepPlayer.StopFeedbacks();

        _dir = (Waypoints[_currentWayPoint].position - this.transform.position).normalized.x;
        if (_dir <= -1) {
            _dir = -1;
        } else if (_dir >= 1) {
            _dir = 1;
        }

        Quaternion rot = Quaternion.LookRotation(new Vector3(_dir,0));

        _isRotating = true;
        while (EntityMeshModel.transform.rotation != rot && _state == AIState.Sleep) {
            EntityMeshModel.transform.rotation = Quaternion.Slerp(EntityMeshModel.transform.rotation, rot, RotationSpeed * Time.deltaTime);
            yield return null;
        }

        _anim.SetBool("Walking", true);
        WalkPlayer.PlayFeedbacks();

        while (_state == AIState.Sleep) {
            this.transform.position = this.transform.position + new Vector3(_dir, 0) * Time.deltaTime * MovementSpeed;

            _dir = (Waypoints[_currentWayPoint].position - this.transform.position).normalized.x;
            if (_dir <= -1) {
                _dir = -1;
            } else if (_dir >= 1) {
                _dir = 1;
            }

            rot = Quaternion.LookRotation(new Vector3(_dir,0));

            _isRotating = true;
            while (EntityMeshModel.transform.rotation != rot && _state == AIState.Sleep) {
                EntityMeshModel.transform.rotation = Quaternion.Slerp(EntityMeshModel.transform.rotation, rot, RotationSpeed * Time.deltaTime);
                yield return null;
            }
            _isRotating = false;
            
            yield return null;
        }

        if (_state != AIState.Idle) {
            WalkPlayer.StopFeedbacks();
        }
        yield return null;
    }
}
