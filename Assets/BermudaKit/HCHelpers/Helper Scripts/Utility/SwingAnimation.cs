using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwingAnimation : MonoBehaviour
{
    [SerializeField] private float _yOffset = 0.5f;
    [SerializeField] private float _speed = 1f;

    private Vector3 _firstPosition;

    private void Awake()
    {
        _firstPosition = transform.localPosition;
    }

    void OnEnable()
    {
        StartCoroutine(SwingProcess());
    }

    void OnDisable() 
    {
        StopAllCoroutines();
        transform.DOKill(true);
        transform.localPosition = _firstPosition;
    }

    IEnumerator SwingProcess()
    {
        while(true)
        {
            yield return transform.DOLocalMoveY(-_yOffset, _speed).SetRelative().SetSpeedBased().SetEase(Ease.InOutSine).WaitForCompletion();
            yield return transform.DOLocalMoveY(_yOffset, _speed).SetRelative().SetSpeedBased().SetEase(Ease.InOutSine).WaitForCompletion();
        }
    }
}
