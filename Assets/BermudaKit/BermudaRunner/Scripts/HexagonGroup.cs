using TMPro;
using UnityEngine;

public class HexagonGroup : MonoBehaviour
{
    [SerializeField] private TextMeshPro _multiplierText;
    [SerializeField] private DestructibleHexagon[] _hexagons;
    
    public void SetMultiplier(int multiplier)
    {
        _multiplierText.text = multiplier.ToString() + "x";
    }

    public void SetHexagonHPs(int hp)
    {
        for (int i = 0; i < _hexagons.Length; i++)
        {
            _hexagons[i].SetHp(hp);
        }
    }

    public void ResetHexagons()
    {
        for (int i = 0; i < _hexagons.Length; i++)
        {
            _hexagons[i].ResetHexagon();
        }
    }
}
