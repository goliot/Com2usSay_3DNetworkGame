using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private readonly float GRAVITY = -20f;
    private float _yVelocity = 0f;

    [Header("# Stats")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpPower = 10f;

    [Header("# Component")]
    private CharacterController _characterController;
    private Animator _animator;

    private float _v;
    private float _h;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
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
        //horizontal = Camera.main.transform.TransformDirection(horizontal);
        horizontal.y = 0f;
        horizontal.Normalize();

        Vector3 move = horizontal * _speed;

        if (_characterController.isGrounded)
        {
            _yVelocity = -0.5f;

            //// Space 키는 여기서도 처리 가능 (더 안정적으로 동작)
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    _yVelocity = _jumpPower;
            //}
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
            _yVelocity = _jumpPower;
        }
    }

    private void SetAnimation()
    {
        _animator.SetFloat("Horizontal", _h);
        _animator.SetFloat("Vertical", _v);
        _animator.SetBool("IsMoving", !(Mathf.Approximately(0, _h) && Mathf.Approximately(0, _v)));
    }
}
