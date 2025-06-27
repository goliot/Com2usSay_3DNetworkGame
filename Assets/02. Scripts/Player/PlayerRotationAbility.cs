using UnityEngine;

public class PlayerRotationAbility : PlayerAbility
{
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private float _rotationSpeed = 10f;

    private float _mx;
    private float _my;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _mx += mouseX * _rotationSpeed * Time.deltaTime;
        _my += mouseY * _rotationSpeed * Time.deltaTime;

        _my = Mathf.Clamp(_my, -90f, 90f);

        transform.eulerAngles = new Vector3(0, _mx, 0);

        _cameraRoot.localEulerAngles = new Vector3(-_my, 0f, 0f);
    }
}
