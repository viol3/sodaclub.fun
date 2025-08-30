using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class HangeableObject : MonoBehaviour
{
    [SerializeField] private HangeableObjectStand hangeableObjectStand;
    [SerializeField] private TextMeshPro _levelText;
    
    private int _level = 0;
    public bool IsCollected { get; set; } = false;
    public bool IsHanged { get; set; } = false;

    private void Start()
    {
        _levelText.enabled = false;
    }

    public int GetLevel() => _level;
    public void SetLevel(int value)
    {
        _level = value;
        _levelText.text = _level.ToString();
    } 
    public HangeableObjectStand GetProjectileStand() => hangeableObjectStand;
    public TextMeshPro GetLevelText() => _levelText;
}
