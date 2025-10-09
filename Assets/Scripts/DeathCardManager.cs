using Ali.Helper;
using Ali.Helper.Audio;
using Ali.Helper.UI;
using Ali.Helper.World;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static System.Net.Mime.MediaTypeNames;

public class DeathCardManager : LocalSingleton<DeathCardManager>
{
    [SerializeField] private Vector3[] _cardPositions;
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
    [SerializeField] private TMP_InputField _privateKeyInput;
    [SerializeField] private TMP_InputField _publicKeyInput;
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
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        SuiManager.Instance.OnDeathCardEnded.AddListener(OnDeathCardEnded);
        SuiManager.Instance.OnBalanceUpdated.AddListener(OnBalanceUpdated);
        SuiManager.Instance.OnBalanceChanged.AddListener(OnBalanceChanged);
        SuiManager.Instance.OnAccountInfoReceived.AddListener(OnAccountInfoReceived);
    }

    private void OnDestroy()
    {
        SuiManager.Instance?.OnDeathCardEnded.RemoveListener(OnDeathCardEnded);
        SuiManager.Instance?.OnBalanceUpdated.RemoveListener(OnBalanceUpdated);
        SuiManager.Instance?.OnBalanceChanged.RemoveListener(OnBalanceChanged);
        SuiManager.Instance?.OnAccountInfoReceived.RemoveListener(OnAccountInfoReceived);
    }

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
    }

    void OnAccountInfoReceived(string privateKey, string publicKey)
    {
        _privateKeyInput.text = privateKey;
        _publicKeyInput.text = publicKey;
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
        SuiManager.Instance.DeathCard(_realBet, (byte)closedCardCount);
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
        SuiManager.OpenURL("https://faucet.sui.io");
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
        yield return SuiManager.Instance.RefreshProcess();
        _balanceRefreshRotator.enabled = false;
        _refreshingBalance = false;
    }

    public void OnResetAccountButtonClick()
    {
        SuiManager.Instance.ResetAccount();
    }

    public void OnCopyClipboardPublicKeyClicked()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLCopyAndPaste.WebGLCopyAndPasteAPI.CopyToClipboard(_publicKeyInput.text);
#else
        GUIUtility.systemCopyBuffer = _publicKeyInput.text;
#endif
    }

    public void OnCopyClipboardPrivateKeyClicked()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLCopyAndPaste.WebGLCopyAndPasteAPI.CopyToClipboard(_privateKeyInput.text);
#else
        GUIUtility.systemCopyBuffer = _privateKeyInput.text;
#endif
        
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
