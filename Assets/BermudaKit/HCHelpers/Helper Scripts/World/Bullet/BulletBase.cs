using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public event System.Action<BulletBase, Collision> OnCollideEnter;
    protected Rigidbody _rigidbody;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void ApplyForce(Vector3 velocity)
    {
        _rigidbody.AddForce(velocity, ForceMode.Impulse);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        OnCollideEnter?.Invoke(this, collision);
    }
}
