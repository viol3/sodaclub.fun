using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ali.Helper.UI
{
    public class CurrencyPanel : MonoBehaviour
    {
        [SerializeField] private SmoothNumberText _currencyText;
        [SerializeField] private Image _currencyIconImage;
        [SerializeField] private float _punchIconRate = 0.05f;
        [SerializeField] private float _punchTextRate = 0.1f;
        [Space]
        [SerializeField] private bool _particleEnabled = true;
        [SerializeField] private int _particleCount = 20;
        [SerializeField] private Vector3 _particleScale;
        [SerializeField] private Vector2 _particleTargetOffset;


        private CurrencyParticle[] _particles;
        private RectTransform _rectTransform;

        public event System.Action OnParticleArrived;
        private void Start()
        {
            _rectTransform = (RectTransform)transform;
            //InitParticles();
        }

        void InitParticles()
        {
            if(!_particleEnabled)
            {
                return;
            }
            _particles = new CurrencyParticle[_particleCount];
            for (int i = 0; i < _particles.Length; i++)
            {
                RectTransform currencyParticle = new GameObject("currencyParticle", typeof(RectTransform)).GetComponent<RectTransform>();
                currencyParticle.SetParent(transform.parent);
                currencyParticle.gameObject.AddComponent<CanvasRenderer>();
                currencyParticle.sizeDelta = _currencyIconImage.rectTransform.sizeDelta;
                currencyParticle.localScale = _particleScale;
                Image particleImage = currencyParticle.gameObject.AddComponent<Image>();
                particleImage.sprite = _currencyIconImage.sprite;
                particleImage.raycastTarget = false;
                _particles[i] = particleImage.gameObject.AddComponent<CurrencyParticle>();
                _particles[i].Init();
                _particles[i].OnArrived += CurrencyParticle_OnArrived;
                RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
                _particles[i].SetTargetPosition(GameUtility.GetCanvasPositionFromWorldPosition(transform.position, canvasRect));
            }
        }

        private void CurrencyParticle_OnArrived()
        {
            OnParticleArrived?.Invoke();
        }

        public void SetCurrencyAmount(float amount, bool instantly = false)
        {
            if(instantly)
            {
                _currencyText.SetPointsInstantly(amount);
            }
            else
            {
                _currencyText.SetPoints(amount);
                PunchIcon();
                PunchText();
            }
        }

        void PunchIcon()
        {
            _currencyIconImage.rectTransform.DOKill(true);
            _currencyIconImage.rectTransform.DOPunchScale(Vector3.one * _punchIconRate, 0.3f, 6);
        }

        void PunchText()
        {
            _currencyText.transform.DOKill(true);
            _currencyText.transform.DOPunchScale(Vector3.one * _punchTextRate, 0.6f, 6);
        }

        public void GenerateCurrencyParticles(Vector2 spawnPosition)
        {
            if (!_particleEnabled)
            {
                return;
            }
            for (int i = 0; i < _particleCount; i++)
            {
                _particles[i].Travel(spawnPosition);
            }
            
        }
    }
}