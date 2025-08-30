using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ali.Helper.UI
{
    public class TextPoint : MonoBehaviour
    {
        private TextMeshPro _textComponent;

        public void SetText(string text)
        {
            _textComponent.text = text;
        }

        public void SetTextActive(bool value)
        {
            _textComponent.enabled = value;
        }

    }
}