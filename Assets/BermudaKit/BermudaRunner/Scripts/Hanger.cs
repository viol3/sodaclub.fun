using UnityEngine;

public class Hanger : MonoBehaviour
{
    [SerializeField] private Transform _hangPoint;
    private HangeableObject _hangedProjectile;
    
    public void SetHanged(HangeableObject hangeableObject) => _hangedProjectile = hangeableObject;
    public HangeableObject GetHanged() => _hangedProjectile;
    public Transform GetHangPoint() => _hangPoint;
    
}