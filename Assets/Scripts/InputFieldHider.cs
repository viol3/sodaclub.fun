using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldHider : MonoBehaviour
{
    [SerializeField] private TMP_InputField _targetInput;
    [SerializeField] private GameObject _openObject;
    [SerializeField] private GameObject _closeObject;

    private bool _open = false;

    private void OnEnable()
    {
        Close();
    }

    public void OnClick()
    {
        if(_targetInput.contentType == TMP_InputField.ContentType.Password)
        {
            _targetInput.contentType = TMP_InputField.ContentType.Standard;
            _open = true;
            
        }
        else
        {
            _targetInput.contentType = TMP_InputField.ContentType.Password;
            _open = false;
        }
        UpdateUI();
    }

    public void Close()
    {
        _open = false;
        UpdateUI();
    }

    void UpdateUI()
    {
        _targetInput.ForceLabelUpdate();
        _openObject.gameObject.SetActive(_open);
        _closeObject.gameObject.SetActive(!_open);
    }
}
