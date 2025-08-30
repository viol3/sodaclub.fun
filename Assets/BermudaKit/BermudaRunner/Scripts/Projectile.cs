using System;
using DG.Tweening;
using System.Collections;
using Ali.Helper;
using TMPro;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _trail;
    [SerializeField] private Transform _frontPoint;
    [SerializeField] private Collider _collider;
    [Space]
    [SerializeField] private Transform _modelTransform;

    private Coroutine _throwCo;
    private Tweener _punchTween;
    public bool IsSpawnedByPool { get; set; } = false;
    public bool IsTriggered { get; set; } = false;
    
    public void Init()
    {
        _modelTransform.gameObject.SetActive(true);
        IsTriggered = false;
        _collider.enabled = false;
    }

    public Coroutine GetThrowCoroutine()
    {
        return _throwCo;
    }
    public void Throw()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        
        if(_throwCo != null)
        {
            StopCoroutine(_throwCo);
            transform.DOKill();
        }
        //Debug.Break();
        _throwCo = StartCoroutine(ThrowProcess());
    }
    private IEnumerator ThrowProcess()
    {
        _trail.gameObject.SetActive(true);
        _collider.enabled = true;
        transform.SetParent(HCLevelManager.Instance.GetCurrentLevel().transform);
        transform.DOKill();
        yield return transform.DOMove(transform.forward * UpgradeManager.Instance.FireRange, GunController.Instance.GetProjectileSpeed())
            .SetRelative().SetSpeedBased().WaitForCompletion();
        yield return DestroyProcess();
    }
    
    private IEnumerator DestroyProcess(bool waitForTrail = true)
    {
        _collider.enabled = false;
        _modelTransform.gameObject.SetActive(false);
        if(waitForTrail)
        {
            yield return new WaitForSeconds(1f);
        }
        
        _trail.gameObject.SetActive(false);
        PoolManager.Instance.DespawnProjectile(gameObject);
        yield return null;
    }
    private void ViewOn()
    {
        _modelTransform.gameObject.SetActive(true);
        _collider.enabled = true;
    }
    public void PunchModel()
    {
        _punchTween?.Complete();
        _punchTween = _modelTransform.DOPunchScale(_modelTransform.localScale * 0.75f, 0.35f);
    }

    public Transform GetFrontPoint() => _frontPoint;
}
