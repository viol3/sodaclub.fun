using Ali.Helper;
using Ali.Helper.Audio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CardDeck : MonoBehaviour
{
    [SerializeField] private float _cardCollectDuration = 0.25f;
    [SerializeField] private float _shuffleDurationPerCard = 0.25f;
    [SerializeField] private float _shuffleOffsetPerCard = 3f;
    [SerializeField] private float _spreadDurationPerCard = 0.25f;
    [SerializeField] private Vector3 _pileOffset;
    [Space]
    [SerializeField] private List<CardObject> _cards;

    private bool _busy = false;

    public List<CardObject> GetCards()
    {
        return _cards;
    }

    public List<CardObject> GetClosedCards()
    {
        return _cards.Where(c => !c.IsOpened()).ToList();
    }

    public bool IsAnyCardBusy()
    {
        foreach (var card in _cards)
        {
            if(card.IsFlipping())
            {
                return true;
            }
        }
        return false;
    }

    public void SetAllCardsClickable(bool clickable)
    {
        foreach (var card in _cards) 
        {
            card.SetClickable(clickable);
        }
    }

    public void SetAllClosedCardsClickable(bool clickable)
    {
        foreach (var card in _cards)
        {
            card.SetClickable(!card.IsOpened());
        }
    }

    public void FlipAllClosedCards()
    {
        foreach (var card in _cards)
        {
            if(!card.IsOpened())
            {
                card.Flip();
            }
        }
    }

    public int GetClosedCardCount()
    {
        int count = 0;
        foreach (var card in _cards) 
        { 
            if(!card.IsOpened())
            {
                count++;
            }
        }
        return count;
    }

    public bool IsBusy()
    {
        return _busy; 
    }

    public void Intro(bool passShow = false)
    {
        if (_busy)
        {
            return;
        }
        _busy = true;
        StartCoroutine(IntroProcess(passShow));
    }

    public void CloseAllCards()
    {
        if (_busy)
        {
            return;
        }
        _busy = true;
        StartCoroutine(CloseAllCardsProcess());
    }

    IEnumerator CloseAllCardsProcess()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            if(_cards[i].IsOpened())
            {
                _cards[i].Flip();
                yield return new WaitForSeconds(0.05f);
            }
        }
        _busy = false;
    }

    IEnumerator IntroProcess(bool passShow = false)
    {
        if(!passShow)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].Flip();
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(2f);
        }
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].Flip();
            yield return new WaitForSeconds(0.05f);
        }
        _busy = false;
    }

    public void Spread(Vector3[] points)
    {
        if (_busy)
        {
            return;
        }
        _busy = true;
        StartCoroutine(SpreadProcess(points));
        AudioPool.Instance.PlayCardSpread();
    }

    IEnumerator SpreadProcess(Vector3[] points)
    {
        for (int i = points.Length - 1; i >= 0; i--)
        {
            _cards[i].transform.DOMove(points[i], _spreadDurationPerCard).SetEase(Ease.InOutSine);
        }
        yield return new WaitForSeconds(_spreadDurationPerCard);
        _busy = false;
    }

    public void Collect()
    {
        if(_busy)
        {
            return;
        }
        _busy = true;
        StartCoroutine(CollectProcess());
    }

    IEnumerator CollectProcess()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].SetOrder(i * 5);
            _cards[i].transform.DOMove(transform.position + (i * _pileOffset), _cardCollectDuration).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(0.05f);
        }
        _busy = false;
    }

    public void Shuffle()
    {
        if (_busy)
        {
            return;
        }
        _busy = true;
        StartCoroutine(ShuffleProcess());
    }

    IEnumerator ShuffleProcess()
    {
        List<int> indices = new List<int>();
        for (int j = 0; j < _cards.Count; j++)
        {
            indices.Add(j);
        }
        GameUtility.Shuffle(ref indices);
        for (int i = 0; i < 5; i++)
        {

            int part1 = _cards.Count / 2;

            for (int k = 0; k < part1; k++)
            {
                _cards[indices[k]].transform.DOMoveX(-_shuffleOffsetPerCard + (Random.Range(-0.5f, 0.5f)), _shuffleDurationPerCard).SetEase(Ease.InOutSine);
            }

            for (int k = part1; k < _cards.Count; k++)
            {
                _cards[indices[k]].transform.DOMoveX(_shuffleOffsetPerCard + (Random.Range(-0.5f, 0.5f)), _shuffleDurationPerCard).SetEase(Ease.InOutSine);
            }
            AudioPool.Instance.PlayRandomCardShuffle();
            yield return new WaitForSeconds(_shuffleDurationPerCard);
            for (int k = 0; k < _cards.Count; k++)
            {
                _cards[k].transform.DOMoveX(0f, _shuffleDurationPerCard).SetEase(Ease.InOutSine);
            }

            yield return new WaitForSeconds(_shuffleDurationPerCard);
        }
        _busy = false;
    }
}
