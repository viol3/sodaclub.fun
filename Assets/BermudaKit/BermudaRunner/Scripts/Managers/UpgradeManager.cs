using System;
using Ali.Helper;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradeManager : LocalSingleton<UpgradeManager>
{
    [SerializeField] private IdleButton _damageButton;
    [SerializeField] private IdleButton _fireRateButton;
    [SerializeField] private IdleButton _fireRangeButton;
    [SerializeField] private IdleButton _incomeButton;
    
    [Space] [Header("Attributes")]
    [SerializeField] private int _damage;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _fireRange;
    [SerializeField] private int _income;

    private int _tempFireRateValue = 0;
    private GameConfig _gameConfig;
    
    public static event Action OnTimeValueUpgrade;

    #region PROPERTIES
    public int Damage { get => _damage; set => _damage = value; }
    public float FireRate { get => _fireRate; set => _fireRate = value; }
    public float FireRange { get => _fireRange; set => _fireRange = value; }
    public int Income { get => _income; set => _income = value; }
    public int TempFireRate { get => _tempFireRateValue; set => _tempFireRateValue = value; }

    #endregion
    
    public void Init()
    {
        _damageButton.OnClick += DamageButtonOnClick;
        _fireRateButton.OnClick += FireRateButtonOnClick;
        _fireRangeButton.OnClick += FireRangeButtonOnClick;
        _incomeButton.OnClick += IncomeButton_OnClick;

        _gameConfig = GameManager.Instance.GameConfig();
        _tempFireRateValue = 0;
        UpdateAllAttributes();
    }
    private void OnDestroy()
    {
        _damageButton.OnClick -= DamageButtonOnClick;
        _fireRateButton.OnClick -= FireRateButtonOnClick;
        _fireRangeButton.OnClick -= FireRangeButtonOnClick;
        _incomeButton.OnClick -= IncomeButton_OnClick;
    }

    public int GetDamageButtonLevel()
    {
        return _damageButton.GetValue();
    }
    public int GetFireRateButtonLevel()
    {
        return _fireRateButton.GetValue();
    }
    public int GetFireRangeButtonLevel()
    {
        return _fireRangeButton.GetValue();
    }
    public int GetIncomeButtonLevel()
    {
        return _incomeButton.GetValue();
    }
    private void DamageButtonOnClick(bool success)
    {
        if(success)
        {
            UpdateDamage();
        }
    }
    private void FireRateButtonOnClick(bool success)
    {
        if(success)
        {
            UpdateFireRate();
        }
    }
    private void FireRangeButtonOnClick(bool success)
    {
        if (success)
        {
            UpdateFireRange();
        }
    }
    private void IncomeButton_OnClick(bool success)
    {
        if (success)
        {
            UpdateIncome();
        }
    }
    private float CalculateDamage(int timeValue)
    {
        const float minTimeValueLevel = 1;
        var maxTimeValueLevel = _gameConfig.DamageMaxLevel;
        var timeValueAtMinLevel = _gameConfig.DamageAtMinLevel;
        var timeValueAtMaxLevel = _gameConfig.DamageAtMaxLevel;

        var t = (timeValue - minTimeValueLevel) / (maxTimeValueLevel - minTimeValueLevel);
        t *= GameManager.Instance.GameConfig().DamageIncrementMultiplier;

        return Mathf.Lerp(timeValueAtMinLevel, timeValueAtMaxLevel, t);
    }
    private float CalculateFireRate(int fireRate)
    {
        const float minFireRateLevel = 1;
        var maxFireRateLevel = _gameConfig.FireRateMaxLevel;
        var fireRateAtMinLevel = _gameConfig.FireRateAtMinLevel;
        var fireRateAtMaxLevel = _gameConfig.FireRateAtMaxLevel;

        var t = (fireRate - minFireRateLevel) / (maxFireRateLevel - minFireRateLevel);
        t *= GameManager.Instance.GameConfig().FireRateIncrementMultiplier;
        return Mathf.Lerp(fireRateAtMinLevel, fireRateAtMaxLevel, t);
    }
    private float CalculateFireRange(int fireRangeLevel)
    {
        const float minFireRangeLevel = 1f;
        var maxFireRangeLevel = _gameConfig.FireRangeMaxLevel;
        var fireRangeAtMinLevel = _gameConfig.FireRangeAtMinLevel;
        var fireRangeAtMaxLevel = _gameConfig.FireRangeAtMaxLevel;

        var t = (fireRangeLevel - minFireRangeLevel) / (maxFireRangeLevel - minFireRangeLevel);

        return Mathf.Lerp(fireRangeAtMinLevel, fireRangeAtMaxLevel, t);
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void UpdateDamage(int gateValue = 0)
    {
        var timeValueLevel = GetDamageButtonLevel();
        _damage = (int)CalculateDamage(timeValueLevel + gateValue);
        OnTimeValueUpgrade?.Invoke();
    }
    public void UpdateFireRate()
    {
        if (!GunController.Instance.IsInitted) return;
        
        var fireRateLevel = GetFireRateButtonLevel() + TempFireRate;
        _fireRate = CalculateFireRate(fireRateLevel);
        foreach (var gun in GunController.Instance.GetGuns())
        {
            gun.GetAnimancer().SetStateSpeed(_fireRate);
        }
    }
    public void UpdateFireRange(int gateValue = 0)
    {
        var fireRangeLevel = GetFireRangeButtonLevel();
        _fireRange = CalculateFireRange(fireRangeLevel + gateValue);
    }
    public void UpdateIncome(int gateValue = 0)
    {
        _income = Mathf.Max(1,GetIncomeButtonLevel() + gateValue);
    }
    public void UpdateAllAttributes()
    {
        UpdateFireRate();
        UpdateFireRange();
        UpdateDamage();
        UpdateIncome();
    }
}
