using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ali.Helper.UI
{
    public class RatioBarVertical : MonoBehaviour
    {
        [SerializeField] private Text _percentText;

        private Image _image;

        // Update is called once per frame
        void Update()
        {
            _percentText.text = "%" + (int)(_image.fillAmount * 100f);
        }

        public void Init()
        {
            _image = GetComponent<Image>();
        }

        public void SetRatio(float ratio)
        {
            _image.fillAmount = ratio;
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }
    }
}