using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMoveAbility : PlayerAbility, IPunObservable
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
    private bool _wasGrounded = true;

    private Vector3 _receivedPosition = Vector3.zero;
    private Quaternion _receivedRotation = Quaternion.identity;

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

    // 데이터 동기화를 위한 데이터 전송 및 수신 기능
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
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * 20f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _receivedRotation, Time.deltaTime * 20f);
            return;
        }

        GetInput();
        //Jump();

        Movement();
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
            if(Input.GetKeyDown(KeyCode.Space))
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
            _yVelocity = -0.5f; // ← 착지 안정화
            _animator.SetTrigger("DoLanding");
        }

        _wasGrounded = isGroundedNow;
    }

    private void Jump()
    {
        /*if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }*/

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