using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unSelectedColor;
    [Space]
    [SerializeField] private Image _image;
    public void OnClick()
    {
        TabButton[] tabButtons = transform.parent.GetComponentsInChildren<TabButton>();
        foreach (TabButton tabButton in tabButtons)
        {
            tabButton.Unselect();
        }
        Select();
    }

    public void Select()
    {
        _image.color = _selectedColor;
    }

    public void Unselect()
    {
        _image.color = _unSelectedColor;
    }
}
