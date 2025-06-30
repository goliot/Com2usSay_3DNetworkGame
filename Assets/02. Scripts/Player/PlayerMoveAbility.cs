using Photon.Pun;
using Photon.Realtime;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMoveAbility : PlayerAbility//, IPunObservable
{
    private readonly float GRAVITY = -20f;
    private float _yVelocity = 0f;
    [Header("# CameraFollow")]
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private GameObject _minimapMarker;
    [SerializeField] private GameObject _minimapMarkerEnemy;

    [Header("# Component")]
    private CharacterController _characterController;
    private Animator _animator;

    private float _v;
    private float _h;
    public bool IsSprinting { get; private set; }
    public bool IsJumping { get; private set; }
    private bool _wasGrounded = true;
    private float _currentSpeed;

    /*private Vector3 _receivedPosition = Vector3.zero;
    private Quaternion _receivedRotation = Quaternion.identity;*/

    protected override void Awake()
    {
        base.Awake();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (_photonView.IsMine)
        {
            CinemachineCamera camera = GameObject.FindGameObjectWithTag("FollowCamera").GetComponent<CinemachineCamera>();
            camera.Follow = _cameraRoot;

            MinimapCamera minimapCamera = FindAnyObjectByType<MinimapCamera>();
            minimapCamera.SetTarget(transform);
            _minimapMarker.SetActive(true);
            _minimapMarkerEnemy.SetActive(false);
        }
        else
        {
            _minimapMarker.SetActive(false);
            _minimapMarkerEnemy.SetActive(true);
        }
    }

    /*// 데이터 동기화를 위한 데이터 전송 및 수신 기능
    // stream : 서버에서 주고 받을 데이터가 담긴 변수
    // info : 송수신 성공/실패 여부 로그
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && _photonView.IsMine)
        {
            // 데이터를 전송하는 상황 -> 데이터를 보내주면 되고
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading && !_photonView.IsMine)
        {
            // 데이터를 수신하는 상황 -> 받은 데이터를 세팅
            _receivedPosition = (Vector3)stream.ReceiveNext();
            _receivedRotation = (Quaternion)stream.ReceiveNext();
        }
    }*/

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            /*transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * 20f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _receivedRotation, Time.deltaTime * 20f);*/
            return;
        }

        GetInput();
        Movement();
        SetAnimation();
    }

    private void GetInput()
    {
        _v = Input.GetAxisRaw("Vertical");
        _h = Input.GetAxisRaw("Horizontal");
        SetSpeed(Input.GetKey(KeyCode.LeftShift));
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

        Vector3 move = horizontal * _currentSpeed;

        if (_characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }

        move.y = _yVelocity;
        _characterController.Move(move * Time.deltaTime);

        bool isGroundedNow = _characterController.isGrounded;

        if (isGroundedNow && !_wasGrounded)
        {
            _yVelocity = -0.5f;
            //_animator.SetTrigger("DoLanding");
            _photonView.RPC(nameof(PlayLandingAnimation), RpcTarget.All);
            IsJumping = false;
        }

        _wasGrounded = isGroundedNow;
    }

    private void SetSpeed(bool isPressingSprint)
    {
        if (!isPressingSprint)
        {
            IsSprinting = false;
            _currentSpeed = _player.GetStat(EStatType.MoveSpeed);
            return;
        }

        if (_player.TryUseStamina(_player.GetStat(EStatType.SprintStaminaCost) * Time.deltaTime))
        {
            IsSprinting = true;
            _currentSpeed = _player.GetStat(EStatType.SprintSpeed);
        }
        else
        {
            IsSprinting = false;
            _currentSpeed = _player.GetStat(EStatType.MoveSpeed);
        }
    }

    private void Jump()
    {
        if(_player.TryUseStamina(_player.GetStat(EStatType.JumpStaminaCost)))
        {
            IsJumping = true;
            //_animator.SetTrigger("DoJump");
            _photonView.RPC(nameof(PlayJumpAnimation), RpcTarget.All);
            _yVelocity = _player.GetStat(EStatType.JumpPower);
        }
    }

    private void SetAnimation()
    {
        _animator.SetFloat("Horizontal", _h);
        _animator.SetFloat("Vertical", _v);
        _animator.SetBool("IsSprinting", IsSprinting);
        _animator.SetBool("IsMoving", !(Mathf.Approximately(0, _h) && Mathf.Approximately(0, _v)));
    }

    [PunRPC]
    private void PlayJumpAnimation()
    {
        _animator.SetTrigger("DoJump");
    }

    [PunRPC]
    private void PlayLandingAnimation()
    {
        _animator.SetTrigger("DoLanding");
    }
}