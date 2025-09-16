using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteryGunRotateTest : MonoBehaviour
{
    [SerializeField] private Transform[] _cylinderSets;

    private int _setIndex = 0;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            _cylinderSets[_setIndex].DOKill(true);
            _cylinderSets[_setIndex].DOLocalRotate(Vector3.forward * 90f, 0.5f).SetEase(Ease.OutBack).SetRelative();
            _setIndex = (_setIndex + 1) % _cylinderSets.Length;
        }
    }
}
