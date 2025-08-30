using System;
using System.Collections;
using System.Collections.Generic;
using Ali.Helper;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GunController : LocalSingleton<GunController>
{
    [SerializeField] private List<Gun> _guns;
    [Space]
    [SerializeField] private bool _isSpreadShotActive = false;
    
    private float _projectileSpeed = 20f;
    private float _spreadShotAngle = 60f;
    private bool _throwing = false;
    private int _gunLevel = 0;
    private int _projectileCount = 1;
    public bool IsInitted { get; set; } = false;
    
    public static event Action OnGunLevelUpdated;

    public void Init()
    {
        GameManager.OnGameplayStarted += StartThrowingForAllGuns;
        GameManager.OnGameplayEnded += StopThrowingForAllGuns;

        _projectileSpeed = GameManager.Instance.GameConfig().ProjectileThrowSpeed;
        _spreadShotAngle = GameManager.Instance.GameConfig().SpreadShotAngle;
        //_gunLevel = PlayerPrefs.GetInt("GUN_LEVEL", 0);
        _gunLevel = 0;
        
        foreach (var gun in _guns)
        {
            gun.SetGunLevel(_gunLevel);
            gun.Init();
        }

        UpgradeManager.Instance.UpdateAllAttributes();
        IsInitted = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)) UpgradeAllGunsLevel();
        if(Input.GetKeyDown(KeyCode.V)) DowngradeAllGunsLevel();
    }

    private void OnDestroy()
    {
        GameManager.OnGameplayStarted -= StartThrowingForAllGuns;
        GameManager.OnGameplayEnded -= StopThrowingForAllGuns;
    }
    public void StartThrowingForAllGuns()
    {
        if(_throwing || GameManager.Instance.IsGamePlayFinished || !GameManager.Instance.IsGamePlayStarted)
        {
            return;
        }
        _throwing = true;
        foreach (var gun in _guns)
        {
            gun.StartShooting();
        }
    }
    public void StopThrowingForAllGuns()
    {
        _throwing = false;
        foreach (var gun in _guns)
        {
            gun.StopShooting();
        }
    }

    public IEnumerator StopThrowingAndStartAgainForAllGuns()
    {
        StopThrowingForAllGuns();
        yield return new WaitForSeconds(0.25f);
        StartThrowingForAllGuns();
    }
    public void UpgradeAllGunsLevel()
    {
        _gunLevel = Mathf.Min(++_gunLevel, _guns[0].GetGunModelsCount()- 1);
        //PlayerPrefs.SetInt("GUN_LEVEL", _gunLevel);
        foreach (var gun in _guns)
        {
            gun.IncreaseLevel(1);
        }
    }
    public void DowngradeAllGunsLevel(int decreaseAmount = 1)
    {
        _gunLevel = Mathf.Max(_gunLevel -= decreaseAmount, 0);
        //PlayerPrefs.SetInt("GUN_LEVEL", _gunLevel);
        foreach (var gun in _guns)
        {
            gun.DecreaseLevel(decreaseAmount);
        }
    }
    public bool IsThrowing() => _throwing;
    public void SetIsThrowing(bool value) => _throwing = value;

    public List<Gun> GetGuns() => _guns;
    public float GetProjectileSpeed() => _projectileSpeed;
    public void SetProjectileSpeed(float newProjectileSpeed) => _projectileSpeed = newProjectileSpeed;
    public bool GetIsSpreadShotActive() => _isSpreadShotActive;
    public void SetSpreadShotActive(bool value) => _isSpreadShotActive = value;
    public int GetGunLevel() => _gunLevel;
    public void SetGunLevel(int newLevel)
    {
        StartCoroutine(StopThrowingAndStartAgainForAllGuns());
        _gunLevel = newLevel;
        //PlayerPrefs.SetInt("GUN_LEVEL", _gunLevel);
        foreach (var gun in _guns)
        {
            gun.SetGunLevel(_gunLevel);
        }
        OnGunLevelUpdated?.Invoke();
    }
    public float SpreadShotAngle() => _spreadShotAngle;
    public int GetProjectileCount() => _projectileCount;
    public void SetProjectileCount(int value) => _projectileCount = value;
}
