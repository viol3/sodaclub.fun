using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderButton : MonoBehaviour
{
    public UnityEvent OnClick;

    // Update is called once per frame
    private void OnMouseUpAsButton()
    {
        OnClick.Invoke();
    }
}
