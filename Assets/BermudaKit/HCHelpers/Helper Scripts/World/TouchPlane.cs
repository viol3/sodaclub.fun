using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class TouchPlane : MonoBehaviour
    {
        [SerializeField] private Transform _planeTransform;
        [SerializeField] private bool _activated = true;
        [SerializeField] private bool _holdRequired = true;
        [SerializeField] private string[] _layers;

        public event System.Action<Vector3> OnTouchDown;
        public event System.Action<Vector3> OnTouchUpdate;
        public event System.Action<Vector3> OnTouchUp;

        void Update()
        {
            if (_holdRequired && !Input.GetMouseButton(0))
            {
                return;
            }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, LayerMask.GetMask(_layers)) && hit.transform == _planeTransform)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnTouchDown?.Invoke(hit.point);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    OnTouchUp?.Invoke(hit.point);
                }
                OnTouchUpdate?.Invoke(hit.point);
            }
        }
    }
}