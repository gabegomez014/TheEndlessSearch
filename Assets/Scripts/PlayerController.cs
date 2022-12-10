using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using Cinemachine;

public class PlayerController : Entity, IDamageable
{
    [Header("Player Model to manipulate")]
    public GameObject PlayerModel;

    [Header("Camera look at position")]
    public Transform CameraLookAt;

    [Header("Main Virtual Camera")]
    public CinemachineVirtualCamera VCam;

    [Header("Movement mechanics")]
    [Tooltip("Rotation speed")]
    public float SpeedRot;

    [Tooltip("Movement speed")]
    public float SpeedMove;

    [Tooltip("Feedback to play on Move")]
    public MMF_Player MovePlayer;

    [Header("Jump Mechanics")]
    [Tooltip("Max Jump height")]
    public float MaxJumpHeight;

    [Tooltip("Movement speed during jump")]
    public float JumpMove;

    [Tooltip("Drag on Player Jump")]
    public float CounterJumpForce;

    [Tooltip("Feedback to play on Jump")]
    public MMF_Player JumpPlayer;
    [Tooltip("Feedback to play on Jump Land")]
    public MMF_Player JumpLandPlayer;

    [Header("Dodge Mechanics")]
    [Tooltip("How far the dodge takes the player")]
    public float DodgeDistance;

    [Tooltip("How long the dodge lasts")]
    public float DodgeTime;
    [Tooltip("The feedback to play when the player dodges")]
    public MMF_Player DodgePlayer;

    [Header("Camera Settings")]
    public float CameraAimOffset;

    public float CameraLookAtOffset;

    [Header("Time available for Attack combo")]
    public float Term;

    [Header("Feedback to play on being hit")]
    public MMF_Player HitPlayer;

    [Header("Player abilities")]
    public SlashAbility[] AttackAbilities;
    public Ability HealAbility;

    private bool _isJump;

    private bool _jumpKeyHeld;

    private bool _isFalling;

    private bool _isDodging;

    private bool _isAttacking;

    private Animator _anim;

    private CinemachineComposer _composer;
    
    private Rigidbody _rb;

    private BuffActivator _buffActivator;
    private SlashActivator _slashActivator;

    float _raycastDistance = 1;
    RaycastHit _hit;
    float _currentDodgeTime;

    MMF_Sound _moveSound;
    MMF_Particles _moveParticles;
    MMF_Particles _jumpLandParticles;

    private new void Start() {
        base.Start();
        if (PlayerModel) {
            _anim = PlayerModel.GetComponent<Animator>();
        } else {
            Debug.LogWarning("No Player Model attached");
        }

        if (VCam) {
            _composer = VCam.GetCinemachineComponent<CinemachineComposer>();
        } else {
            Debug.LogWarning("Need to attach Virtual Camera");
        }

        _rb = GetComponent<Rigidbody>();
        _slashActivator = GetComponent<SlashActivator>();
        _buffActivator = GetComponent<BuffActivator>();

        _jumpKeyHeld = false;
        _isJump = false;

        GameUIManager.Instance.UpdateHealth(_currentHealth/TotalHealth);
        GameUIManager.Instance.UpdateMana(_currentMana/TotalMana);

        _moveSound = MovePlayer.GetFeedbackOfType<MMF_Sound>();
        _moveParticles = MovePlayer.GetFeedbackOfType<MMF_Particles>();
        _jumpLandParticles = MovePlayer.GetFeedbackOfType<MMF_Particles>();

        HealAbility.Initialize(this.gameObject);
        _isAttacking = false;
    }

