using System;
using System.Collections;
using System.Collections.Generic;
using Bermuda.Runner;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class HangerArea : MonoBehaviour
{
    [SerializeField] private List<Hanger> _hangers;
    private int _currentHangerIndex = 0;
    private bool _isTriggeredByHand = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HangeableObject>())
        {
            var collectibleProjectile = other.GetComponent<HangeableObject>();
            if(!collectibleProjectile.IsHanged && !(_currentHangerIndex > _hangers.Count - 1))
                HangIt(collectibleProjectile);
        }
        else if (other.CompareTag("Player") && !_isTriggeredByHand)
        {
            _isTriggeredByHand = true;

            StartCoroutine(CollectProcess());
        }
    }
    private void HangIt(HangeableObject hangeableObject)
    {
        if (_hangers[_currentHangerIndex].GetHanged() != null) return;
    
        var tempIndex = _currentHangerIndex;
        const float duration = 0.5f;
        
        _hangers[tempIndex].SetHanged(hangeableObject);
        hangeableObject.GetComponent<Collider>().enabled = false;
        hangeableObject.IsHanged = true;
        hangeableObject.transform.DOKill();
        hangeableObject.transform.SetParent(_hangers[tempIndex].GetHangPoint());
        hangeableObject.transform.DOScale(Vector3.one * 0.75f, duration);
        hangeableObject.transform.DOLocalJump(Vector3.zero, 2f, 1, duration);
        hangeableObject.transform.DOLocalRotate(Vector3.zero, duration).OnComplete(() =>
        {
            hangeableObject.GetLevelText().enabled = true;
        });
        _currentHangerIndex++;
    }

    private IEnumerator CollectProcess()
    {
        //GunController.Instance.StopThrowingForAllGuns();
        
        for (var i = 0; i < _hangers.Count; i++)
        {
            if(_hangers[i].GetHanged() == null) continue;
                        
            Collect(_hangers[i].GetHanged());
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.1f);
        
        //GunController.Instance.StartThrowingForAllGuns();
    }
    private void Collect(HangeableObject hangeableObject)
    {
        var duration = 0.25f;
        hangeableObject.IsHanged = false;
        hangeableObject.transform.DOKill();
        
        // USE IT WHEN YOU WANT TO FIND LOWEST VALUE
        
        //Projectile lowestValueProjectile = null;
        //var lowestValue = int.MaxValue;
        // foreach (var projectile in GunController.Instance.GetGuns()[0].GetCurrentProjectiles())
        // {
        //     int timeValue = 0; //projectile.GetTimeValue(); // set here projectiles values
        //     if (timeValue < lowestValue)
        //     {
        //         lowestValue = timeValue;
        //         lowestValueProjectile = projectile;
        //     }
        // }
        
        var hObject = hangeableObject.GetProjectileStand().GetHangeableObject().transform;
        hangeableObject.GetProjectileStand().SetLevelTextVisibility(false);
        hObject.SetParent(BermudaRunnerCharacter.Instance.GetSimpleAnimancer().transform); 
        hObject.DOScale(Vector3.one * 0.25f, duration);
        hObject.DOLocalRotate(Vector3.zero, duration);
        hObject.DOLocalJump(new Vector3(0f, 0.75f,0f), 2f, 1, duration).OnComplete(
            () =>
            {
                hObject.gameObject.SetActive(false);
            });
    }

}