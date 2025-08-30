using System;
using Ali.Helper.Audio;
using DG.Tweening;
using UnityEngine;

public class Cash : MonoBehaviour
{
    [SerializeField] private int _cashValue = 5;
    [Space] 
    [SerializeField] private Transform _cashModels;
    [SerializeField] private bool _isMoving = false;
    [SerializeField] private float _firstPosY, _lastPosY;
    [SerializeField] private float _moveSpeed = 5f;
    public bool IsCollected { get; set; } = false;

    private void Start()
    {
        IsCollected = false;
        CheckMove();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !IsCollected)
        {
            Collect();
        }
    }

    private void Collect()
    {
        IsCollected = true;
        UIManager.Instance.SpawnCashPopup(transform.position, _cashValue);
        AudioPool.Instance.PlayClipByName("itemPickUp", false, 0.2f);
        gameObject.SetActive(false);
    }
    private void CheckMove()
    {
        if (_isMoving)
        {
            var moverObject = _cashModels;
        
            var firstTarget = moverObject.localPosition.y >= 0.35 ? _lastPosY : _firstPosY;
            var secondTarget = moverObject.localPosition.y >= 0.35 ? _firstPosY : _lastPosY;

            moverObject.DOLocalMoveY(firstTarget, _moveSpeed).SetSpeedBased().OnComplete(() =>
            {
                moverObject.DOLocalMoveY(secondTarget, _moveSpeed).SetSpeedBased().SetLoops(-1, LoopType.Yoyo);
            });
        }
    }
    public int GetValue() { return _cashValue; }
    public void SetCashValue(int newValue) { _cashValue = newValue; }
    public Transform GetCashModelsParent() => _cashModels;
    public void JumpProcess()
    {
        transform.GetComponent<Collider>().enabled = true;
        transform.SetParent(null);
        transform.DOScale(Vector3.one * 0.75f, 0.8f);
        transform.DOLocalRotate(Vector3.up * -180f, 0.8f);
        transform.DOJump(
            new Vector3(transform.position.x, 0f, transform.position.z), 1f, 2, 0.8f);
    }

    public void SetIsMoving(bool value)
    {
        _isMoving = value;
        if(_isMoving) CheckMove();
    }
}