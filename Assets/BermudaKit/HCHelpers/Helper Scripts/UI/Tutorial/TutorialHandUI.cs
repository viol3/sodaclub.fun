using Ali.Helper;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ali.Helper.UI
{
    public class TutorialHandUI : LocalSingleton<TutorialHandUI>
    {
        [SerializeField] private RectTransform _canvasRect;
        [SerializeField] private Image _image;

        Coroutine _handCo;
        Coroutine _paintBucketCo;
        private bool _paintBucketTutorialActive = false;

        private Vector3 _firstScale;
        private void Start()
        {
            _image.enabled = false;
            _firstScale = _image.rectTransform.localScale;
        }

        public void StopTutorial()
        {
            KillHandCoroutine();
            _paintBucketTutorialActive = false;
        }

        public bool IsPaintBucketTutorialActive()
        {
            return _paintBucketTutorialActive;
        }

        void KillHandCoroutine()
        {
            if (_handCo != null)
            {
                StopCoroutine(_handCo);
            }
            if (_paintBucketCo != null)
            {
                StopCoroutine(_paintBucketCo);
            }
            _handCo = null;
            _image.rectTransform.DOKill();
            _image.DOKill();
            _image.enabled = false;
            _image.rectTransform.localScale = _firstScale;
        }

        public void ShowSwipe(Vector2 startAnchorPos, Vector2 deltaAnchorPos, float speed = 10f)
        {
            KillHandCoroutine();
            _handCo = StartCoroutine(ShowSwipeProcess(startAnchorPos, deltaAnchorPos, speed));
        }

        IEnumerator ShowSwipeProcess(Vector2 startAnchorPos, Vector2 deltaAnchorPos, float speed)
        {
            while (true)
            {
                _image.rectTransform.anchoredPosition = startAnchorPos;
                _image.enabled = true;
                yield return new WaitForSeconds(0.2f);
                yield return _image.rectTransform.DOScale(_image.rectTransform.localScale * 0.8f, 0.3f).WaitForCompletion();
                yield return _image.rectTransform.DOAnchorPos(deltaAnchorPos, speed).SetSpeedBased().SetRelative().WaitForCompletion();
                yield return _image.rectTransform.DOScale(_image.rectTransform.localScale / 0.8f, 0.3f).WaitForCompletion();
                _image.enabled = false;
                yield return new WaitForSeconds(1f);
            }
        }

        public void ShowDragDrop(Vector3 worldStart, Vector3 worldEnd, float speed = 10f)
        {
            KillHandCoroutine();
            _handCo = StartCoroutine(ShowDragDropProcess(worldStart, worldEnd, speed));
        }


        IEnumerator ShowDragDropProcess(Vector3 worldStart, Vector3 worldEnd, float speed)
        {
            Vector2 startPosUI = GameUtility.GetCanvasPositionFromWorldPosition(worldStart, _canvasRect);
            Vector2 endPosUI = GameUtility.GetCanvasPositionFromWorldPosition(worldEnd, _canvasRect);
            while (true)
            {
                _image.rectTransform.anchoredPosition = startPosUI;
                _image.enabled = true;
                yield return new WaitForSeconds(0.2f);
                yield return _image.rectTransform.DOScale(_image.rectTransform.localScale * 0.8f, 0.3f).WaitForCompletion();
                yield return _image.rectTransform.DOAnchorPos(endPosUI, speed).SetSpeedBased().WaitForCompletion();
                yield return _image.rectTransform.DOScale(_image.rectTransform.localScale / 0.8f, 0.3f).WaitForCompletion();
                _image.enabled = false;
                yield return new WaitForSeconds(1f);
            }
        }

        public void ShowHorizontalSwipe(Vector3 worldPos, float speed = 10f)
        {
            KillHandCoroutine();
            _handCo = StartCoroutine(ShowHorizontalSwipeProcess(worldPos, speed));
        }

        IEnumerator ShowHorizontalSwipeProcess(Vector3 worldPos, float speed)
        {
            Vector2 startPosUI = GameUtility.GetCanvasPositionFromWorldPosition(worldPos, _canvasRect);
            while (true)
            {
                _image.rectTransform.anchoredPosition = startPosUI;
                _image.enabled = true;
                yield return new WaitForSeconds(0.2f);
                yield return _image.rectTransform.DOAnchorPosX(-100f, speed).SetRelative().SetSpeedBased().WaitForCompletion();
                for (int i = 0; i < 3; i++)
                {
                    yield return _image.rectTransform.DOAnchorPosX(200f, speed).SetRelative().SetSpeedBased().WaitForCompletion();
                    yield return _image.rectTransform.DOAnchorPosX(-200f, speed).SetRelative().SetSpeedBased().WaitForCompletion();
                }
                yield return new WaitForSeconds(0.2f);
                _image.enabled = false;
                yield return new WaitForSeconds(1f);
            }
        }

        public void ShowSineDraw(Vector3 worldPos, float speed = 10f)
        {
            KillHandCoroutine();
            _handCo = StartCoroutine(ShowSineDrawProcess(worldPos, speed));
        }

        IEnumerator ShowSineDrawProcess(Vector3 worldPos, float speed)
        {
            Vector2 startPosUI = GameUtility.GetCanvasPositionFromWorldPosition(worldPos, _canvasRect);
            while (true)
            {
                _image.rectTransform.anchoredPosition = startPosUI;
                _image.enabled = true;
                yield return new WaitForSeconds(0.1f);
                //Vector2[] path = new Vector2[3];
                //path[0] = _image.rectTransform.anchoredPosition + new Vector2(80f, 100f);
                _image.rectTransform.DOAnchorPosY(150f, speed).SetSpeedBased().SetRelative().SetEase(Ease.OutSine);
                yield return _image.rectTransform.DOAnchorPosX(120f, speed).SetRelative().SetSpeedBased().WaitForCompletion();
                _image.rectTransform.DOAnchorPosY(-150f, speed).SetRelative().SetSpeedBased().SetEase(Ease.InSine);
                yield return _image.rectTransform.DOAnchorPosX(120f, speed).SetRelative().SetSpeedBased().WaitForCompletion();
                yield return new WaitForSeconds(0.1f);
                _image.enabled = false;
                yield return new WaitForSeconds(1f);
            }
        }

        public void ShowClick(Vector3 worldPos, float speed = 10f)
        {
            KillHandCoroutine();
            _handCo = StartCoroutine(ShowClickProcess(worldPos, speed));
        }

        IEnumerator ShowClickProcess(Vector3 worldPos, float speed)
        {
            Vector2 startPosUI = GameUtility.GetCanvasPositionFromWorldPosition(worldPos, _canvasRect);
            while (true)
            {
                _image.rectTransform.anchoredPosition = startPosUI;
                _image.enabled = true;
                yield return new WaitForSeconds(0.1f);
                yield return _image.rectTransform.DOScale(_image.rectTransform.localScale * 0.8f, speed).SetSpeedBased().WaitForCompletion();
                yield return _image.rectTransform.DOScale(_image.rectTransform.localScale / 0.8f, speed).SetSpeedBased().WaitForCompletion();
                _image.enabled = false;
                yield return new WaitForSeconds(1f);
            }
        }
    }
}