using Ali.Helper;
using Ali.Helper.UI;
using UnityEngine;

public class CurrencyManager : LocalSingleton<CurrencyManager>
{
    [SerializeField] private CurrencyPanel _currencyPanel;
    [SerializeField] private string _currencyPrefName = "CURRENCY";
    [SerializeField] private bool _resetMoney = false;

    private float _currency = 0;
        
    public void Init()
    {
        Load();
    }

    public float GetCurrency()
    {
        return _currency;
    }

    public void DealCurrency(float value, bool effectByIncome = true)
    {
        if(value > 0 && effectByIncome)
        {
            value *= UpgradeManager.Instance.Income;
        }
        _currency += value;
        UpdateCurrencyPanel();
        UIManager.Instance.UpdateAllUI_IdleButtons();
        Save();
    }

    public void DealCurrencyButton(float value)
    {
        _currency += value;
        UpdateCurrencyPanel();
        UIManager.Instance.UpdateAllUI_IdleButtons();
        Save();
    }
    
    private void UpdateCurrencyPanel()
    {
        _currencyPanel.SetCurrencyAmount(_currency, false);
    }
       

    private void Load()
    {
        _currency = _resetMoney ? 0 : PlayerPrefs.GetFloat(_currencyPrefName);
        UpdateCurrencyPanel();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat(_currencyPrefName, _currency);
    }

    public CurrencyPanel GetCurrencyPanel() => _currencyPanel;
}