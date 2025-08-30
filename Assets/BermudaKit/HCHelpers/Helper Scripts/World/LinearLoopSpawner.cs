using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class LinearLoopSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _spawnObject;
        [SerializeField] private Transform _parent;
        [SerializeField] private Vector3 _interval;
        [SerializeField] private int _count = 30;
        // Start is called before the first frame update
        void Awake()
        {
            for (int i = 0; i < _count; i++)
            {
                GameObject spawnGO = Instantiate(_spawnObject, _parent);
                spawnGO.transform.localPosition = _spawnObject.transform.position + _interval * (i + 1);
            }
        }


    }
}