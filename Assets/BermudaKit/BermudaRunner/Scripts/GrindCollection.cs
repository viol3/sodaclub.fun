using Ali.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindCollection : MonoBehaviour
{
    private GrindObject[] _grindObjects;

    private int _objectIndex = 0;
    void Start()
    {
        _grindObjects = GetComponentsInChildren<GrindObject>();
        GameUtility.Shuffle(ref _grindObjects);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G) && _objectIndex < _grindObjects.Length)
        {
            _grindObjects[_objectIndex++].Explode();
        }
    }
}
