using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Ali.Helper.UI
{
    public class RandomBlinkFadeLightUI : MonoBehaviour
    {
        [SerializeField] private bool _instantChange;
        [SerializeField] private bool _randomized = true;
        [SerializeField] private float _fadeDuration = 0.3f;
        [SerializeField] private float _waitDuration = 0f;

        private MaskableGraphic _image;
        void Start()
        {
            _image = GetComponent<MaskableGraphic>();
            GameUtility.ChangeAlphaMaskableGraphic(_image, 0f);
            StartCoroutine(Animation());
        }

        IEnumerator Animation()
        {
            if(_randomized)
            {
                _fadeDuration = Random.Range(0.3f, 1f);
                _waitDuration = Random.Range(0.3f, 1f);
            }
            while (true)
            {
                yield return new WaitForSeconds(_waitDuration);
                if (_instantChange)
                {
                    GameUtility.ChangeAlphaMaskableGraphic(_image, 1f);
                    yield return new WaitForSeconds(_fadeDuration);
                    GameUtility.ChangeAlphaMaskableGraphic(_image, 0f);
                }
                else
                {
                    yield return _image.DOFade(1f, _fadeDuration).WaitForCompletion();
                    yield return _image.DOFade(0f, _fadeDuration).WaitForCompletion();
                }

            }
        }
    }
}