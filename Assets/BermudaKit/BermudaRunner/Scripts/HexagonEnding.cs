using System;
using Ali.Helper;
using UnityEngine;

public class HexagonEnding : LocalSingleton<HexagonEnding>
{
    [SerializeField] private int _groupCount = 10;
    [SerializeField] private float _groupOffset = 1;
    [SerializeField] private int _floorCount = 20;
    [SerializeField] private float _floorOffset = 10;
    [SerializeField] private int _startHp = 6;
    [SerializeField] private float _hpMultiplier = 1.5f;
    [Space]
    [SerializeField] private GameObject _hexagonGroupPrefab;
    [SerializeField] private GameObject _floorPrefab;
    [SerializeField] private Transform _groupParent;
    [SerializeField] private Transform _endingHouse;

    private HexagonGroup[] _groups;

    private void Start()
    {
        _groups = null;
    }

    public void Init()
    {
        if(_groups == null)
        {
            _groups = new HexagonGroup[_groupCount];
            float z = 0;
            for (int i = 0; i < _groups.Length; i++)
            {
                _groups[i] = Instantiate(_hexagonGroupPrefab, _groupParent).GetComponent<HexagonGroup>();
                _groups[i].transform.localPosition = new Vector3(0f, 0f, z);
                z += _groupOffset;
            }

            for (int i = 0; i < _floorCount; i++)
            {
                Transform floor = Instantiate(_floorPrefab, _floorPrefab.transform.parent).transform;
                floor.transform.localPosition = new Vector3(0, 0, (i + 1) * _floorOffset);
                if(i == _floorCount - 1)
                {
                    _endingHouse.localPosition = floor.localPosition + (Vector3.forward * 9f);
                }
            }
        }

        for (int i = 0; i < _groups.Length; i++)
        {
            int hp = (int)(_startHp + (_startHp * _hpMultiplier * i));
            _groups[i].SetMultiplier(i + 1);
            _groups[i].SetHexagonHPs(hp);
            _groups[i].ResetHexagons();
        }
    }
}