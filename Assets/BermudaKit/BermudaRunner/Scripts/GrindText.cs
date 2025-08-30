using Ali.Helper;
using Ali.Helper.Audio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrindText : LocalSingleton<GrindText>
{
    [SerializeField] private int _startValue = 1;
    [SerializeField] private int _incrementalValue = 1;
    [SerializeField] private float _moveYOffset = 50;
    [SerializeField] private float _moveDuration = 0.3f;
    [SerializeField] private float _startPitch = 0.8f;
    [SerializeField] private float _pitchIncremental = 0.1f;
    [SerializeField] private float _localScale = 3;
    [Space]
    [SerializeField] private TextMeshProUGUI _grindText;
    private int _value = 1;
    private float _pitch = 1;

    public void Init()
    {
        ResetGrind();
    }

    public void Grind()
    {
        Stop();
        _grindText.text = _value + "X";
        GameUtility.ChangeAlphaText(_grindText, 1f);
        _grindText.rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 6);
        _grindText.rectTransform.DOAnchorPosY(_moveYOffset, _moveDuration).SetRelative().OnComplete(() => 
        {
            Stop();
        }) ;
        _value += _incrementalValue;
        AudioPool.Instance.PlayClipByName("grind", false, 0.2f, _pitch);
        _pitch += _pitchIncremental;
    }

    public void ResetGrind()
    {
        _value = _startValue;
        _pitch = _startPitch;
    }

    public void Stop()
    {
        _grindText.rectTransform.DOKill();
        _grindText.rectTransform.anchoredPosition = Vector2.zero;
        GameUtility.ChangeAlphaText(_grindText, 0f);
        _grindText.rectTransform.localScale = Vector3.one * _localScale;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Grind();
        }
    }

}
