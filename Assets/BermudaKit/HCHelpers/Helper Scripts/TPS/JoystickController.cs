using Ali.Helper;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : LocalSingleton<JoystickController>
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Rigidbody _rigidbody;
    [Space]
    [SerializeField] private float _inputMultiplier = 1f;
    [SerializeField] private float _moveLerpSpeed = 1f;

    private bool _holding = false;
    private Vector3 _targetVelocity = Vector3.zero;
    private bool _enabled = true;

    //private void Start()
    //{
    //    if (PlayerPrefs.HasKey("speed"))
    //    {
    //        _inputMultiplier = PlayerPrefs.GetFloat("speed");
    //    }
    //}

    void Update()
    {
        Move();
    }

    private void Move()
    {
        bool _wasd = false;
        Vector2 inputDirection = Vector2.zero;
        inputDirection = _joystick.Direction;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f)
        {
            inputDirection.x = Input.GetAxisRaw("Horizontal");
            _wasd = true;
        }
        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f)
        {
            inputDirection.y = Input.GetAxisRaw("Vertical");
            _wasd = true;
        }
        if (!_wasd && Mathf.Abs(inputDirection.x) < 0.1f && Mathf.Abs(inputDirection.y) < 0.1f)
        {
            _targetVelocity = Vector3.zero;
            _rigidbody.linearVelocity = _targetVelocity;
            return;
        }
        Vector3 direction = new Vector3(inputDirection.x, 0f, inputDirection.y);
        Vector3 cameraRot = Camera.main.transform.rotation.eulerAngles;
        cameraRot.x = 0f;
        cameraRot.z = 0f;
        _targetVelocity = Quaternion.Euler(cameraRot) * direction.normalized;
        _targetVelocity = _targetVelocity.normalized * _inputMultiplier;
        _targetVelocity = new Vector3(_targetVelocity.x, 0f, _targetVelocity.z);



        if (!_enabled)
        {
            return;
        }

        _rigidbody.linearVelocity = Vector3.Lerp(_rigidbody.linearVelocity, _targetVelocity, Time.deltaTime * _moveLerpSpeed);
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;
        _rigidbody.linearVelocity = Vector3.zero;
    }

    public Joystick GetJoystick()
    {
        return _joystick;
    }

    public bool IsEnabled()
    {
        return _enabled;
    }

    public Vector3 GetVelocity()
    {
        return _rigidbody.linearVelocity;
    }

    public void SetSpeed(float value)
    {
        _inputMultiplier += value;
        PlayerPrefs.SetFloat("speed", _inputMultiplier);
    }
    public float GetMoveSpeed()
    {
        return _inputMultiplier;
    }
}