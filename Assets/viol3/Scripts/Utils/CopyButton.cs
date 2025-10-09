using TMPro;
using UnityEngine;

public class CopyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _textComponent;

    public void OnClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLCopyAndPaste.WebGLCopyAndPasteAPI.CopyToClipboard(_textComponent.text);
#else
        GUIUtility.systemCopyBuffer = _textComponent.text;
#endif
    }

}
