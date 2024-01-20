Shader "Comet/GridMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridStateTex("Texture",2D) = "white" {}
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
                float2 uv2 : TEXCOORD1;
                float4 vertColor : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 vertColor : COLOR;
                float2 uv2 : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _GridStateTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertColor = v.vertColor;
                o.uv2 = v.uv2;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float kBorderSize = 0.05;
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                
                //col = float4(i.uv.x,i.uv.y,0,0) * i.vertColor;
                //fixed4 col = i.vertColor;
                //fixed4 col = fixed4(i.uv2,0,1);
                fixed4 col = tex2D(_GridStateTex,i.uv2);
                if(i.uv.x < kBorderSize || i.uv.y < kBorderSize || i.uv.x > 1-kBorderSize || i.uv.y > 1-kBorderSize)
                {
                    col = fixed4(0,0,0,0);
                }
                return col;
            }
            ENDCG
        }
    }
}
