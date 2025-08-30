using Ali.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PreWinPanel : LocalSingleton<PreWinPanel>
{
    [SerializeField] private RectTransform _levelCompletePanel;
    [SerializeField] private Image _faderImage;
    [SerializeField] private ParticleSystem _confettiParticle;
    [Space]
    [SerializeField] private float _fadeAmount = 0.3f;

    private bool _showing = false;

    public void Show(float duration)
    {
        if(_showing)
        {
            return;
        }
        _showing = true;
        StartCoroutine(ShowProcess(duration));
    }

    IEnumerator ShowProcess(float duration)
    {
        _faderImage.DOFade(_fadeAmount, 0.3f);
        _levelCompletePanel.anchoredPosition = new Vector2(0f, -1200f);
        yield return _levelCompletePanel.DOAnchorPosY(0, duration * 0.15f).WaitForCompletion();
        _confettiParticle.Play();
        yield return _levelCompletePanel.DOPunchScale(Vector3.one * 0.3f, duration * 0.1f, 6).WaitForCompletion();
        yield return new WaitForSeconds(duration * 0.55f);
        _faderImage.DOFade(0f, 0.3f);
        //_levelCompletePanel.DOPunchScale(Vector3.one * 0.3f, duration * 0.2f, 6);
        yield return _levelCompletePanel.DOAnchorPosY(1200f, duration * 0.2f).SetEase(Ease.InQuad).WaitForCompletion();
        _faderImage.DOKill();
        GameUtility.ChangeAlphaImage(_faderImage, 0f);
        _showing = false;
    }
}
