using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFreezer : MonoBehaviour
{
    [SerializeField] private Vector3 _targetRotation;
    [SerializeField] private bool _worldSpace = false;

    private void Update()
    {
        if(_worldSpace)
        {
            transform.eulerAngles = _targetRotation;
        }
        else
        {
            transform.localEulerAngles = _targetRotation;
        }
        
    }

}
