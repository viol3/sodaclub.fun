using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Ali.Helper.World
{
    public class RandomBlinkFadeLight : MonoBehaviour
    {
        [SerializeField] private bool _instantChange;

        private SpriteRenderer _spriteRenderer;
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            GameUtility.ChangeAlphaSprite(_spriteRenderer, 0f);
            StartCoroutine(Animation());
        }

        IEnumerator Animation()
        {
            float fadeDuration = Random.Range(0.3f, 1f);
            float waitDuration = Random.Range(0.3f, 1f);
            while (true)
            {
                yield return new WaitForSeconds(waitDuration);
                if (_instantChange)
                {
                    GameUtility.ChangeAlphaSprite(_spriteRenderer, 1f);
                    yield return new WaitForSeconds(fadeDuration);
                    GameUtility.ChangeAlphaSprite(_spriteRenderer, 0f);
                }
                else
                {
                    yield return _spriteRenderer.DOFade(1f, fadeDuration).WaitForCompletion();
                    yield return _spriteRenderer.DOFade(0f, fadeDuration).WaitForCompletion();
                }

            }
        }
    }
}