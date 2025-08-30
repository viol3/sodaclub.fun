using Ali.Helper;
using Ali.Helper.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DeathCardManager : LocalSingleton<DeathCardManager>
{
    [SerializeField] private Vector3[] _cardPositions;
    [Space]
    [SerializeField] private CurrencyPanel _currencyPanel;
    [SerializeField] private SmoothNumberText _betText;
    [SerializeField] private SmoothNumberText _multiplierText;
    [SerializeField] private CardDeck _deck;
    [SerializeField] private Button _newHandButton;

    private float _balance = 10f;
    private float _bet = 1f;
    private float _initialBet = 1f;
    private float _betFactor = 1f;

    private List<int> _normalCardIndices = new List<int>();
    // Start is called before the first frame update
    IEnumerator Start()
    {
        
        Init();
        _deck.SetAllCardsClickable(false);
        yield return new WaitForSeconds(0.5f);
        _deck.Intro();
        yield return new WaitWhile(() => _deck.IsBusy());
        yield return new WaitForSeconds(0.5f);
        _deck.Collect();
        yield return new WaitWhile(() => _deck.IsBusy());
        yield return new WaitForSeconds(0.5f);
        _deck.Shuffle();
        yield return new WaitWhile(() => _deck.IsBusy());
        yield return new WaitForSeconds(0.5f);
        _deck.Spread(_cardPositions);
        _deck.SetAllCardsClickable(true);
    }

    IEnumerator NewHandProcess()
    {
        ResetSystem();
        _deck.SetAllCardsClickable(false);
        yield return new WaitForSeconds(0.5f);
        _deck.CloseAllCards();
        yield return new WaitWhile(() => _deck.IsBusy());
        yield return new WaitForSeconds(0.5f);
        _deck.Collect();
        yield return new WaitWhile(() => _deck.IsBusy());
        yield return new WaitForSeconds(0.5f);
        _deck.Shuffle();
        yield return new WaitWhile(() => _deck.IsBusy());
        yield return new WaitForSeconds(0.5f);
        _deck.Spread(_cardPositions);
        _deck.SetAllCardsClickable(true);
    }

    void UpdateBalanceUI()
    {
        _currencyPanel.SetCurrencyAmount(_balance);
    }

    void UpdateBetText()
    {
        _betText.SetPoints(_bet);
    }

    void UpdateMultiplierText()
    {
        _multiplierText.SetPoints(_betFactor);
    }
    float GetCurrentFactor()
    {
        int closedCardCount = _deck.GetClosedCardCount();
        float factor = 0.45f;
        if (closedCardCount == 3)
        {
            factor = 0.3f;
        }
        else if (closedCardCount == 4)
        {
            factor = 0.2f;
        }
        else if (closedCardCount == 5)
        {
            factor = 0.18f;
        }
        else if (closedCardCount == 6)
        {
            factor = 0.15f;
        }
        else if (closedCardCount == 7)
        {
            factor = 0.12f;
        }
        else if (closedCardCount == 8)
        {
            factor = 0.1f;
        }
;
        return factor;
    }

    void Init()
    {
        _bet = _initialBet;
        _normalCardIndices.Clear();
        for (int i = 0; i < 7; i++)
        {
            _normalCardIndices.Add(i);
        }
        GameUtility.Shuffle(ref _normalCardIndices);

        List<CardObject> cards = _deck.GetCards();
        foreach (var item in cards)
        {
            item.OnClick += OnCardClick;
        }

        UpdateBalanceUI();
        UpdateMultiplierText();
        UpdateBetText();
    }

    void ResetSystem()
    {
        _bet = _initialBet;
        _betFactor = 1f;
        _normalCardIndices.Clear();
        for (int i = 0; i < 7; i++)
        {
            _normalCardIndices.Add(i);
        }
        GameUtility.Shuffle(ref _normalCardIndices);
    }

    int GetNextNormalCardIndex()
    {
        int result = _normalCardIndices[0];
        _normalCardIndices.RemoveAt(0);
        return result;
    }

    Sprite GetNextNormalCardSprite()
    {
        int spriteIndex = GetNextNormalCardIndex();
        return SpriteReferencer.Instance.GetNormalCardSpriteByIndex(spriteIndex);
    }

    void OnCardClick(CardObject card)
    {
        Debug.Log("Clicked card => " + card);
        _newHandButton.gameObject.SetActive(true);
        int closedCardCount = _deck.GetClosedCardCount();
        int randomIndex = Random.Range(0, closedCardCount);
        _balance -= _bet;
        if (randomIndex == 0)
        {
            card.SetCardSprite(SpriteReferencer.Instance.GetDeathCardSprite());
            _bet = 0;
            _betFactor = 0;
        }
        else
        {
            card.SetCardSprite(GetNextNormalCardSprite());
            float factor = GetCurrentFactor();
            _bet += _bet * factor;
            _betFactor += _betFactor * factor;
            _balance += _bet;
        }
        UpdateBalanceUI();
        UpdateMultiplierText();
        UpdateBetText();
        card.Flip();
    }

    public void OnNewHandButtonClick()
    {
        StartCoroutine(NewHandProcess());
        UpdateBalanceUI();
        UpdateMultiplierText();
        UpdateBetText();
        _newHandButton.gameObject.SetActive(false);
    }

}
