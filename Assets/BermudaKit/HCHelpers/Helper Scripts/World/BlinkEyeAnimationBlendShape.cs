using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class BlinkEyeAnimationBlendShape : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
        [SerializeField] private string[] _eyeBSNames;
        [SerializeField] private float _closeEyeWeight = 100;
        [SerializeField] private float _openEyeWeight = 0;
        [SerializeField] private float _blinkSpeed = 5f;

        private float _currentWeight = 0;
        private float _targetWeight = 0;
        private bool _opened = true;
        void Start()
        {
            _targetWeight = _skinnedMeshRenderer.GetBlendShapeWeight(GameUtility.GetBSIndexByName(_skinnedMeshRenderer, _eyeBSNames[0]));
            _currentWeight = _targetWeight;
            StartCoroutine(BlinkLoopProcess());
        }

        private void Update()
        {
            UpdateWeights();
        }

        void SwitchStatus()
        {
            if (_opened)
            {
                CloseEyes();
            }
            else
            {
                OpenEyes();
            }
        }

        void CloseEyes()
        {
            if (!_opened)
            {
                return;
            }
            _targetWeight = _closeEyeWeight;
            _opened = false;
        }

        void OpenEyes()
        {
            if (_opened)
            {
                return;
            }
            _targetWeight = _openEyeWeight;
            _opened = true;
        }

        void UpdateWeights()
        {
            _currentWeight = Mathf.Lerp(_currentWeight, _targetWeight, _blinkSpeed * Time.deltaTime);
            for (int i = 0; i < _eyeBSNames.Length; i++)
            {
                _skinnedMeshRenderer.SetBlendShapeWeight(GameUtility.GetBSIndexByName(_skinnedMeshRenderer, _eyeBSNames[i]), _currentWeight);
            }
        }

        IEnumerator BlinkLoopProcess()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1f, 3f));
                CloseEyes();
                yield return new WaitForSeconds(0.3f);
                OpenEyes();
            }
        }
    }
}