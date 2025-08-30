using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ali.Helper.World
{
    public class ColliderNotifier : MonoBehaviour
    {
        [SerializeField] private bool _isTrigger = false;
        [SerializeField] private string[] _requiredTags;
        public event System.Action<GameObject> OnCollided;

        private void OnTriggerEnter(Collider other)
        {
            if (!_isTrigger)
            {
                return;
            }
            if (_requiredTags.Length > 0)
            {
                for (int i = 0; i < _requiredTags.Length; i++)
                {
                    if (other.transform.tag.Equals(_requiredTags[i]))
                    {
                        OnCollided.Invoke(other.gameObject);
                        return;
                    }
                }
            }
            else
            {
                OnCollided.Invoke(other.gameObject);
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isTrigger)
            {
                return;
            }
            if (_requiredTags.Length > 0)
            {
                for (int i = 0; i < _requiredTags.Length; i++)
                {
                    if (collision.transform.tag.Equals(_requiredTags[i]))
                    {
                        OnCollided.Invoke(collision.gameObject);
                        return;
                    }
                }
            }
            else
            {
                OnCollided.Invoke(collision.gameObject);
            }
        }
    }
}