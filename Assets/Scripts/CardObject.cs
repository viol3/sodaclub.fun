using Ali.Helper.Audio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using viol3.SuiWorks.UI;

public class CardObject : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 200f;
    [SerializeField] private Ease _flipInEase;
    [SerializeField] private Ease _flipOutEase;
    [Space]
    [SerializeField] private MultiSpriteOrderer _multiSpriteOrderer;
    [Space]
    [SerializeField] private Transform _inter;
    [SerializeField] private SpriteRenderer _cardBack;
    [SerializeField] private SpriteRenderer _cardFront;
    [SerializeField] private SpriteRenderer _outline;
    [SerializeField] private SpriteRenderer _loading;
    private Coroutine _flipCo;
    private bool _flipping = false;
    private bool _opened = false;
    private bool _clickable = false;

    public event System.Action<CardObject> OnClick;
    public void SetLoading(bool loading)
    {
        //_loading.gameObject.SetActive(loading);
        if(loading)
        {
            _inter.DOShakePosition(1f, 0.08f, 15, 20, false, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
        else
        {
            _inter.DOKill(true);
            _inter.localPosition = Vector3.zero;
        }
            
    }

    public void SetCardSprite(Sprite sprite)
    {
        _cardFront.sprite = sprite;
    }

    public void SetClickable(bool clickable)
    { 
        _clickable = clickable; 
        _outline.gameObject.SetActive(false);
    }

    public void SetOrder(int order)
    {
        _multiSpriteOrderer.SetOrder(order);
    }

    public int GetOrder(int order)
    {
        return _multiSpriteOrderer.GetOrder();
    }

    public bool IsFlipping()
    { 
        return _flipping; 
    }

    public bool IsOpened()
    { 
        return _opened; 
    }

    public void Flip()
    {
        if(_flipping)
        {
            return;
        }
        if(_flipCo != null)
        {
            StopCoroutine(_flipCo);
        }
        _inter.DOKill();
        _flipping = true;
        _opened = !_opened;
        _flipCo = StartCoroutine(FlipProcess());
        AudioPool.Instance.PlayRandomCardFlip();
    }

    IEnumerator FlipProcess()
    {
        yield return _inter.DOLocalRotate(Vector3.up * 90f, _rotateSpeed).SetRelative().SetSpeedBased().SetEase(_flipInEase).WaitForCompletion();
        _cardBack.gameObject.SetActive(!_cardBack.gameObject.activeInHierarchy);
        _cardFront.gameObject.SetActive(!_cardFront.gameObject.activeInHierarchy);
        yield return _inter.DOLocalRotate(Vector3.up * 90f, _rotateSpeed).SetRelative().SetSpeedBased().SetEase(_flipOutEase).WaitForCompletion();
        _flipping = false;
    }

    private void OnMouseEnter()
    {
        if (!_clickable || SuiAccountUI.Instance.IsUIActive())
        {
            return;
        }
        _outline.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (!_clickable || SuiAccountUI.Instance.IsUIActive())
        {
            return;
        }
        _outline.gameObject.SetActive(false);
    }

    private void OnMouseUpAsButton()
    {
        if (!_clickable || SuiAccountUI.Instance.IsUIActive())
        {
            return;
        }
        OnClick?.Invoke(this);
    }
}
