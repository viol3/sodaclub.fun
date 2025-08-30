using Ali.Helper;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : LocalSingleton<UIManager>
{
    [Space]
    [Header("Idle Buttons")]
    [SerializeField] private IdleButton[] _startIdleButtons;
    [SerializeField] private IdleButton[] _endIdleButtons;
    [SerializeField] private Transform _tapToStartButton;
    [SerializeField] private GameObject _handCursor;
    [Space]  
    [Space][Header("Finish UI")]
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private TextMeshProUGUI _currentLevelText;
    
    [Space]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _cashIconTransform;

    private Tweener _punchImageTweener;
    private Tweener _smoothTween;
    private Tweener _rotationTweener;
    private float _rotationValue = 5f;
    public void Init()
    {
        for (int i = 0; i < _startIdleButtons.Length; i++)
        {
            _startIdleButtons[i].Init();
        }
        for (int i = 0; i < _endIdleButtons.Length; i++)
        {
            _endIdleButtons[i].Init();
        }
        UpdateAllUI_IdleButtons();
        _currentLevelText.text = "Level " + (HCLevelManager.Instance.GetGlobalLevelIndex() + 1);
        _tapToStartButton.gameObject.SetActive(true);
        _handCursor.SetActive(HCLevelManager.Instance.GetGlobalLevelIndex() == 0);
        SetEndIdleButtons(false);
        SetStartIdleButtons(true);
        ShowLosePanel(false);
        ShowWinPanel(false);
    }

    public void UpdateAllUI_IdleButtons()
    {
        for (int i = 0; i < _startIdleButtons.Length; i++)
        {
            _startIdleButtons[i].UpdateUI();
        }
        for (int i = 0; i < _endIdleButtons.Length; i++)
        {
            _endIdleButtons[i].UpdateUI();
        }
    }

    public void ShowWinPanel(bool show)
    {
        _winPanel.SetActive(show);
        SetEndIdleButtons(show);
    }

    public void ShowLosePanel(bool show)
    {
        _losePanel.SetActive(show);
        SetEndIdleButtons(show);
    }

    public void SetStartIdleButtons(bool show)
    {
        for (int i = 0; i < _startIdleButtons.Length; i++)
        {
            _startIdleButtons[i].gameObject.SetActive(show);
        }
    }
    public void SetEndIdleButtons(bool show)
    {
        for (int i = 0; i < _endIdleButtons.Length; i++)
        {
            _endIdleButtons[i].gameObject.SetActive(show);
        }
    }
    
    public void OnTestNextButtonClick()
    {
        HCLevelManager.Instance.LevelUp();
        SceneManager.LoadScene("Main");
    }
    public void OnNextButtonClick()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnTryAgainButtonClick()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnTapToPlayClick()
    {
        _tapToStartButton.gameObject.SetActive(false);
        for (int i = 0; i < _startIdleButtons.Length; i++)
        {
            _startIdleButtons[i].gameObject.SetActive(false);
        }
        
        GameManager.Instance.StartGameplay();
    }
    public void PunchImage(Transform image ,float punchScale, float duration)
    {
        _punchImageTweener?.Complete();

        _punchImageTweener = image.DOPunchScale(image.localScale * punchScale, duration);
    }
    public void SpawnCashPopup(Vector3 spawnPosition, float cashValue, float duration = 0.4f)
    {
        GameObject cashPopup = null;
        Transform target = null;
        Vector3 targetWorldPosition = new Vector3();

        cashPopup = PoolManager.Instance.SpawnCashPopup();
        target = _cashIconTransform;
        targetWorldPosition = target.position;
        
        cashPopup.transform.SetParent(_canvas.transform, false);
        var initialScale = cashPopup.transform.localScale;
        cashPopup.transform.localScale *= 0.5f;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(spawnPosition);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.GetComponent<RectTransform>(), screenPosition, Camera.main, out Vector2 anchoredPosition);
        cashPopup.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        Vector2 targetScreenPosition = Camera.main.WorldToScreenPoint(targetWorldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.GetComponent<RectTransform>(), targetScreenPosition, Camera.main, out Vector2 targetAnchoredPosition);

        cashPopup.transform.DOScale(initialScale, duration);
        cashPopup.GetComponent<RectTransform>().DOAnchorPos(targetAnchoredPosition, duration).SetEase(Ease.OutSine).OnComplete(() => {
            
            CurrencyManager.Instance.DealCurrency(cashValue, true);
            PoolManager.Instance.DespawnCashPopup(cashPopup);
            
            PunchImage(target ,0.15f,0.2f);
        });
    }
    
    public void CashPopupCanvasToWorld(Transform target, float cashValue)
    {
        GameObject cashPopup = null;
        Transform source = null;
        Vector3 sourceWorldPosition = new Vector3();
        
        cashPopup = PoolManager.Instance.SpawnCashPopup();
        source = _cashIconTransform;
        sourceWorldPosition = source.position;

        cashPopup.transform.SetParent(_canvas.transform, false);
        cashPopup.transform.localScale = cashPopup.transform.localScale;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(sourceWorldPosition);
    
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.GetComponent<RectTransform>(), screenPosition, Camera.main, out Vector2 anchoredPosition);
        cashPopup.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        
        const float moveDuration = 0.4f;

        Vector2 targetScreenPosition = Camera.main.WorldToScreenPoint(target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.GetComponent<RectTransform>(), targetScreenPosition, Camera.main, out Vector2 targetAnchoredPosition);

        cashPopup.transform.DOScale(cashPopup.transform.localScale * 0.5f, moveDuration);
        cashPopup.GetComponent<RectTransform>().DOAnchorPos(targetAnchoredPosition, moveDuration).SetEase(Ease.OutSine).OnComplete(() => {

            CurrencyManager.Instance.DealCurrency(cashValue, false);
            PoolManager.Instance.DespawnCashPopup(cashPopup);
        });
    }
    public Canvas GetCanvas() => _canvas;
}
