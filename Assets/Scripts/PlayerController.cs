using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
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

    [Header("Movement speed during jump")]
    public float JumpMove;

    [Header("Dodge speed")]
    public float DodgeSpeed;

    [Header("How long player moves by dodge speed")]
    public float DodgeTime;

    [Header("Camera Aim Offset")]
    public float CameraAimOffset;

    [Header("Camera look at position offset")]
    public float CameraLookAtOffset;

    [Header("Time available for combo")]
    public int term;

    public bool _isJump;

    private Animator _anim;

    private float _dir = 1;

    private CinemachineComposer _transposer;
    
    private Rigidbody _rb;

    private bool _dodging;

    private float _currentDodgeTime;

    private void Start() {
        if (PlayerModel) {
            _anim = PlayerModel.GetComponent<Animator>();
        } else {
            Debug.LogWarning("No Player Model attached");
        }

        if (VCam) {
            _transposer = VCam.GetCinemachineComponent<CinemachineComposer>();
        } else {
            Debug.LogWarning("Need to attach Virtual Camera");
        }

        _rb = GetComponent<Rigidbody>();

        _dodging = false;
    }

    private void Update()
    {
        Rotate();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (!_isJump)
        {            
            // Attack();
            
            Dodge();
            
            // Jump();
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

                _transposer.m_TrackedObjectOffset = new Vector3(CameraAimOffset, _transposer.m_TrackedObjectOffset.y, _transposer.m_TrackedObjectOffset.z);
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
                _transposer.m_TrackedObjectOffset = new Vector3(-CameraAimOffset, _transposer.m_TrackedObjectOffset.y, _transposer.m_TrackedObjectOffset.z);
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
            transform.position += transform.forward * JumpMove * Time.deltaTime;            
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
            _currentDodgeTime = DodgeTime;
            _dodging = true;
            _rb.AddForce(new Vector3(_dir * DodgeSpeed, 0, 0), ForceMode.Impulse);
            _anim.SetTrigger("Dodge");
            StartCoroutine(DodgeCooldown());
        }
    }

    IEnumerator DodgeCooldown() {
        while(_currentDodgeTime <= 0) {
            yield return null;

            _currentDodgeTime -= Time.deltaTime;
        }

        Debug.Log("Hit it ");
        _rb.velocity = new Vector3(0,0,0);
    }
}
