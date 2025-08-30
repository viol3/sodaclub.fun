using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.UI
{
    public class BarCursorUI : MonoBehaviour
    {
        [SerializeField] private bool _horizontal = true;
        [SerializeField] private float _min;
        [SerializeField] private float _max;

        private RectTransform _rectTransform;
        private Tweener _moveTween;
        private float _ratio = 0;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        public void SetRatio(float newRatio, float speed)
        {
            _ratio = newRatio;
            float newAxesValue = _min + (_max - _min) * newRatio;
            if (_moveTween != null)
            {
                _moveTween.Kill();
            }
            if (_horizontal)
            {
                _moveTween = _rectTransform.DOAnchorPosX(newAxesValue, speed).SetSpeedBased();
            }
            else
            {
                _moveTween = _rectTransform.DOAnchorPosY(newAxesValue, speed).SetSpeedBased();
            }
        }

        public void SetRatio(float newRatio)
        {
            _ratio = newRatio;
            float newAxesValue = _min + (_max - _min) * newRatio;
            if (_moveTween != null)
            {
                _moveTween.Kill();
            }
            if (_horizontal)
            {
                _rectTransform.anchoredPosition = new Vector2(newAxesValue, _rectTransform.anchoredPosition.y);
            }
            else
            {
                _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, newAxesValue);
            }
        }

        public float GetRatio()
        {
            return _ratio;
        }
    }
}