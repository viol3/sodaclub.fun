using Ali.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.Particle
{
    public class ParticleGenerator : LocalSingleton<ParticleGenerator>
    {
        [SerializeField] private GameObject[] _particlePrefabs;

        public GameObject SpawnParticle(string particleName, Vector3 position, float deathDuration)
        {
            GameObject particleGO = GetParticleByName(particleName);
            if (particleGO)
            {
                ParticleSystem particle = Instantiate(particleGO).GetComponent<ParticleSystem>();
                particle.transform.position = position;
                Destroy(particle.gameObject, deathDuration);
                return particle.gameObject;
            }
            return null;
        }

        private GameObject GetParticleByName(string particleName)
        {
            for (int i = 0; i < _particlePrefabs.Length; i++)
            {
                if (_particlePrefabs[i].name.Equals(particleName))
                {
                    return _particlePrefabs[i];
                }
            }
            return null;
        }
    }
}