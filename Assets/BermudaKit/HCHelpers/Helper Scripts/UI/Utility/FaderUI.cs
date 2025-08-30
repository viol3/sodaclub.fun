using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Ali.Helper.UI
{
    public class FaderUI : MonoBehaviour
    {
        public Image faderImage;
        public float fadeTime = 0.25f;

        public static FaderUI Instance;
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        public IEnumerator CloseTheater(float time = 0.25f)
        {
            faderImage.raycastTarget = true;
            faderImage.DOFade(1f, time);
            yield return new WaitForSeconds(time);
        }

        public IEnumerator OpenTheater(float time = 0.25f, float delay = 0f)
        {
            faderImage.DOFade(0f, time).SetDelay(delay);
            yield return new WaitForSeconds(time + delay);
            faderImage.raycastTarget = false;
        }
    }
}