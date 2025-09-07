using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCardBetButton : MonoBehaviour
{
    [SerializeField] private float _value;
    [Space]
    [SerializeField] private Image _back;
    
    public void OnClick()
    {
        DeathCardManager.Instance.OnBetButtonClick(this);
    }

    public float GetValue()
    { 
        return _value; 
    }

    public void Select()
    {
        _back.sprite = SpriteReferencer.Instance.GetBetButtonSelectedSprite();
    }

    public void UnSelect()
    {
        _back.sprite = SpriteReferencer.Instance.GetBetButtonNormalSprite();
    }
}
