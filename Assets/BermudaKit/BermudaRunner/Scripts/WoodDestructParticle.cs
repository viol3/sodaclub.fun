using System.Collections;
using UnityEngine;

public class WoodDestructParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _woodDestructParticle;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.5f);
        PoolManager.Instance.DespawnWoodParticle(_woodDestructParticle.gameObject);
    }
    
}
