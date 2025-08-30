using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindObject : MonoBehaviour
{
    [SerializeField] private float _explosionForce;
    [Space]
    [SerializeField] private Transform _modelTransform;

    private Rigidbody _rigidbody;
    private Collider _collider;

    private bool _exploded = false;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponentInChildren<Collider>();
    }

    public bool IsExploded()
    {
        return _exploded;
    }

    public void Explode()
    {
        if(_exploded)
        {
            return;
        }
        Vector3 origin = transform.parent.position;// + Vector3.forward * 0.5f + Vector3.right * (((Random.value - 0.5f) * 2f) * 0.5f);
        origin.y = 0;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _collider.isTrigger = false;
        _rigidbody.AddExplosionForce(_explosionForce, origin, 3f);
        _exploded = true;
        StartCoroutine(AfterExplosion());
    }

    IEnumerator AfterExplosion()
    {
        yield return new WaitForSeconds(2f);
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        transform.DOJump(CameraRayPointManager.Instance.GetNearest(transform.position), 1f, 1, 0.5f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        //_modelTransform.gameObject.SetActive(false);
        //yield return new WaitForSeconds(0.5f);
        
    }
}
