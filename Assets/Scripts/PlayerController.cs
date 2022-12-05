using System.Collections;
using System.Collections.Generic;
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

    [Header("Rotation speed")]
    public float SpeedRot;

    [Header("Movement speed")]
    public float SpeedMove;

    [Header("Max Jump height")]
    public float MaxJumpHeight;

    [Header("Movement speed during jump")]
    public float JumpMove;

    [Header("Drag on Player Jump")]
    public float CounterJumpForce;

    [Header("How far dodge takes player forward")]
    public float DodgeDistance;

    [Header("How long dodge state is active")]
    public float DodgeTime;

    [Header("Camera Aim Offset")]
    public float CameraAimOffset;

    [Header("Camera look at position offset")]
    public float CameraLookAtOffset;

    [Header("Time available for combo")]
    public int term;

    public bool _isJump;

    private bool _jumpKeyHeld;

    private bool _isFalling;

    private bool _isDodging;

    private Animator _anim;

    private float _dir = 1;

    private CinemachineComposer _composer;
    
    private Rigidbody _rb;

    float _raycastDistance = 1;
    RaycastHit _hit;
    float _currentDodgeTime;

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

        _jumpKeyHeld = false;
        _isJump = false;
    }

    private void Update()
    {
        Rotate();
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
                        
            // Attack();
            
            Dodge();
            
            Jump();
        }

        else if (_isJump && _jumpKeyHeld && !_isDodging) {
            Jumping();
        }

        else if (_isJump && !_jumpKeyHeld && !_isDodging) {
            Falling();
        } else if (_isFalling && !_isDodging) {
            Falling();
        }
    }

    Quaternion rot;
    bool isRun;

    void Rotate()
    {
        if (Input.GetKey(KeyCode.D))
        {           
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
            transform.position += new Vector3(_dir, 0, 0) * JumpMove * Time.deltaTime;            
            _anim.SetBool("Run", false);
                _anim.SetBool("Walk", false);

        }
        else
        {   
            transform.position = transform.position + new Vector3(_dir * SpeedMove, 0, 0) * Time.deltaTime;
            _anim.SetBool("Run", true);
                _anim.SetBool("Walk", Input.GetKey(KeyCode.LeftControl));
        }
    }

    void Dodge()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dodging());
            _anim.SetTrigger("Dodge");
        }
    }

    void Jump() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            _jumpKeyHeld = true;
            _isJump = true;
            _rb.AddForce(new Vector3(0, CalculateJumpForce()), ForceMode.Impulse);
            _anim.SetTrigger("Jump");
            _anim.SetBool("IsGrounded", false);
        }
    }

    private void Jumping() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            _jumpKeyHeld = false;
        } else if (_rb.velocity.y < 0) {
            _isFalling = true;
            Physics.Raycast(transform.position, Vector3.down, out _hit, _raycastDistance);
            if (_hit.transform != null && _hit.distance <= 0.5f && _rb.velocity.y <= 0 && _hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
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
        Physics.Raycast(transform.position, Vector3.down, out _hit, _raycastDistance);
        if (_hit.transform != null && _hit.distance <= 0.5f && _rb.velocity.y <= 0 && _hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            _isJump = false;
            _isFalling = false;
            _anim.SetBool("IsGrounded", true);
            _anim.SetBool("IsFalling", false);
            CameraLookAt.transform.position = new Vector3(CameraLookAt.transform.position.x, transform.position.y, 0);
        } else {
            _rb.AddForce(new Vector3(0, CounterJumpForce) * _rb.mass);
        }
    }

    private float CalculateJumpForce() {
        return Mathf.Sqrt(2 * Physics.gravity.magnitude * MaxJumpHeight);
    }

    private float CalculateDodgeSpeed() {
        return DodgeDistance / DodgeTime;
    }

    public void TakeDamage(int damage) {
        _currentHealth -= damage;
        if (_currentHealth <= 0) {
            _currentHealth = 0;
            Die();
        }
    }
    public void Heal(int heal) {
        _currentHealth += heal;
        if (_currentHealth > TotalHealth) {
            _currentHealth = TotalHealth;
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public override void AttackAnticipation() {
        throw new System.Exception("Attack anticipation not implemented");
    }
    public override void Attack() {
        throw new System.Exception("Attack not implemented");
    }
    public override void AttackRecovery() {
        throw new System.Exception("Attack recovery not implemented");
    }

    IEnumerator Dodging() {
        _isDodging = true;
        _currentDodgeTime = DodgeTime;
        float speed = CalculateDodgeSpeed();

        while(_currentDodgeTime > 0) {
            Vector3 newPos = transform.position;
            newPos.x += _dir * speed * Time.deltaTime;
            transform.position = newPos;

            _currentDodgeTime -= Time.deltaTime;
            yield return null;
        }

        _isDodging = false;
    }
}
