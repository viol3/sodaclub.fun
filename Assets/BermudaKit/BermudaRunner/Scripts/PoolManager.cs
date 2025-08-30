using Ali.Helper;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
public class PoolManager : LocalSingleton<PoolManager>
{
    [SerializeField] private LeanGameObjectPool _projectilePool;
    [SerializeField] private LeanGameObjectPool _cashPool;
    [SerializeField] private LeanGameObjectPool _cashPopupPool;
    [SerializeField] private LeanGameObjectPool _hitParticlePool;
    [SerializeField] private LeanGameObjectPool _woodParticlePool;

    public GameObject SpawnProjectile()
    {
        GameObject result = null;
        _projectilePool.TrySpawn(ref result);
        result.GetComponent<Projectile>().IsSpawnedByPool = true;
        return result;
    }
    public GameObject SpawnHitParticle()
    {
        GameObject result = null;
        _hitParticlePool.TrySpawn(ref result);
        return result;
    }
    public GameObject SpawnCash()
    {
        GameObject result = null;
        _cashPool.TrySpawn(ref result);
        return result;
    }
    public GameObject SpawnCashPopup()
    {
        GameObject result = null;
        _cashPopupPool.TrySpawn(ref result);
        return result;
    }
    public GameObject SpawnWoodParticle()
    {
        GameObject result = null;
        _woodParticlePool.TrySpawn(ref result);
        return result;
    }
    public void DespawnProjectile(GameObject projectile)
    {
        projectile.transform.DOKill();
        
        if (projectile.GetComponent<Projectile>().IsSpawnedByPool)
        {
            _projectilePool.Despawn(projectile);
        }
        else
        {
            Destroy(projectile);
        }
    }
    public void DespawnHitParticle(GameObject particle) { _hitParticlePool.Despawn(particle);}
    public void DespawnCash(GameObject cash) { _cashPool.Despawn(cash);}
    public void DespawnCashPopup(GameObject cashPopup) { _cashPopupPool.Despawn(cashPopup);}
    public void DespawnWoodParticle(GameObject wood) { _woodParticlePool.Despawn(wood); }
}
