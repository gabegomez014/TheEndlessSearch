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

    [Header("Camera Aim Offset")]
    public float CameraAimOffset;

    [Header("Camera look at position offset")]
    public float CameraLookAtOffset;

    [Header("Time available for combo")]
    public int term;

    public bool isJump;

    private Animator _anim;

    private float _dir;

    private CinemachineComposer _transposer;

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
    }

    private void Update()
    {
        Rotate();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (!isJump)
        {            
            // Attack();
            
            // Dodge();
            
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
        if (isJump)
        {            
            transform.position += transform.forward * JumpMove * Time.deltaTime;            
            _anim.SetBool("Run", false);
                _anim.SetBool("Walk", false);

        }
        else
        {   
            transform.position = transform.position + new Vector3(_dir * SpeedMove, 0, 0) * Time.deltaTime;
            // transform.position = new Vector3(transform.position.x  )
            _anim.SetBool("Run", true);
                _anim.SetBool("Walk", Input.GetKey(KeyCode.LeftControl));
        }
    }
}
