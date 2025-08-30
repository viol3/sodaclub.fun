using Ali.Helper;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ali.Helper.UI
{
    public class InstagramPost : LocalSingleton<InstagramPost>
    {
        [SerializeField] private Image _sipsakImage;
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private Image _fader;
        [SerializeField] private RectTransform _postPanel;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _tagText;
        [SerializeField] private Text _likeText;
        [SerializeField] private GameObject[] _otherGOs;


        public void Show()
        {
            _fader.gameObject.SetActive(true);
            _postPanel.gameObject.SetActive(true);
            for (int i = 0; i < _otherGOs.Length; i++)
            {
                _otherGOs[i].SetActive(true);
            }
            _postPanel.DOPunchScale(Vector3.one * 0.1f, 0.4f);
        }

        public void Hide()
        {
            _fader.gameObject.SetActive(false);
            for (int i = 0; i < _otherGOs.Length; i++)
            {
                _otherGOs[i].SetActive(false);
            }
            _postPanel.DOKill(true);
            _postPanel.gameObject.SetActive(false);
        }

        public void UpdateInstagramImage()
        {
            _rawImage.texture = GameUtility.GetCameraTexture(Camera.main);
            float ratio = (float)_rawImage.texture.width / _rawImage.texture.height;
            _rawImage.rectTransform.sizeDelta = new Vector2(_rawImage.rectTransform.sizeDelta.y * ratio, _rawImage.rectTransform.sizeDelta.y);
        }

        public void UpdateInformation(string name, string tag, string like)
        {
            _nameText.text = name;
            _tagText.text = tag;
            _likeText.text = like;
        }

        public bool IsHidden()
        {
            return !_postPanel.gameObject.activeInHierarchy;
        }

        public void SetPictureAnchorPos(Vector2 pos)
        {
            _rawImage.rectTransform.anchoredPosition = pos;
        }

        public void Sipsak()
        {
            StartCoroutine(SipsakProcess());
        }

        IEnumerator SipsakProcess()
        {
            yield return _sipsakImage.DOFade(1f, 0.1f).WaitForCompletion();
            yield return _sipsakImage.DOFade(0f, 0.5f).WaitForCompletion();
        }
    }
}