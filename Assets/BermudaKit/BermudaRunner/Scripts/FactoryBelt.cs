using System;
using Bermuda.Runner;
using DG.Tweening;
using UnityEngine;

public class FactoryBelt : MonoBehaviour
{
    [SerializeField] private Material _bandMaterial;
    [Space]
    [SerializeField] private float _duration = 150;
    [SerializeField] private bool _isSpeeder;
    [Space][Header("MOVE SETTINGS")]
    [SerializeField] private bool _isMoving;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Vector2 _moveBounds = new();
    
    private bool _isTriggered = false;

    private void Start()
    {
        _bandMaterial.mainTextureOffset = new Vector2(0f, 0f);
        SetOffset();
        CheckMove();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isTriggered)
        {
            _isTriggered = true;
            BermudaRunnerCharacter.Instance.SetForwardSpeed(BermudaRunnerCharacter.Instance.GetInitialSpeed() * (_isSpeeder ? 1.5f : 0.5f));
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _isTriggered)
        {
            _isTriggered = false;
            BermudaRunnerCharacter.Instance.SetForwardSpeed(BermudaRunnerCharacter.Instance.GetInitialSpeed());
        }
    }
    
    private void CheckMove()
    {
        if (_isMoving)
        {
            var leftPos = _moveBounds.x;
            var rightPos = _moveBounds.y;
            var moverObject = transform;
        
            var firstTarget = moverObject.localPosition.x >= 0 ? rightPos : leftPos;
            var secondTarget = moverObject.localPosition.x >= 0 ? leftPos : rightPos;

            moverObject.DOLocalMoveX(firstTarget, _moveSpeed).SetSpeedBased().OnComplete(() =>
            {
                moverObject.DOLocalMoveX(secondTarget, _moveSpeed).SetSpeedBased().SetLoops(-1, LoopType.Yoyo);
            });
        }
    }

    private void OnApplicationQuit()
    {
        _bandMaterial.mainTextureOffset = new Vector2(0f, 0f);
    }

    private void SetOffset()
    {
        if (_bandMaterial != null)
            _bandMaterial.DOOffset(new Vector2(0, -1000), _duration * 1000f);
    }
    public bool GetIsSpeeder() => _isSpeeder;
}