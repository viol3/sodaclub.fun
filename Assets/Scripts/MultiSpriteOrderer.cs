using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSpriteOrderer : MonoBehaviour
{
    [SerializeField] private MultiSpriteOrderData[] _datas;

    private int _currentOrder;
    public void SetOrder(int order)
    {
        for (int i = 0; i < _datas.Length; i++)
        {
            _datas[i].SetOrder(order);
        }
        _currentOrder = order;
    }

    public int GetOrder()
    {
        return _currentOrder;
    }
}

[System.Serializable]
public class MultiSpriteOrderData
{
    [SerializeField] private string _name;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private int _orderOffset;

    public void SetOrder(int order)
    {
        _renderer.sortingOrder = order + _orderOffset;
    }
}
