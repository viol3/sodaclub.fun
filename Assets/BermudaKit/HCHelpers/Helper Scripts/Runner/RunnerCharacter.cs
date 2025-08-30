using Ali.Helper.TPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.Runner
{
    public class RunnerCharacter : CharacterBase
    {
        [SerializeField] private Transform _modelTransform;
        [SerializeField] private bool _rotateEnabled = true;

        private Vector3 _velocity;
        private Rigidbody _rigidbody;
        [SerializeField] private float _rotateSpeed = 100f;
        
        public Vector3 Velocity { get => _velocity; private set => _velocity = value; }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void ProcessVelocity(Vector3 velocity)
        {
            _rigidbody.velocity = velocity;
        }

        void Update()
        {
            UpdateRotation();
        }

        void UpdateRotation()
        {
            if(!_rotateEnabled)
            {
                return;
            }

            if(_rigidbody.velocity == Vector3.zero)
            {
                return;
            }
            Vector3 eulerAngles = GameUtility.GetLookAtEulerAngles(Vector3.zero, _rigidbody.velocity);
            eulerAngles.x = _modelTransform.eulerAngles.x;
            eulerAngles.z = _modelTransform.eulerAngles.z;
            _modelTransform.eulerAngles = new Vector3(eulerAngles.x, Mathf.LerpAngle(_modelTransform.eulerAngles.y, eulerAngles.y, _rotateSpeed * Time.deltaTime));
        }

        public void Stop()
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }
}