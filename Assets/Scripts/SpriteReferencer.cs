using Ali.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteReferencer : GenericSingleton<SpriteReferencer>
{
    [SerializeField] private Sprite[] _normalCardSprites;
    [SerializeField] private Sprite _deathCardSprite;
    [SerializeField] private Sprite _betButtonNormalSprite;
    [SerializeField] private Sprite _betButtonSelectedSprite;
    public Sprite GetNormalCardSpriteByIndex(int index)
    {
        return _normalCardSprites[index];
    }

    public Sprite GetDeathCardSprite() 
    { 
        return _deathCardSprite;
    }

    public Sprite GetBetButtonNormalSprite()
    {
        return _betButtonNormalSprite;
    }

    public Sprite GetBetButtonSelectedSprite()
    {
        return _betButtonSelectedSprite;
    }
}
