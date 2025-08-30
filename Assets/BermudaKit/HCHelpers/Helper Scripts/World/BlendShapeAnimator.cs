using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeAnimator : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _renderer;

    private Tweener tweener;

    public void DOBlendShape(int blendShapeIndex, float endValue, float duration)
    {
        if(tweener != null)
        {
            tweener.Kill();
        }
        tweener = DOTween.To(() => _renderer.GetBlendShapeWeight(blendShapeIndex), x => _renderer.SetBlendShapeWeight(blendShapeIndex, x), endValue, duration);
    }

    private void OnDestroy()
    {
        if (tweener != null)
        {
            tweener.Kill();
        }
    }

}
