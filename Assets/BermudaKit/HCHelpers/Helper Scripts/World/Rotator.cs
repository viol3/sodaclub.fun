using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotateVelocity;
        [SerializeField] private Space _rotateSpace;

        void Update()
        {
            transform.Rotate(_rotateVelocity * Time.deltaTime, _rotateSpace);
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rotateVelocity = velocity;
        }
    }
}