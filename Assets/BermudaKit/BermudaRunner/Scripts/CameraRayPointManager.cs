using Ali.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRayPointManager : LocalSingleton<CameraRayPointManager>
{
    private CameraRayPoint[] _rayPoints;

    private void Start()
    {
        _rayPoints = GetComponentsInChildren<CameraRayPoint>();
    }

    public Vector3 GetNearest(Vector3 source)
    { 
        return GameUtility.GetNearest<CameraRayPoint>(source, _rayPoints).transform.position;
    }

    
}
