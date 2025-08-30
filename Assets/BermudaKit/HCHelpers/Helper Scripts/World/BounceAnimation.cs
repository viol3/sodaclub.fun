using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class BounceAnimation : MonoBehaviour
    {
        [SerializeField] private float _yOffset = -1f;
        [SerializeField] private float _duration = 0.5f;
        void Start()
        {
            transform.DOLocalMoveY(_yOffset, _duration).SetRelative().SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        }

    }
}