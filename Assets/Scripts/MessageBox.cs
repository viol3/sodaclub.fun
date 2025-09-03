using Ali.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : GenericSingleton<MessageBox>
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _okButton;

    private UnityEvent OnOkClick;

    private void Start()
    {
        OnOkClick = new UnityEvent();
        ResetListeners();
    }

    public bool IsOpen()
    {
        return _panel.activeInHierarchy;
    }

    public void ResetListeners()
    {
        OnOkClick.RemoveAllListeners();
        OnOkClick.AddListener(ClosePanel);
    }

    public void SetButtonActive(bool active)
    {
        _okButton.gameObject.SetActive(active);
    }

    public void Show(string text)
    {
        _messageText.text = text;
        Debug.Log(text);
        _panel.SetActive(true);
    }

    void ClosePanel()
    {
        _panel.SetActive(false);
    }

    public void OnOKButtonClick()
    {
        //_okButton.gameObject.SetActive(false);
        OnOkClick?.Invoke();
    }

    public void Register(UnityAction action)
    {
        OnOkClick.RemoveAllListeners();
        OnOkClick.AddListener(ClosePanel);
        OnOkClick.AddListener(action);
    }
}
