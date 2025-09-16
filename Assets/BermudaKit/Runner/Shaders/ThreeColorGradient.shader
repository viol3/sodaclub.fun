Shader "UI/ThreeColorGradientUnlit"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1,0,0,1)
        _MidColor ("Middle Color", Color) = (0,1,0,1)
        _BottomColor ("Bottom Color", Color) = (0,0,1,1)
        _MidPoint ("Mid Point", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _BottomColor;
            float4 _MidColor;
            float4 _TopColor;
            float _MidPoint;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                half t = i.uv.y;

                half4 color;
                if(t < _MidPoint)
                {
                    color = lerp(_BottomColor, _MidColor, t / _MidPoint);
                }
                else
                {
                    color = lerp(_MidColor, _TopColor, (t - _MidPoint) / (1.0 - _MidPoint));
                }
                return color;
            }
            ENDCG
        }
    }
}
