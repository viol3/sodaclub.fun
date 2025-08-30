using Ali.Helper;
using Bermuda.Runner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TextPopupUI : LocalSingleton<TextPopupUI>
{
    [SerializeField] private PopupObjectUI[] _popupObjectUIs;
    [SerializeField] private Color _greenColor;
    public void Init()
    {
        for (int i = 0; i < _popupObjectUIs.Length; i++)
        {
            _popupObjectUIs[i].Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            Spawn(BermudaRunnerCharacter.Instance.transform.position + (Vector3.forward * 10f) + (Vector3.right * Random.Range(-2f, 2f)), "+3 Fire Rate");
        }
    }

    public void Spawn(Vector3 position, string text)
    {
        Spawn(position, text, _greenColor);
    }

    public void Spawn(Vector3 position, string text, Color color)
    {
        PopupObjectUI pou = GetAvailablePopupObject();
        if(pou == null)
        {
            return;
        }
        pou.SetPositionByWorld(position);
        pou.SetText(text);
        pou.SetColor(color);
        pou.Show();
    }

    PopupObjectUI GetAvailablePopupObject()
    {
        for (int i = 0; i < _popupObjectUIs.Length; i++)
        {
            if (!_popupObjectUIs[i].IsShowing())
            {
                return _popupObjectUIs[i];
            }
        }
        return null;
    }
}
