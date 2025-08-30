using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class BSFaceAnimator : MonoBehaviour
    {
        private SkinnedMeshRenderer _smr;
        private Tweener[] _tweenBSArray;
        private void Start()
        {
            _smr = GetComponent<SkinnedMeshRenderer>();
            _tweenBSArray = new Tweener[_smr.sharedMesh.blendShapeCount];
        }

        public void MoveBSValueTo(string bsName, float value, float duration)
        {
            int bsIndex = GameUtility.GetBSIndexByName(_smr, bsName);
            if(bsIndex == -1)
            {
                return;
            }

            if(_tweenBSArray[bsIndex] != null)
            {
                _tweenBSArray[bsIndex].Kill();
                _tweenBSArray[bsIndex] = null;
            }
            _tweenBSArray[bsIndex] = DOTween.To(() => _smr.GetBlendShapeWeight(bsIndex), x => _smr.SetBlendShapeWeight(bsIndex, x), value, duration);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _tweenBSArray.Length; i++)
            {
                if(_tweenBSArray[i] != null)
                {
                    _tweenBSArray[i].Kill();
                }
            }
        }
    }
}