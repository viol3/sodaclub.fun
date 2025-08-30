using System;
using Ali.Helper;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType { Damage, FireRate, FireRange, Income}
public class IdleButton : MonoBehaviour
{
    [SerializeField] private UpgradeType _upgradeType;
    [Space]
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private string _preText = "";
    [SerializeField] private string _postText = "";
    [Space]
    //[SerializeField] private float _startPrice;
    [SerializeField] private int _startValue;
    [SerializeField] private int _addValue;
    //[SerializeField] private float _startIncrementalPrice;
    //[SerializeField] private float _addIncrementalPrice;
    [Space]
    [SerializeField] private string _pricePrefName;
    [SerializeField] private string _valuePrefName;
    [SerializeField] private string _incrementalPricePrefName;
    [Space]
    [SerializeField] private Image[] _bottomBacks;
    [SerializeField] private Color _negativeColor;
    private Color[] _positiveColors;

    private float _currentPrice = 0;
    private int _currentValue = 0;
    private float _currentIncrementalPrice = 0;
    private GameConfig _gameConfig;

    public event System.Action<bool> OnClick;

    public void Init()
    {
        _gameConfig = GameManager.Instance.GameConfig();
        LoadColors();
        Load();
        UpdateUI();
    }

    public void UpdateUI()
    {
        _priceText.text = GameUtility.FormatFloatToReadableString(_currentPrice);
        if (_valueText)
        {
            _valueText.text = _preText + (_currentValue).ToString() + _postText;
        }
        if (CurrencyManager.Instance.GetCurrency() >= _currentPrice)
        {
            for (int i = 0; i < _bottomBacks.Length; i++)
            {
                _bottomBacks[i].color = _positiveColors[i];
            }
        
        }
        else
        {
            for (int i = 0; i < _bottomBacks.Length; i++)
            {
                _bottomBacks[i].color = _negativeColor - new Color(0.1f, 0.1f, 0.1f, 0.1f) * i;
            }
        }
    }

    void LoadColors()
    {
        _positiveColors = new Color[_bottomBacks.Length];
        for (int i = 0; i < _positiveColors.Length; i++)
        {
            _positiveColors[i] = _bottomBacks[i].color;
        }
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey(_incrementalPricePrefName))
        {
            _currentIncrementalPrice = _upgradeType switch
            {
                UpgradeType.Damage => _gameConfig.DamageStartIncrementalPrice,
                UpgradeType.FireRate => _gameConfig.FireRateStartIncrementalPrice,
                UpgradeType.FireRange => _gameConfig.FireRangeStartIncrementalPrice,
                UpgradeType.Income => _gameConfig.IncomeStartIncrementalPrice,
                _ => throw new ArgumentOutOfRangeException()
            };
            _currentPrice = _upgradeType switch
            {
                UpgradeType.Damage => _gameConfig.DamageStartPrice,
                UpgradeType.FireRate => _gameConfig.FireRateStartPrice,
                UpgradeType.FireRange => _gameConfig.FireRangeStartPrice,
                UpgradeType.Income => _gameConfig.IncomeStartPrice,
                _ => throw new ArgumentOutOfRangeException()
            };
        
            _currentValue = _startValue;
            Save();
        }
        else
        {
            _currentPrice = PlayerPrefs.GetFloat(_pricePrefName);
            _currentIncrementalPrice = PlayerPrefs.GetFloat(_incrementalPricePrefName);
            _currentValue = PlayerPrefs.GetInt(_valuePrefName);
        }

    }

    private void Save()
    {
        PlayerPrefs.SetFloat(_incrementalPricePrefName, _currentIncrementalPrice);
        PlayerPrefs.SetFloat(_pricePrefName, _currentPrice);
        PlayerPrefs.SetInt(_valuePrefName, _currentValue);
    }

    public int GetValue()
    {
        return _currentValue;
    }

    public void OnButtonClick()
    {
        if (CurrencyManager.Instance.GetCurrency() >= _currentPrice)
        {
            CurrencyManager.Instance.DealCurrency(-_currentPrice);
            CurrencyManager.Instance.Save();
            _currentPrice = (int)(_currentPrice + _currentIncrementalPrice);
            _currentIncrementalPrice += _upgradeType switch
            {
                UpgradeType.Damage => _gameConfig.DamageAddIncrementalPrice,
                UpgradeType.FireRate => _gameConfig.FireRateAddIncrementalPrice,
                UpgradeType.FireRange => _gameConfig.FireRangeAddIncrementalPrice,
                UpgradeType.Income => _gameConfig.IncomeAddIncrementalPrice,
                _ => throw new ArgumentOutOfRangeException()
            };
            _currentValue += _addValue;
            UIManager.Instance.UpdateAllUI_IdleButtons();
            Save();
            OnClick?.Invoke(true);
            HapticManager.Haptic(0);
            transform.DOKill(true);
            transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 6);
        }
        else
        {
            OnClick?.Invoke(false);
        }
    }
}