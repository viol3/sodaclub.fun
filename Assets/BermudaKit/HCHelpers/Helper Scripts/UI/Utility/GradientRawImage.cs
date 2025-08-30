using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ali.Helper.UI
{
    [ExecuteInEditMode]
    public class GradientRawImage : MonoBehaviour
    {
        [SerializeField] private Color _topColor;
        [SerializeField] private Color _bottomColor;

        private Texture2D backgroundTexture;
        private RawImage _image;


        void UpdateImage()
        {
            _image = GetComponent<RawImage>();
            backgroundTexture = new Texture2D(1, 2);
            backgroundTexture.wrapMode = TextureWrapMode.Clamp;
            backgroundTexture.filterMode = FilterMode.Bilinear;
            backgroundTexture.SetPixels(new Color[] { _bottomColor, _topColor });
            backgroundTexture.Apply();
            _image.texture = backgroundTexture;
        }

        public void SetColors(Color topColor, Color bottomColor)
        {
            _topColor = topColor;
            _bottomColor = bottomColor;
            UpdateImage();
        }

        public Color GetTopColor()
        {
            return _topColor;
        }

        public Color GetBottomColor()
        {
            return _bottomColor;
        }

    }
}