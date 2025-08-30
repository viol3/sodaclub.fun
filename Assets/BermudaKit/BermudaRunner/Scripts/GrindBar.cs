using System;
using System.Collections.Generic;
using Ali.Helper;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GrindBar : LocalSingleton<GrindBar>
{
    [SerializeField] private Image _fillBar;
    [SerializeField] private ParticleSystem _barParticle;
    [SerializeField] private Transform _grindBarProgressModel;
    [Space] 
    [SerializeField] private Image _currentGunImage;
    
    [Space] 
    [SerializeField] private Image _nextGunImage;

    [SerializeField] private TextMeshProUGUI _currentGunLevelText;
    [SerializeField] private TextMeshProUGUI _nextGunLevelText;

    private const string PREFS_BASE_LEVEL = "BaseLevel";
    private const string PREFS_CURRENT_POINTS = "CurrentPoints";
    
    private float _currentPoints = 0;
    private int _currentBarLevel = 0;

    private Tweener _progressTween;
    private Tweener _currentGunImagePunchTween, _nextGunImagePunchTween;
    private float _targetFillAmount;
    private GameConfig _gameConfig;
    
    public static event Action OnBarLevelUp;

    public void Init()
    {
        _gameConfig = GameManager.Instance.GameConfig();
        LoadProgress();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) IdleButtonUpgrade();
        _barParticle.transform.localPosition = new Vector3(-3f, Mathf.Lerp(-200f,200f, _fillBar.fillAmount), 0f);
    }
    private void LoadProgress()
    {
        _currentBarLevel = PlayerPrefs.GetInt(PREFS_BASE_LEVEL, _currentBarLevel);
        _currentPoints = PlayerPrefs.GetFloat(PREFS_CURRENT_POINTS, _currentPoints);
        UpdateLevelDisplay();
    }
    public void IdleButtonUpgrade()
    {
        AddPoints(_gameConfig.GrindBarLevelThresholds[_currentBarLevel] / 10f);
        SaveProgress();
    }
    private void SaveProgress()
    {
        PlayerPrefs.SetInt(PREFS_BASE_LEVEL, _currentBarLevel);
        PlayerPrefs.SetFloat(PREFS_CURRENT_POINTS, _currentPoints);
    }
    private void UpdateLevelDisplay()
    {
        _fillBar.fillAmount = _currentPoints / _gameConfig.GrindBarLevelThresholds[_currentBarLevel];
        _currentGunLevelText.text = (_currentBarLevel + 1).ToString();
        _nextGunLevelText.text = (_currentBarLevel + 2).ToString();
    }

    public void AddPoints(float points)
    {
        _currentPoints += points;
        var maxLevel = _gameConfig.GrindBarLevelThresholds.Length - 1;

        if (_currentBarLevel < maxLevel && _currentPoints >= _gameConfig.GrindBarLevelThresholds[_currentBarLevel])
        {
            _currentPoints -= _gameConfig.GrindBarLevelThresholds[_currentBarLevel];
            _currentBarLevel++;
            SmoothFillBar();
            UpdateLevelDisplay();
            PunchImages();
            //UpgradeManager.Instance.UpdateMultiplier();
            OnBarLevelUp?.Invoke();
        }
        else
        {
            SmoothFillBar();
        }
        SaveProgress();
    } 
    private void SmoothFillBar()
    {
        _targetFillAmount = _currentPoints / _gameConfig.GrindBarLevelThresholds[_currentBarLevel];

        if (_progressTween != null && _progressTween.IsActive() && !_progressTween.IsComplete())
        {
            _progressTween.ChangeEndValue(_targetFillAmount, true).Restart();
        }
        else
        {
            _progressTween = _fillBar.DOFillAmount(_targetFillAmount, 0.5f).SetEase(Ease.OutQuad);
        }
        PlayBarParticle();
    }
    private void PunchImages()
    {
        _currentGunImagePunchTween?.Complete();
        _nextGunImagePunchTween?.Complete();

        _currentGunImagePunchTween = _currentGunImage.transform.DOPunchScale(_currentGunImage.transform.localScale * 0.25f, 0.25f);
        _nextGunImagePunchTween = _nextGunImage.transform.DOPunchScale(_nextGunImage.transform.localScale * 0.25f, 0.25f);
    }
    public Image GetFillBar() => _fillBar;
    public void PlayBarParticle() => _barParticle?.Play(true);
    public Transform GetGrindBarProgressModel() => _grindBarProgressModel;
    public int GetCurrentLevel() => _currentBarLevel;
}
