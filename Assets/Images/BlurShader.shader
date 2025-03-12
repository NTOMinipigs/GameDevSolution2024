Shader "Custom/GaussianBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 10)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = float2(_BlurSize / _ScreenParams.x, _BlurSize / _ScreenParams.y);

                fixed4 col = tex2D(_MainTex, i.uv) * 0.2;
                col += tex2D(_MainTex, i.uv + offset) * 0.15;
                col += tex2D(_MainTex, i.uv - offset) * 0.15;
                col += tex2D(_MainTex, i.uv + float2(offset.x, 0)) * 0.15;
                col += tex2D(_MainTex, i.uv + float2(0, offset.y)) * 0.15;
                col += tex2D(_MainTex, i.uv - float2(offset.x, 0)) * 0.15;
                col += tex2D(_MainTex, i.uv - float2(0, offset.y)) * 0.15;

                return col;
            }
            ENDCG
        }
    }
}
