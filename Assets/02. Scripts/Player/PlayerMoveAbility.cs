using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMoveAbility : PlayerAbility
{
    private readonly float GRAVITY = -20f;
    private float _yVelocity = 0f;
    [Header("# CameraFollow")]
    [SerializeField] private Transform _cameraRoot;

    [Header("# Component")]
    private CharacterController _characterController;
    private Animator _animator;

    private float _v;
    private float _h;

    protected override void Awake()
    {
        base.Awake();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if(_photonView.IsMine)
        {
            CinemachineCamera camera = GameObject.FindGameObjectWithTag("FollowCamera").GetComponent<CinemachineCamera>();
            camera.Follow = _cameraRoot;
        }
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        GetInput();
        Movement();
        Jump();
        SetAnimation();
    }

    private void GetInput()
    {
        _v = Input.GetAxisRaw("Vertical");
        _h = Input.GetAxisRaw("Horizontal");
    }

    private void Movement()
    {
        Vector3 horizontal = new Vector3(_h, 0, _v);
        horizontal = Camera.main.transform.TransformDirection(horizontal);
        horizontal.y = 0f;
        horizontal.Normalize();

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 15f);
        }

        Vector3 move = horizontal * _player.GetStat(EStatType.MoveSpeed);

        if (_characterController.isGrounded)
        {
            _yVelocity = -0.5f;
            _animator.SetTrigger("DoLanding");
        }
        else
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }

        move.y = _yVelocity;
        _characterController.Move(move * Time.deltaTime);
    }

    private void Jump()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }

        if (_characterController.isGrounded)
        {
            _animator.SetTrigger("DoJump");
            _yVelocity = _player.GetStat(EStatType.JumpPower);
        }
    }

    private void SetAnimation()
    {
        _animator.SetFloat("Horizontal", _h);
        _animator.SetFloat("Vertical", _v);
        _animator.SetBool("IsMoving", !(Mathf.Approximately(0, _h) && Mathf.Approximately(0, _v)));
    }
}
