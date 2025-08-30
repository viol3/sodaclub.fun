using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.UI
{
    public class CursorLoopUI : MonoBehaviour
    {
        [SerializeField] private Vector2 _startPos;
        [SerializeField] private Vector2 _endPos;
        [SerializeField] private float _speed = 100;
        [SerializeField] private Ease _ease;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
        }
        private void OnEnable()
        {
            _rectTransform.DOKill();
            _rectTransform.anchoredPosition = _startPos;
            _rectTransform.DOAnchorPos(_endPos, _speed).SetSpeedBased().SetEase(_ease).SetLoops(-1, LoopType.Yoyo);
        }

        public float GetRatioX()
        {
            return (_rectTransform.anchoredPosition.x - _startPos.x) / (_endPos.x - _startPos.x);
        }
    }
}