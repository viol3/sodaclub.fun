using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using Bermuda.Animation;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private List<GameObject> _gunModels;
    [SerializeField] private int _gunLevel;
    [SerializeField] private ParticleSystem _levelUpParticle;
    [SerializeField] private Transform _projectilesParent;
    [Space]
    [SerializeField] private List<Projectile> _currentProjectileList = new List<Projectile>();

    [Space] [Header("Projectiles Offset Settings")] 
    [SerializeField] private List<Vector3> _projectilesOffset = new();

    private Transform _currentGunModel;
    private SimpleAnimancer _simpleAnimancer;
    private AnimancerComponent _animancer;
    private Coroutine _gunAnim;
    private Tweener _punchTweener;
    private int _placingProjectileCount;
    private bool _placingProjectile = false;
    private bool _isInitted = false;
    private bool _isEndGameStarted = false;
    public bool IsThrowing { get; set; } = false;
    private Projectile _middleProjectile;
    public void Init()
    {
        GunController.OnGunLevelUpdated += UpdateGunView;
        UpdateGunView();
        _isInitted = true;
        _middleProjectile = _currentProjectileList[1];
        foreach (var projectile in _currentProjectileList)
        {
            projectile.Init();
        }
    }
    private void OnDestroy()
    {
        GunController.OnGunLevelUpdated -= UpdateGunView;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndGame") && !_isEndGameStarted)
        {
            _isEndGameStarted = true;
            CameraController.Instance.SetPov(CameraPov.EndGame);
        }
    }
    public void StartShooting()
    {
        if(IsThrowing || GameManager.Instance.IsGamePlayFinished || !GameManager.Instance.IsGamePlayStarted)
        {
            return;
        }
        IsThrowing = true;
        GetAnimancer().PlayAnimation("Throw");
        UpgradeManager.Instance.UpdateFireRate();
    }
    public void StopShooting()
    {
        IsThrowing = false;
        GetAnimancer().Stop();
    }
    public void UpdateGunView()
    {
        foreach (var gunModel in _gunModels)
        {
            gunModel.SetActive(false);
        }
        
        if (_gunLevel < _gunModels.Count)
        {
            var activeModel = _gunModels[_gunLevel];
            activeModel.SetActive(true);
            
            _simpleAnimancer = activeModel.GetComponent<SimpleAnimancer>();
            _animancer = activeModel.GetComponent<AnimancerComponent>();
            _currentGunModel = activeModel.transform;
            
            if (_isInitted && _levelUpParticle != null)
            {
                _levelUpParticle.Play();
            }
        }
    }
    public void ThrowProjectile()
    {
        if(_placingProjectile || _currentProjectileList.Count == 0 || !GunController.Instance.IsThrowing())
        {
            return;
        }
        ThrowProjectileProcess();
    }

    private void ThrowProjectileProcess()
    {
        foreach (var projectile in _currentProjectileList)
        {
            projectile.Throw();
        }
        _currentProjectileList.Clear();
    }
    public void PlaceProjectile()
    {
        if(_placingProjectile)
        {
            return;
        }
        PlaceArrowProcess();
    }
    private void PlaceArrowProcess()
    {
        _placingProjectile = true;

        var arrowCount = GunController.Instance.GetProjectileCount();
        var spacing = 0.1f;

        for (var i = 0; i < arrowCount; i++)
        {
            var projectile = PoolManager.Instance.SpawnProjectile().GetComponent<Projectile>();
            projectile.Init();
            projectile.transform.SetParent(transform);
            
            var positionOffset = (i - (arrowCount - 1) / 2.0f) * spacing;
            projectile.transform.localPosition = new Vector3(positionOffset, 0.055f, 0.08f);
            
            if (GunController.Instance.GetIsSpreadShotActive())
            {
                projectile.transform.localEulerAngles = new Vector3(0, positionOffset * GunController.Instance.SpreadShotAngle(), 0f);
            }
            else
            {
                projectile.transform.localEulerAngles = Vector3.zero;
            }

            //projectile.transform.DOLocalMoveZ(-0.5f, _simpleAnimancer.GetDuration() / 2f).WaitForCompletion(); // if its arrow u can use it
            _currentProjectileList.Add(projectile);
        }

        _placingProjectile = false;
    }
    public void IncreaseLevel(int increaseAmount = 1)
    {
        if (_gunLevel < _gunModels.Count - 1)
        {
            GunController.Instance.StopThrowingForAllGuns();
            _gunLevel = Mathf.Min(_gunLevel += increaseAmount, _gunModels.Count - 1) ;
            UpdateGunView();
            GunController.Instance.StartThrowingForAllGuns();
        }
    }
    public void DecreaseLevel(int decreaseAmount = 1)
    {
        if (_gunLevel > 0)
        {
            GunController.Instance.StopThrowingForAllGuns();
            _gunLevel = Mathf.Max(_gunLevel -= decreaseAmount, 0) ;
            UpdateGunView();
            GunController.Instance.StartThrowingForAllGuns();
        }
    }
    public void SetCurrentListClear()
    {
        _currentProjectileList.Clear();
    }
    public void Punch()
    {
        _punchTweener?.Complete();
        _punchTweener = transform.DOPunchScale(transform.localScale * 0.2f, 0.25f);
    }
    public List<Projectile> GetCurrentProjectiles() => _currentProjectileList;
    public int GetGunLevel() => _gunLevel;
    public void SetGunLevel(int value)
    {
        _gunLevel = value;
    }
    public SimpleAnimancer GetAnimancer() => _simpleAnimancer;
    public AnimancerComponent GetAnimancerComponent() => _animancer;
    public Transform GetCurrentGunModel() => _currentGunModel;
    public Transform GetProjectilesParent() => _projectilesParent;
    public int GetGunModelsCount() => _gunModels.Count;

}
