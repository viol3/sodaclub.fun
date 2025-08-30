using System;
using System.Collections;
using Ali.Helper;
using Ali.Helper.Audio;
using Bermuda.Runner;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class HangeableObjectStand : MonoBehaviour
{
    [SerializeField] private HangeableObject _hangeableObject;
    [SerializeField] private Canvas _progressBarCanvas;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TextMeshProUGUI _currentLevelText, _nextLevelText;
    [Space]
    [SerializeField] private int[] _levelThresholds;
    [SerializeField] private int _level = 0;
    
    private Tweener _fillTween, _punchTween;
    private float _currentPoints = 0;
    private float _targetFillAmount;
    
    private void Start()
    {
        _progressBarCanvas.worldCamera = Camera.main!;
        _targetFillAmount = 0;
        _fillImage.fillAmount = 0;
        _currentPoints = 0;
        _hangeableObject.IsHanged = false;
        _hangeableObject.SetLevel(_level);
        UpdateLevelTexts();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Projectile>() && !_hangeableObject.IsCollected)
        {
            var projectile = other.GetComponent<Projectile>();
            
            if (projectile.IsTriggered) return;
            projectile.IsTriggered = true;
            
            var hitPoint = projectile.GetFrontPoint().transform.position;
            var hitParticle = PoolManager.Instance.SpawnHitParticle().GetComponent<HitParticleController>();
            hitParticle.transform.SetParent(HCLevelManager.Instance.GetCurrentLevel().transform);
            hitParticle.PlayHitParticle(hitPoint);

            var damage = UpgradeManager.Instance.Damage;
            TakeHit(damage);
            Punch();
            
            PoolManager.Instance.DespawnProjectile(projectile.gameObject);
            HapticManager.Haptic(0);
            AudioPool.Instance.PlayClipByName("hexHit", false, 0.1f);
        }
    }

    public void Collect()
    {
        _hangeableObject.IsCollected = true;
        MoveToBelt(0);
        AudioPool.Instance.PlayClipByName("itemPickUp", false, 0.2f);
    }
    public void AddPoints(float points)
    {
        _currentPoints += points;
        var maxLevel = _levelThresholds.Length - 1;

        if (_level < maxLevel && _currentPoints >= _levelThresholds[_level])
        {
            _currentPoints -= _levelThresholds[_level];
            _level++;
            _hangeableObject.SetLevel(_level);
            SmoothFillBar();
            UpdateLevelTexts();
            Punch();
        }
        else
        {
            SmoothFillBar();
        }
    }
    private void SmoothFillBar()
    {
        _targetFillAmount = _currentPoints / _levelThresholds[_level];

        if (_fillTween != null && _fillTween.IsActive() && !_fillTween.IsComplete())
        {
            _fillTween.ChangeEndValue(_targetFillAmount, true).Restart();
        }
        else
        {
            var fillSpeed = UpgradeManager.Instance.FireRate + UpgradeManager.Instance.TempFireRate;
            _fillTween = _fillImage.DOFillAmount(_targetFillAmount, fillSpeed * 1.1f).SetEase(Ease.OutQuad).SetSpeedBased();
        }
    }
    public void MoveToBelt(float delay)
    {
        _progressBarCanvas.gameObject.SetActive(false);
        StartCoroutine(MoveSequence(delay));
    }

    private IEnumerator MoveSequence(float delay)
    {
        const float duration = 0.5f;
        
        yield return new WaitForSeconds(delay * 0.2f);
        _hangeableObject.DOKill(true);
        _hangeableObject.transform.SetParent(null);

        _hangeableObject.transform.DORotate(Vector3.zero, duration);
        yield return _hangeableObject.transform.DOJump(new Vector3(-3f, 0.185f, BermudaRunnerCharacter.Instance.GetLocalMover().position.z + 4f),
            2f, 1, duration).WaitForCompletion();

        _hangeableObject.transform.DOMoveZ(999f, 10f).SetSpeedBased().SetRelative();
    }
    public void TakeHit(float damageAmount)
    {
        AddPoints(damageAmount * GameManager.Instance.GameConfig().HangeableObjectHitMultiplier);
    }

    private void UpdateLevelTexts()
    {
        _currentLevelText.text = (_level).ToString();
        _nextLevelText.text = (_level + 1).ToString();
    }
    private void Punch()
    {
        _punchTween?.Complete();
        _punchTween = transform.DOPunchScale(transform.localScale * 0.2f, 0.25f);
    }
    public void SetLevelTextVisibility(bool isVisible)
    {
        _currentLevelText.gameObject.SetActive(isVisible);
        _nextLevelText.gameObject.SetActive(isVisible);
    }
    
    public HangeableObject GetHangeableObject() => _hangeableObject;
}
