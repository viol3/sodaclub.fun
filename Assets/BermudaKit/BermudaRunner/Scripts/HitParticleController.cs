using System.Collections;
using UnityEngine;

public class HitParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _hitParticles;

    public void PlayHitParticle(Vector3 hitPoint, int particleIndex = 0, Quaternion rotation = default) => StartCoroutine(ParticleProcess(hitPoint, particleIndex, rotation));
    
    private IEnumerator ParticleProcess(Vector3 hitPoint, int particleIndex = 0, Quaternion rotation = default)
    {
        ParticleSystem hitParticle = null;
        transform.position = hitPoint;

        hitParticle = _hitParticles[particleIndex];
            
        if (hitParticle != null)
        {
            //hitParticle.transform.rotation = rotation;
            hitParticle.Play();

            yield return new WaitUntil(() => !hitParticle.isEmitting);
        }

        PoolManager.Instance.DespawnHitParticle(gameObject);
    }
}