using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper
{
    public class LineOnPlane : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private MouseOnPlane _mouseOnPlane;
        [SerializeField] private float _distanceThreshold = 0.1f;
        [SerializeField] private int _vertexLimit = 50;
        [SerializeField] private Vector3 _offsetPos;

        private Vector3 _lastPos;
        private bool _activated = false;

        public event System.Action OnLimitReached;
        void Start()
        {
            _mouseOnPlane.OnMouseUpdate += MouseOnPlane_OnMouseUpdate;
        }

        private void MouseOnPlane_OnMouseUpdate(Vector3 pos)
        {
            if (_lastPos.Equals(Vector3.zero))
            {
                _lastPos = pos;
            }
            if(_lineRenderer.positionCount >= _vertexLimit || !_activated)
            {
                return;
            }
            if (Vector3.Distance(_lastPos, pos) > _distanceThreshold)
            {
                _lineRenderer.positionCount++;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, pos + _offsetPos);
                _lastPos = pos;

                if(_lineRenderer.positionCount >= _vertexLimit)
                {
                    OnLimitReached?.Invoke();
                }
            }
        }

        public void SetActive(bool active)
        {
            _activated = active;
        }

        public Vector3[] GetPositions()
        {
            Vector3[] result = new Vector3[_lineRenderer.positionCount];
            _lineRenderer.GetPositions(result);
            return result;
        }

        public void Clear()
        {
            _lineRenderer.positionCount = 0;
        }



    }
}