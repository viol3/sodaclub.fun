using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class CameraFOVAspectRatio : MonoBehaviour
    {
        [Tooltip("If screen ratio lower than first ratio in the array, camera uses first FOV in the array.")]

        [SerializeField] private Camera _camera;
        [SerializeField] private float[] _fovs;
        [SerializeField] private float[] _ratios;


        private void Awake()
        {
            if (_ratios.Length != _fovs.Length || _fovs.Length == 0)
            {
                return;
            }
            for (int i = 0; i < _ratios.Length; i++)
            {
                float screenRatio = (float)Screen.width / Screen.height;
                if (screenRatio < _ratios[i])
                {
                    _camera.fieldOfView = _fovs[i];
                    return;
                }
            }
            _camera.fieldOfView = _fovs[_fovs.Length - 1];
        }
    }
}