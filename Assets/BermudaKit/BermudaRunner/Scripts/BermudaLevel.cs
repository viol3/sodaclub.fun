using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BermudaLevel : MonoBehaviour
{
    [SerializeField] private PathCreator _pathCreator;

    public PathCreator GetPathCreator()
    {
        return _pathCreator;
    }
}
