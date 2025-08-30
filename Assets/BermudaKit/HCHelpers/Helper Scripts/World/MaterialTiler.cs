using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ali.Helper.World
{
    public class MaterialTiler : MonoBehaviour
    {
        [SerializeField] private Vector2 _scrollVelocity;
        Renderer rend;

        void Start()
        {
            rend = GetComponent<Renderer>();
        }

        void Update()
        {
            float offsetX = Time.time * _scrollVelocity.x;
            float offsetY = Time.time * _scrollVelocity.y;
            rend.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
        }

        public void SetScrollVelocity(Vector2 velocity)
        {
            _scrollVelocity = velocity;
        }

    }
}