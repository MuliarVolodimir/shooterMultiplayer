using System;
using Unity.Netcode;
using UnityEngine;

public class FPSController : NetworkBehaviour
{
    [SerializeField] CharacterController _cc;
    [SerializeField] GameObject _head;
    [SerializeField] Weapon _weapon;

    [SerializeField] float _speed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _sensitivity;
    [SerializeField] float _gravity;

    Vector3 _directionVector;

    [SerializeField] float _verticalVelocity;
    float _mouseX;
    float _mouseY;
    float _stepOffset;

    private void Start()
    {
        transform.position = new Vector3(0, 2, 0);
        if (!IsOwner)
        {
            this.GetComponent<FPSController>().enabled = false;

            this.GetComponentInChildren<Camera>().gameObject.SetActive(false);
            _weapon.enabled = false;
            return;
        }

        _cc = GetComponent<CharacterController>();
        _stepOffset = _cc.stepOffset;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        GravityAndJump();
        Rotation();
        Movement();
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
        _directionVector = new Vector3(Input.GetAxis("Horizontal") * _speed, _verticalVelocity, Input.GetAxis("Vertical") * _speed);
        _directionVector = transform.TransformDirection(_directionVector);

        _cc.Move(_directionVector * Time.deltaTime);
    }
}
