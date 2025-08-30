using Ali.Helper;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupObjectUI : MonoBehaviour
{
    [SerializeField] private float _upDistance;
    [SerializeField] private float _upSpeed;
    [Space]
    [SerializeField] private TMP_Text _textComponent;

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private bool _showing = false;

    public void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void SetColor(Color color)
    {
        _textComponent.color = color;
    }

    public void SetText(string text)
    {
        _textComponent.text = text;
    }

    public void SetPositionByWorld(Vector3 worldPos)
    {
        Vector3 canvasPos = GameUtility.GetCanvasPositionFromWorldPosition(worldPos, _canvas.GetComponent<RectTransform>());
        _rectTransform.anchoredPosition = canvasPos;
    }

    public bool IsShowing()
    {
        return _showing;
    }

    public void Show()
    {
        if(_showing)
        {
            return;
        }
        _showing = true;
        StartCoroutine(ShowProcess());
    }

    IEnumerator ShowProcess()
    {
        _textComponent.enabled = true;
        _textComponent.DOFade(0f, _upSpeed * 0.005f).SetDelay(0.3f);
        yield return _rectTransform.DOAnchorPosY(_upDistance, _upSpeed).SetRelative().SetSpeedBased().WaitForCompletion();
        _textComponent.enabled = true;
        _textComponent.DOKill(true);
        _rectTransform.DOKill();
        _showing = false;
    }
}
