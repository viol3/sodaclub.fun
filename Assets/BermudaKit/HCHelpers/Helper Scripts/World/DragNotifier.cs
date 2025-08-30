using Ali.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNotifier : MonoBehaviour
{
    [SerializeField] private float _thresholdMagnitude;
    private bool _holding = false;
    private Vector3 _lastPosition;

    public event System.Action OnDragStarted;
    public event System.Action OnDragFinished;
    public event System.Action<Vector3> OnDrag;

    private void Update()
    {
        if(_holding)
        {
            Vector3 currentPosition = GameUtility.MouseWorldPosition();
            Vector3 delta = currentPosition - _lastPosition;
            _lastPosition = currentPosition;
            if(delta.magnitude >= _thresholdMagnitude)
            {
                OnDrag?.Invoke(delta);
            }
        }
    }

    private void OnMouseDown()
    {
        if(_holding)
        {
            return;
        }
        _holding = true;
        _lastPosition = GameUtility.MouseWorldPosition();
        OnDragStarted?.Invoke();
    }

    private void OnMouseUp()
    {
        if (!_holding)
        {
            return;
        }
        _holding = false;
        OnDragFinished?.Invoke();
    }

    public void SetThresholdMagnitude(float value)
    {
        _thresholdMagnitude = value;
    }
}
