using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HangeableObjectCollector : MonoBehaviour
{
    [SerializeField] private HangeableObjectStand _hangeableObjectStand;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hangeableObjectStand.GetHangeableObject().IsCollected)
        {
            _hangeableObjectStand.Collect();
        }
    }
}
