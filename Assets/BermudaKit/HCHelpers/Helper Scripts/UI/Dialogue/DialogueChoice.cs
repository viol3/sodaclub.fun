using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ali.Helper.UI.Dialogue
{
    public class DialogueChoice : MonoBehaviour
    {
        [SerializeField] private RectTransform _choicePanel;
        [SerializeField] private Text _choiceText;
        [SerializeField] private int _choiceIndex = 0;

        public event System.Action<int> OnClick;

        private bool _alreadyChoiced = false;

        public void OnChoiceClick()
        {
            if (_alreadyChoiced)
            {
                return;
            }
            _alreadyChoiced = true;
            Bounce();
            OnClick?.Invoke(_choiceIndex);
        }

        public void SetText(string text)
        {
            _choiceText.text = text;
        }

        public void Show(float duration)
        {
            _choicePanel.gameObject.SetActive(true);
            _choicePanel.localScale *= 0.5f;
            _choicePanel.DOScale(_choicePanel.localScale * 2f, duration).OnComplete(OnFirstScaleFinish);
        }

        public void Hide()
        {
            _choicePanel.gameObject.SetActive(false);
        }

        public void Reset()
        {
            _alreadyChoiced = false;
        }

        void Bounce()
        {
            StopAllCoroutines();
            _choicePanel.DOKill(true);
            _choicePanel.DOPunchScale(Vector3.one * 0.2f, 0.2f, 6);
        }

        void OnFirstScaleFinish()
        {
            StartCoroutine(ScaleLoopProcess());
        }

        IEnumerator ScaleLoopProcess()
        {
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(_choiceIndex * 1f);
            while(true)
            {
                _choicePanel.DOPunchScale(Vector3.one * 0.1f, 0.4f, 6);
                yield return new WaitForSeconds(2f);
                //yield return new WaitForSeconds(_choiceIndex * 1f);
            }
            
        }

        private void OnDestroy()
        {
            _choicePanel.DOKill();
        }

    }
}