using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper
{
    public class MouseOnPlane : MonoBehaviour
    {
        [SerializeField] private Transform _planeTransform;
        [SerializeField] private bool _activated = true;
        [SerializeField] private bool _holdRequired = true;
        [SerializeField] private string _planeLayerName = "MousePlane";

        public event System.Action<Vector3> OnMouseDown;
        public event System.Action<Vector3> OnMouseUp;
        public event System.Action<Vector3> OnMouseUpdate;

        private bool _holding = false;
        void Update()
        {
            UpdateMouseEvents();
        }

        void UpdateMouseEvents()
        {
            if (!_activated)
            {
                return;
            }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 50f, LayerMask.GetMask(_planeLayerName)) && hit.transform == _planeTransform)
            {
                OnMouseDown?.Invoke(hit.point);
            }
            else if (Input.GetMouseButtonUp(0) && Physics.Raycast(ray, out hit, 50f, LayerMask.GetMask(_planeLayerName)) && hit.transform == _planeTransform)
            {
                OnMouseUp?.Invoke(hit.point);
            }

            if (_holdRequired && !Input.GetMouseButton(0))
            {
                return;
            }

            if (Physics.Raycast(ray, out hit, 50f, LayerMask.GetMask(_planeLayerName)) && hit.transform == _planeTransform)
            {
                OnMouseUpdate?.Invoke(hit.point);
            }
        }

        public Vector3 GetPlaneNormal()
        {
            return _planeTransform.up;
        }

        public void SetActive(bool value)
        {
            _activated = value;
        }
    }
}