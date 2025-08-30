using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ali.Helper.UI
{
    public class CurrencyParticle : MonoBehaviour
    {
        private Coroutine _travelCo;
        private RectTransform _rectTransform;
        private Image _image;
        private Vector2 _targetPosition;

        public event System.Action OnArrived;
        public void Init()
        {
            _rectTransform = (RectTransform)transform;
            _image = GetComponent<Image>();
            _image.enabled = false;
            _rectTransform.localEulerAngles = Vector3.zero;
            _rectTransform.anchoredPosition3D = new Vector3(_rectTransform.anchoredPosition3D.x, _rectTransform.anchoredPosition3D.y, 0f);
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
            _image.SetNativeSize();
        }

        public void SetTargetPosition(Vector2 targetPosition)
        {
            _targetPosition = targetPosition;
        }

        public void Travel(Vector2 firstPosition)
        {
            if (_travelCo != null)
            {
                StopCoroutine(_travelCo);
            }
            _rectTransform.DOKill(true);
            _travelCo = StartCoroutine(TravelProcess(firstPosition));
        }

        IEnumerator TravelProcess(Vector2 firstPosition)
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.1f));
            _image.enabled = true;
            _rectTransform.anchoredPosition = firstPosition;
            yield return _rectTransform.DOAnchorPos(firstPosition + Random.insideUnitCircle * 100f, Random.Range(0.35f, 0.6f)).WaitForCompletion();
            yield return _rectTransform.DOAnchorPos(_targetPosition, Random.Range(0.35f, 0.6f)).SetEase(Ease.InSine).WaitForCompletion();
            _image.enabled = false;
            OnArrived?.Invoke();
        }
    }
}