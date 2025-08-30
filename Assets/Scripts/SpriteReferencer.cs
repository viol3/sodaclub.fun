using Ali.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteReferencer : GenericSingleton<SpriteReferencer>
{
    [SerializeField] private Sprite[] _normalCardSprites;
    [SerializeField] private Sprite _deathCardSprite;

    public Sprite GetNormalCardSpriteByIndex(int index)
    {
        return _normalCardSprites[index];
    }

    public Sprite GetDeathCardSprite() 
    { 
        return _deathCardSprite;
    }
}