    private void Update()
    {
        if (!_isAttacking) {
            Rotate();
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (!_isJump && !_isFalling && !_isDodging)
        {
            Physics.Raycast(transform.position, Vector3.down, out _hit, _raycastDistance);
            if (_hit.transform == null || _hit.transform.gameObject.layer != LayerMask.NameToLayer("Ground")) {
                _isFalling = true;
                _anim.SetBool("IsFalling", true);
                Falling();
                return;
            }
                        
            Attack();
            
            Dodge();
            
            Jump();
        }

        else if (_isJump && _jumpKeyHeld && !_isDodging) {
            Jumping();
            Dodge();
        }

        else if (_isJump && !_jumpKeyHeld && !_isDodging) {
            Dodge();
            Falling();
        } else if (_isFalling && !_isDodging) {
            Falling();
        }

        if (Input.GetKeyDown(KeyCode.H)) {
            TakeDamage(8);
        }

        if (Input.GetKeyDown(HealAbility.KeyToActivate) && !_buffActivator.GetActivated() && !_isAttacking) {
            _isBuffing = true;
            HealAbility.TriggerAbility();
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || _isAttacking) {
            _moveSound.Stop(transform.position);
            _moveSound.Active = false;
            MovePlayer.StopFeedbacks();
        }

        if (_isBuffing && Input.GetKeyUp(HealAbility.KeyToActivate)) {
            _isBuffing = false;
        }
    }

    Quaternion rot;
    bool isRun;

    void Rotate()
    {
        if (Input.GetKey(KeyCode.D))
        {           
            _isBuffing = false;
            _dir = 1;
            Move();            
            rot = Quaternion.LookRotation(Vector3.right);

            if (CameraLookAt) {
                CameraLookAt.transform.position = new Vector3(transform.position.x + CameraLookAtOffset, 0, 0);
            } else {
                Debug.LogWarning("Need to attach Virtual Camera");
            }

            if (VCam) {

                _composer.m_TrackedObjectOffset = new Vector3(CameraAimOffset, _composer.m_TrackedObjectOffset.y, _composer.m_TrackedObjectOffset.z);
            } else {
                Debug.Log("Need to attach Virtual Camera");
            }
        }

        
        else if (Input.GetKey(KeyCode.A))
        {
            _isBuffing = false;
            _dir = -1;            
            Move();
            rot = Quaternion.LookRotation(Vector3.left);

            if (CameraLookAt) {
                CameraLookAt.transform.position = new Vector3(transform.position.x - CameraLookAtOffset, 0, 0);
            } else {
                Debug.LogWarning("Need to attach Virtual Camera");
            }

            if (VCam) {
                _composer.m_TrackedObjectOffset = new Vector3(-CameraAimOffset, _composer.m_TrackedObjectOffset.y, _composer.m_TrackedObjectOffset.z);
            } else {
                Debug.Log("Need to attach Virtual Camera");
            }
        }

        else
        {            
            _anim.SetBool("Run", false);
                _anim.SetBool("Walk", false);
        }

        PlayerModel.transform.rotation = Quaternion.Slerp(PlayerModel.transform.rotation, rot, SpeedRot * Time.deltaTime);

    }

    
    void Move()
    {
        if (_isJump)
        {   
            _moveParticles.Stop(transform.position);
            _moveParticles.Active = false;
            _moveSound.Stop(transform.position);
            _moveSound.Active = false;

            transform.position += new Vector3(_dir, 0, 0) * JumpMove * Time.deltaTime;            
            _anim.SetBool("Run", false);
                _anim.SetBool("Walk", false);

        }
        else
        {   
            if (!MovePlayer.IsPlaying) {
                MovePlayer.PlayFeedbacks();
                _moveSound.Active = true;
                _moveSound.Play(transform.position);
            } 
            
            if (!_moveParticles.Active && !_isJump) {
                _moveParticles.Active = true;
                 _moveParticles.Play(transform.position); 
            } 
            
            if (!_moveSound.Active && !_isJump) {
                _moveSound.Active = true;
                _moveSound.Play(transform.position);
            }
            transform.position = transform.position + new Vector3(_dir * SpeedMove, 0, 0) * Time.deltaTime;
            _anim.SetBool("Run", true);
                _anim.SetBool("Walk", Input.GetKey(KeyCode.LeftControl));
        }
    }

    void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isBuffing = false;
            StartCoroutine(Dodging());
            _anim.SetTrigger("Dodge");
            _anim.SetBool("Healing", false);
        }
    }

    void Jump() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            _isBuffing = false;
            _jumpKeyHeld = true;
            _isJump = true;
            JumpPlayer.PlayFeedbacks();

            _rb.AddForce(new Vector3(0, CalculateJumpForce()), ForceMode.Impulse);
            _anim.SetTrigger("Jump");
            _anim.SetBool("IsGrounded", false);
            _anim.SetBool("Healing", false);
        }
    }

    private void Jumping() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            _jumpKeyHeld = false;
        } else if (_rb.velocity.y < 0) {
            _isFalling = true;
            Physics.Raycast(transform.position, Vector3.down, out _hit, _raycastDistance);
            if (_hit.transform != null && _hit.distance <= 0.5f && _rb.velocity.y <= 0 && _hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                JumpPlayer.StopFeedbacks();
                StartCoroutine(JumpLand());
                _isJump = false;
                _jumpKeyHeld = false;
                _isFalling = false;
                _anim.SetBool("IsGrounded", true);
                _anim.SetBool("IsFalling", false);
                CameraLookAt.transform.position = new Vector3(CameraLookAt.transform.position.x, transform.position.y, 0);
            }
        }
    }

    private void Falling() {
        if (!_isDodging) {
            Physics.Raycast(transform.position, Vector3.down, out _hit, _raycastDistance);
            if (_hit.transform != null && _hit.distance <= 0.5f && _rb.velocity.y <= 0 && _hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                JumpPlayer.StopFeedbacks();
                StartCoroutine(JumpLand());
                _isJump = false;
                _isFalling = false;
                _anim.SetBool("IsGrounded", true);
                _anim.SetBool("IsFalling", false);
                CameraLookAt.transform.position = new Vector3(CameraLookAt.transform.position.x, transform.position.y, 0);
            } else {
                _rb.AddForce(new Vector3(0, CounterJumpForce) * _rb.mass);
            }
        }
    }

    private float CalculateJumpForce() {
        return Mathf.Sqrt(2 * Physics.gravity.magnitude * MaxJumpHeight);
    }

    private float CalculateDodgeSpeed() {
        return DodgeDistance / DodgeTime;
    }

    public void TakeDamage(int damage) {
        _isBuffing = false;
        _currentHealth -= damage;
        GameUIManager.Instance.UpdateHealth((float) _currentHealth / (float) TotalHealth);
        if (_currentHealth <= 0) {
            _currentHealth = 0;
            Die();
        }

        HitPlayer.PlayFeedbacks();
        _anim.SetTrigger("Hit");
        _anim.SetBool("Healing", false);
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    private void Attack() {
        if (!_isAttacking && Input.GetKeyDown(AttackAbilities[0].KeyToActivate)) {
            _isBuffing = false;
            StartCoroutine(Attacking());
        }
    }

    IEnumerator JumpLand() {
        JumpLandPlayer.PlayFeedbacks();
        yield return new WaitForSeconds(0.5f);
        JumpLandPlayer.StopFeedbacks();
    }

    IEnumerator Dodging() {
        _isDodging = true;
        _currentDodgeTime = DodgeTime;
        float speed = CalculateDodgeSpeed();

        DodgePlayer.PlayFeedbacks();
        while(_currentDodgeTime > 0) {
            Vector3 newPos = transform.position;
            newPos.x += _dir * speed * Time.deltaTime;
            transform.position = newPos;

            _currentDodgeTime -= Time.deltaTime;
            yield return null;
        }
        DodgePlayer.StopFeedbacks();

        _isDodging = false;
    }

    IEnumerator Attacking() {
        _isAttacking = true;
        float currentComboTime = 0;
        int currentClicks = 0;
        int totalClicks = 0;
        SlashAbility currentAbility;
        
        AttackAbilities[currentClicks].Initialize(this.gameObject);
        yield return null;
        AttackAbilities[currentClicks].TriggerAbility();
        ManaChange(-AttackAbilities[currentClicks].ManaCost);
        GameUIManager.Instance.UpdateMana((float) _currentMana / (float) TotalMana);

        currentClicks += 1;
        totalClicks = currentClicks;

        while(currentComboTime < Term && _isAttacking && currentClicks < AttackAbilities.Length) {
            currentAbility = AttackAbilities[currentClicks];
            if (!_slashActivator.GetActivated() && totalClicks > currentClicks) {
                AttackAbilities[currentClicks].Initialize(this.gameObject);
                AttackAbilities[currentClicks].TriggerAbility();
                ManaChange(-AttackAbilities[currentClicks].ManaCost);
                GameUIManager.Instance.UpdateMana((float) _currentMana / (float) TotalMana);
                currentClicks += 1;
                currentComboTime = 0;
            } else if (!_slashActivator.GetActivated()) {
                currentComboTime += Time.deltaTime;
            }

            if (Input.GetKeyDown(currentAbility.KeyToActivate)) {
                totalClicks += 1;
            }

            yield return null;
        }
        yield return new WaitForSeconds(AttackAbilities[AttackAbilities.Length-1].DeathDuration);
        _isAttacking = false;

    }

}
