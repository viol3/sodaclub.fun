using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ali.Helper.World
{
    public class SimpleMoveLoop1D : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _delayDuration = 0f;
        [SerializeField] private bool _isLocalPos;

        public UnityEvent OnStepCompleted;

        private Vector3 _firstPosition;
        void Start()
        {
            if(_isLocalPos)
            {
                _firstPosition = transform.localPosition;
            }
            else
            {
                _firstPosition = transform.position;
            }
            
            StartCoroutine(LoopProcess());
        }
        IEnumerator LoopProcess()
        {
            yield return new WaitForSeconds(_delayDuration);
            while(true)
            {
                if(_isLocalPos)
                {
                    yield return transform.DOLocalMove(_targetPosition, _moveSpeed).SetSpeedBased().WaitForCompletion();
                    transform.localPosition = _firstPosition;
                }
                else
                {
                    yield return transform.DOMove(_targetPosition, _moveSpeed).SetSpeedBased().WaitForCompletion();
                    transform.position = _firstPosition;
                }
                OnStepCompleted?.Invoke();
            }
        }
    }
}