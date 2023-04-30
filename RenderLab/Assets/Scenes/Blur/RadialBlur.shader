Shader "Ayy/PostEffect/RadialBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CenterX("CenterX",Range(0,1)) = 0.5
        _CenterY("CenterY",Range(0,1)) = 0.5
        
        _SampleSteps("SampleSteps",Range(1,20)) = 10
        _BlurAmount("BlurAmount",Range(0.0,20.0)) = 1.0
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
            float4 _MainTex_TexelSize;

            float _CenterX;
            float _CenterY;
            int _SampleSteps;
            float _BlurAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                float2 center = float2(_CenterX,_CenterY);
                
                float2 texelSize = _MainTex_TexelSize.xy;
                float2 delta = (i.uv - center) * _BlurAmount;

                float4 color = tex2D(_MainTex, i.uv);
                float4 sum = color;
                for (int j = 1; j <= _SampleSteps; j++) {
                    float2 offset = delta * j;
                    sum += tex2D(_MainTex, i.uv + offset * texelSize);
                    sum += tex2D(_MainTex, i.uv - offset * texelSize);
                }
                return sum / (float(_SampleSteps) * 2.0 + 1.0);
            }            
            
            ENDCG
        }
    }
}
