using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class CollisionParticle : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _particles;
        [SerializeField] private bool _oneTime = true;
        [SerializeField] private Collider _collider;
        [SerializeField] private string[] _tags;
        [SerializeField] private bool _isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (!_isTrigger)
            {
                return;
            }
            if (_tags.Length == 0)
            {
                PlayParticle();
            }
            else
            {
                for (int i = 0; i < _tags.Length; i++)
                {
                    if (other.tag.Equals(_tags[i]))
                    {
                        PlayParticle();
                        return;
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isTrigger)
            {
                return;
            }
            if (_tags.Length == 0)
            {
                PlayParticle();
            }
            else
            {
                for (int i = 0; i < _tags.Length; i++)
                {
                    if (collision.gameObject.tag.Equals(_tags[i]))
                    {
                        PlayParticle();
                        return;
                    }
                }
            }
        }

        void PlayParticle()
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Play();
            }
            if (_oneTime)
            {
                _collider.enabled = false;
            }
        }
    }
}