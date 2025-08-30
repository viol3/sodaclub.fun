using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class ScaleAnimationLoop : MonoBehaviour
    {
        [SerializeField] private float _startDelay = 0f;
        [SerializeField] private float _preDelay = 0f;
        [SerializeField] private float _scaleMultiplier = 1.1f;
        [SerializeField] private float _duration = 0.7f;

        private Coroutine _animationCo;
        private Vector3 _firstScale;
  
        void Start()
        {
            _firstScale = transform.localScale;
        }

        private void OnEnable()
        {
            StartAnimation();
        }

        private void OnDisable()
        {
            StopAnimation();
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        public void StartAnimation()
        {
            _animationCo = StartCoroutine(AnimationProcess());
        }

        public void StopAnimation()
        {
            if(_animationCo != null)
            {
                StopCoroutine(_animationCo);
                _animationCo = null;
            }
            transform.DOKill(true);
            transform.localScale = _firstScale;
        }

        IEnumerator AnimationProcess()
        {
            yield return new WaitForSeconds(_startDelay);
            while (true)
            {
                yield return new WaitForSeconds(_preDelay);
                yield return transform.DOScale(transform.localScale * _scaleMultiplier, _duration).WaitForCompletion();
                yield return transform.DOScale(transform.localScale / _scaleMultiplier, _duration).WaitForCompletion();
            }
            
        }
    }
}