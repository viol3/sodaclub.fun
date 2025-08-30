using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class OffsetChanger : MonoBehaviour
    {
        [SerializeField] private float _scrollSpeed = 0.5f;
        private Renderer _renderer;

        void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        void Update()
        {
            float offset = Time.time * _scrollSpeed;
            _renderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }
}