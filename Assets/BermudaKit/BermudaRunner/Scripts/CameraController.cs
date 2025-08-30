using DG.Tweening;
using System.Collections.Generic;
using Ali.Helper;
using Bermuda.Runner;
using UnityEngine;

public enum CameraPov { Idle, Gameplay, EndGame, MiniGame }

public class CameraController : LocalSingleton<CameraController>
{
    [System.Serializable]
    public class Pov
    {
        public CameraPov cameraPov;
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;
        public float duration;
    }

    [Space]
    [Header("CAMERA SETTINGS")]
    [SerializeField] private Camera _camera;
    [SerializeField] private List<Pov> povs;
    [SerializeField] private CameraPov _currentPov;
    private Dictionary<CameraPov, Pov> _cameraPovDictionary;

    private void Start()
    {
        _cameraPovDictionary = new Dictionary<CameraPov, Pov>();
        foreach (var pov in povs)
        {
            _cameraPovDictionary[pov.cameraPov] = pov;
        }

        SetPov(CameraPov.Idle);
    }

    public void SetPov(CameraPov newCameraPov)
    {
        Pov targetPov = _cameraPovDictionary[newCameraPov];
        
        _camera.transform.DOLocalMove(targetPov.cameraPosition, targetPov.duration).SetEase(Ease.OutSine);
        _camera.transform.DOLocalRotate(targetPov.cameraRotation, targetPov.duration).SetEase(Ease.OutSine);
        _currentPov = newCameraPov;
    }
    
    public CameraPov GetCurrentPov() => _currentPov;
}