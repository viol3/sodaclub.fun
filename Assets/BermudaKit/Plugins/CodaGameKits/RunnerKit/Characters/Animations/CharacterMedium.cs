using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMedium : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Dance = Animator.StringToHash("Dance");

    private void Start()
    {
        GameManager.OnGameplayStarted += PlayRunAnimation;
        GameManager.OnGameplayEnded += PlayDanceAnimation;
    }
    private void OnDestroy()
    {
        GameManager.OnGameplayStarted -= PlayRunAnimation;
        GameManager.OnGameplayEnded -= PlayDanceAnimation;
    }

    public void PlayRunAnimation()
    {
        _animator.SetTrigger(Run);    
    }
    public void PlayJumpAnimation()
    {
        _animator.SetTrigger(Jump);  
    }
    public void PlayDieAnimation()
    {
        _animator.SetTrigger(Die);  
    }
    public void PlayDanceAnimation()
    {
        _animator.SetTrigger(Dance);  
    }
}
