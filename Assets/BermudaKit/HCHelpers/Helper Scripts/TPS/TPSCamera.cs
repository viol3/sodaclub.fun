using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.TPS
{
    public class TPSCamera : LocalSingleton<TPSCamera>
    {
        [SerializeField] private bool _smooth;
        [SerializeField] private float _smoothSpeed;
        [SerializeField] private bool _followX;
        [SerializeField] private bool _followY;
        [SerializeField] private bool _followZ;
        [SerializeField] private Transform _target;

        private Vector3 _offset;
        private bool _active = true;

        private void Start()
        {
            UpdateOffset();
        }

        public void UpdateOffset()
        {
            if (_target)
            {
                _offset = transform.position - _target.position;
            }
        }

        void Update()
        {
            if(!_active)
            {
                return;
            }
            if (_target)
            {
                Vector3 followPos = transform.position;
                if(_followX)
                {
                    followPos.x = _target.position.x + _offset.x;
                }

                if (_followY)
                {
                    followPos.y = _target.position.y + _offset.y;
                }

                if (_followZ)
                {
                    followPos.z = _target.position.z + _offset.z;
                }

                if (_smooth)
                {
                    transform.position = Vector3.Lerp(transform.position, followPos, _smoothSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = followPos;
                }

            }
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetOffset(Vector3 offset)
        {
            _offset = offset;
        }

        public Transform GetTarget()
        {
            return _target;
        }

        public void SetActive(bool active)
        {
            _active = active;
        }
    }
}