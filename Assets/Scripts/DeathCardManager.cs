using Ali.Helper;
using Ali.Helper.Audio;
using Ali.Helper.UI;
using Ali.Helper.World;
using Cysharp.Threading.Tasks.Triggers;
using Soda.Sui;
using Sui.Rpc;
using Sui.Rpc.Models;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using viol3.SuiWorks.Accounts;
using static System.Net.Mime.MediaTypeNames;

public class DeathCardManager : LocalSingleton<DeathCardManager>
{
    [SerializeField] private UnityEngine.Vector3[] _cardPositions;
    [Space]
    [SerializeField] private ScaleAnimationLoop _faucetAnimation;
    [SerializeField] private Rotator _balanceRefreshRotator;
    [SerializeField] private RectTransform _betPanel;
    [SerializeField] private RectTransform _tutorialPanel;
    [SerializeField] private CurrencyPanel _currencyPanel;
    [SerializeField] private SmoothNumberText _betText;
    [SerializeField] private SmoothNumberText _multiplierText;
    [SerializeField] private CardDeck _deck;
    [SerializeField] private Button _newHandButton;
    [SerializeField] private Button _cashOutButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private DeathCardBetButton[] _betButtons;
    private float _balance = 0f;
    private float _realBet = 0.25f;
    private float _bet = 0.25f;
    private float _initialBet = 0.25f;
    private float _betFactor = 1f;

    private bool _refreshingBalance = false;

    private bool _over = false;
    private bool _globalCardInput = true;
    private CardObject _clickedCard = null;

    private List<int> _normalCardIndices = new List<int>();

    private SodaTransactionKit _transactionKit;

