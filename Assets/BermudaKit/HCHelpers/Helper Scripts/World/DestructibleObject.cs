using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class DestructibleObject : MonoBehaviour
    {
        [SerializeField] private GameObject[] _hideObjects;
        [SerializeField] private GameObject[] _prizeObjects;
        [Space]
        [SerializeField] private float _explosionForce = 1f;
        [SerializeField] private float _explosionRadius = 2f;
        [SerializeField] private Transform _explosionPoint;

        private bool _destructed = false;

        public bool Destructed { get => _destructed; }

        public event System.Action OnDestructed;

        public void Destruct()
        {
            if(_destructed)
            {
                return;
            }
            Destroy(GetComponent<Rigidbody>());
            for (int i = 0; i < _prizeObjects.Length; i++)
            {
                _prizeObjects[i].SetActive(true);
            }
            for (int i = 0; i < _hideObjects.Length; i++)
            {
                _hideObjects[i].SetActive(false);
            }
            if (_explosionPoint != null)
            {
                Rigidbody[] _rbs = GetComponentsInChildren<Rigidbody>();
                for (int i = 0; i < _rbs.Length; i++)
                {
                    _rbs[i].AddExplosionForce(_explosionForce, _explosionPoint.position, _explosionRadius);
                }
                _destructed = true;
                OnDestructed?.Invoke();
            }
        }
    }
}