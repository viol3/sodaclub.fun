using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class PositionClamper : MonoBehaviour
    {
        [SerializeField] private bool _active = false;
        [SerializeField] private bool _isLocal = false;
        [SerializeField] private Vector3 _minPosition;
        [SerializeField] private Vector3 _maxPosition;

        public bool Active { get => _active; set => _active = value; }

        void Update()
        {
            if (_active)
            {
                if(_isLocal)
                {
                    transform.localPosition = new Vector3(
                    Mathf.Clamp(transform.localPosition.x, _minPosition.x, _maxPosition.x),
                    Mathf.Clamp(transform.localPosition.y, _minPosition.y, _maxPosition.y),
                    Mathf.Clamp(transform.localPosition.z, _minPosition.z, _maxPosition.z));
                }
                else
                {
                    transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, _minPosition.x, _maxPosition.x),
                    Mathf.Clamp(transform.position.y, _minPosition.y, _maxPosition.y),
                    Mathf.Clamp(transform.position.z, _minPosition.z, _maxPosition.z));
                }
                
            }
        }
    }
}