    [DllImport("__Internal")]
    public static extern void OpenURL(string url);

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
        float factor = (1f / (1f - (1f / closedCardCount))) - 1;
        factor -= 0.03f;
        return factor;
    }

    void Init()
    {
        _bet = _initialBet;
        _realBet = _bet;
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
        _betButtons[0].OnClick();
        _transactionKit = new SodaTransactionKit();
    }

    void ResetSystem()
    {
        if(_over)
        {
            _bet = _initialBet;
            _realBet = _bet;
            _betFactor = 1f;
            _over = false;
        }
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

    void OnBalanceUpdated(float balance)
    {
        _balance = balance;
        UpdateBalanceUI();
    }

    void OnBalanceChanged(float addAmount, bool updateUI)
    {
        _balance += addAmount;
        _realBet += addAmount;
        if(updateUI)
        {
            UpdateBalanceUI();
        }
    }

    void OnDeathCardEnded(int diceValue)
    {
        AudioPool.Instance.StopClipByName("shakebottle");
        if (diceValue == 1)
        {
            _clickedCard.Flip();
            List<CardObject> cards = _deck.GetClosedCards();
            foreach (var card in cards)
            {
                card.SetCardSprite(GetNextNormalCardSprite());
            }
            _deck.FlipAllClosedCards();
            _deck.SetAllCardsClickable(false);
            _clickedCard.SetLoading(false);
            _clickedCard.SetCardSprite(SpriteReferencer.Instance.GetDeathCardSprite());
            _bet = 0;
            _betFactor = 0;
            _over = true;
            _newHandButton.gameObject.SetActive(false);
            _cashOutButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            AudioPool.Instance.PlayFail();
        }
        else if(diceValue == -1)
        {
            _clickedCard.SetLoading(false);
            _deck.SetAllClosedCardsClickable(true);
            MessageBox.Instance.Show("Blockchain returned an error. Reasons may :\r\n- Low Balance On House\r\n- Low Balance On User\r\n- Internal Code Error");
            return;
        }
        else
        {
            float factor = GetCurrentFactor();
            _deck.SetAllClosedCardsClickable(true);
            _clickedCard.Flip();
            _clickedCard.SetClickable(false);
            _clickedCard.SetLoading(false);
            _clickedCard.SetCardSprite(GetNextNormalCardSprite());
            
            _bet += _bet * factor;
            _betFactor += _betFactor * factor;
            _cashOutButton.gameObject.SetActive(true);
            _newHandButton.gameObject.SetActive(true);
            AudioPool.Instance.PlaySodaOpen();
            
        }
        UpdateBalanceUI();
        UpdateMultiplierText();
        UpdateBetText();
        
    }

    void OnCardClick(CardObject card)
    {
        if(!_globalCardInput)
        {
            return;
        }
        if(_realBet + 0.01f > _balance)
        {
            MessageBox.Instance.Show("Your balance is insufficient for the minimum transaction.");
            return;
        }
        int closedCardCount = _deck.GetClosedCardCount();
        DeathCard(_realBet, (byte)closedCardCount);
        _clickedCard = card;
        card.SetLoading(true);
        _deck.SetAllCardsClickable(false);
        AudioPool.Instance.PlayClipByName("shakebottle", false, 0.5f);
        if(_betPanel.gameObject.activeInHierarchy)
        {
            _betPanel.gameObject.SetActive(false);
            _tutorialPanel.gameObject.SetActive(false);
        }
    }

    public async void DeathCard(float betAmount, byte cardCount)
    {
        int diceValue = await StartDeathCard((decimal)betAmount, cardCount);
        OnDeathCardEnded(diceValue);
    }

    async Task<string> Commit(decimal amount, byte cardCount)
    {

        RpcResult<TransactionBlockResponse> result_task = await SuiAccountManager.Instance.SignAndExecuteTransactionBlockAsync(_transactionKit.Commit(amount, cardCount));


        if (result_task.Error != null)
        {
            Debug.Log("PlayDeathCard Error => " + result_task.Error.Message);
            return null;
        }

        if (result_task.Result != null && result_task.Result.BalanceChanges != null && result_task.Result.BalanceChanges.Length > 0)
        {
            BigInteger changeAmountBig = result_task.Result.BalanceChanges[0].Amount;
            float changeAmount = SuiAccountManager.GetFloatFromBigInteger(changeAmountBig);
            OnBalanceChanged(SuiAccountManager.GetFloatFromBigInteger(result_task.Result.BalanceChanges[0].Amount), false);
        }

        if (result_task.Result != null && result_task.Result.Effects != null && result_task.Result.Effects.Created.Length > 0)
        {
            SuiTransactionBlockEffects effects = result_task.Result.Effects;
            return effects.Created[0].Reference.ObjectID.ToHex();
        }

        return null;

    }

    async Task<int> Reveal(string ownedCommit)
    {
        RpcResult<TransactionBlockResponse> result_task = await SuiAccountManager.Instance.SignAndExecuteTransactionBlockAsync(_transactionKit.Reveal(ownedCommit));

        if (result_task.Error != null)
        {
            Debug.Log("Reveal Error => " + result_task.Error.Code + " => " + result_task.Error.Message);
            Debug.Log(result_task.Error.Data);
            return -1;
        }

        if (result_task.Result != null && result_task.Result.BalanceChanges != null && result_task.Result.BalanceChanges.Length > 0)
        {
            BigInteger changeAmountBig = result_task.Result.BalanceChanges[0].Amount;
            float changeAmount = SuiAccountManager.GetFloatFromBigInteger(changeAmountBig);
            OnBalanceChanged(SuiAccountManager.GetFloatFromBigInteger(result_task.Result.BalanceChanges[0].Amount), true);
        }

        try
        {
            foreach (var m_event in result_task.Result.Events)
            {
                if (m_event.Type.Contains("DiceValue"))
                {
                    string value = m_event.ParsedJson.GetValue("value").ToString();
                    return int.Parse(value);
                }
            }
            return -1;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            Debug.LogError(ex.StackTrace);
            return -1;
        }


    }

    async Task<int> StartDeathCard(decimal amount, byte cardCount)
    {
        string ownedCommit = await Commit(amount, cardCount);
        if (!string.IsNullOrEmpty(ownedCommit))
        {
            int diceResult = await Reveal(ownedCommit);
            return diceResult;
        }
        else
        {
            Debug.Log("ownedCommit is null");
            return -1;
        }
    }

    public bool IsGlobalCardInputEnabled()
    {
        return _globalCardInput;
    }

    public void OnSettingsOpened()
    {
        _globalCardInput = false;
    }

    public void OnSettingsClosed()
    {
        _globalCardInput = true;
    }

    public void OnFaucetButtonClick()
    {
        OpenURL("https://faucet.sui.io");
    }

    public void OnRefreshButtonClick()
    {
        if(_refreshingBalance)
        {
            return;
        }
        _refreshingBalance = true;
        StartCoroutine(RefreshProcess());
    }

    IEnumerator RefreshProcess()
    {
        _balanceRefreshRotator.enabled = true;
        Task<float> balanceTask = SuiAccountManager.Instance.GetSuiBalance();
        yield return new WaitUntil(() => balanceTask.IsFaulted || balanceTask.IsCompleted);
        OnBalanceUpdated(balanceTask.Result);
        _balanceRefreshRotator.enabled = false;
        _refreshingBalance = false;
        yield return null;
    }


    public void OnRestartButtonClick()
    {
        OnCashOutButtonClick();
        _restartButton.gameObject.SetActive(false);
    }

    public void OnNewHandButtonClick()
    {
        StartCoroutine(NewHandProcess());
        UpdateBalanceUI();
        UpdateMultiplierText();
        UpdateBetText();
        _newHandButton.gameObject.SetActive(false);
        _cashOutButton.gameObject.SetActive(false);
    }

    public void OnCashOutButtonClick()
    {
        _over = true;
        ResetSystem();
        UpdateBalanceUI();
        UpdateMultiplierText();
        UpdateBetText();
        _newHandButton.gameObject.SetActive(false);
        _cashOutButton.gameObject.SetActive(false);
        _betPanel.gameObject.SetActive(true);
        _tutorialPanel.gameObject.SetActive(true);
        StartCoroutine(NewHandProcess());
    }

    public void OnBetButtonClick(DeathCardBetButton betButton)
    {
        _bet = betButton.GetValue();
        _realBet = betButton.GetValue();
        _initialBet = betButton.GetValue();
        for (int i = 0; i < _betButtons.Length; i++)
        {
            _betButtons[i].UnSelect();
        }
        betButton.Select();
        UpdateBetText();
    }

}
