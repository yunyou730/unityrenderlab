Shader "Ayy/Brush/Present"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BrushTex ("Texture",2D) = "white" {}
        _BrushColor ("BrushColor",Color) = (1.0,1.0,1.0,1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
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

            sampler2D _BrushTex;
            float4 _BrushColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 frameBufferColor = tex2D(_MainTex, uv);
                fixed4 brushColor = tex2D(_BrushTex,uv);

                fixed4 col = frameBufferColor;
                if(brushColor.a > 0.0)
                {
                    col = _BrushColor;
                }
                return col;
            }
            ENDCG
        }
    }
}
