using Unity.Netcode;
using UnityEngine;

public class FPSController : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] CharacterController _cc;
    [SerializeField] GameObject _head;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] float _sensitivity;
    [SerializeField] float _gravity;

    [Header("Movement")]
    [SerializeField] float _nowSpeed;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;

    [SerializeField] float _stepOffset;
    [SerializeField] float _startScale;

    Vector3 _directionVector;
    float _verticalVelocity;
    float _mouseX;
    float _mouseY;

    private void Start()
    {
        transform.position = new Vector3(0, 2, 0);

        if (!IsOwner)
        {
            this.GetComponent<FPSController>().enabled = false;
            this.GetComponentInChildren<Camera>().gameObject.SetActive(false);
            return;
        }

        _cc = GetComponent<CharacterController>();
        _stepOffset = _cc.stepOffset;
        _startScale = transform.localScale.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        GravityAndJump();
        Rotation();
        Crouching();
        Movement();
    }

    private void Crouching()
    {
        Vector3 rayUp = new Vector3(0, 1, 0);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, _startScale / 2, transform.localScale.z);
        }
        else if (!Physics.Raycast(transform.position, rayUp, 1, _layerMask) &&
                 !Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, _startScale, transform.localScale.z);
        }

        Debug.DrawRay(transform.position, rayUp, Color.blue);
    }

    private void Rotation()
    {
        _mouseX += Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;
        _mouseY += Input.GetAxis("Mouse Y") * _sensitivity * Time.deltaTime;

        _mouseY = Mathf.Clamp(_mouseY, -80, 80);

        transform.rotation = Quaternion.Euler(0, _mouseX, 0);
        _head.transform.rotation = Quaternion.Euler(-_mouseY, _mouseX, 0);
    }

    private void GravityAndJump()
    {
        if (_cc.isGrounded)
        {
            _cc.stepOffset = _stepOffset;
            _verticalVelocity = -_gravity * 0.5f;

            if (Input.GetButtonDown("Jump"))
            {
                _verticalVelocity = _jumpForce;
            }
        }
        else
        {
            _verticalVelocity -= _gravity * Time.deltaTime;
            _cc.stepOffset = 0f;
        }
    }

    private void Movement()
    {
        _nowSpeed = Input.GetKey(KeyCode.LeftShift) ? _moveSpeed * 2 : _moveSpeed;

        _directionVector = new Vector3(Input.GetAxis("Horizontal") * _nowSpeed, _verticalVelocity,
            Input.GetAxis("Vertical") * _nowSpeed);
        _directionVector = transform.TransformDirection(_directionVector);

        _cc.Move(_directionVector * Time.deltaTime);
    }
}
