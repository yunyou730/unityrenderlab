Shader "Ayy/FlowMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlowTex ("Texture", 2D) = "white" {}
        _FlowSpeed ("Flow Speed",Float) = 0.0
        _TimeSpeed ("Time Speed",Float) = 0.4
        
        _MixFactor ("Mix Factor",Range(0.0,1.0)) = 1.0
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

            sampler2D _FlowTex;
            float _FlowSpeed;
            float _TimeSpeed;

            float _MixFactor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 flowDir = tex2D(_FlowTex,i.uv).rg * 2.0 - 1.0;
                flowDir *= -_FlowSpeed;

                float phase0 = frac(_Time.y * _TimeSpeed);
                float phase1 = frac(_Time.y * _TimeSpeed + 0.5);

                float3 tex0 = tex2D(_MainTex,i.uv + flowDir.xy * phase0);
                float3 tex1 = tex2D(_MainTex,i.uv + flowDir.xy * phase1);

                float flowLerp = abs((0.5 - phase0) / 0.5);
                float3 finalColor = lerp(tex0,tex1,flowLerp);


                
                float4 originFlowColor = tex2D(_FlowTex,i.uv);
                finalColor = lerp(originFlowColor,finalColor,_MixFactor);
                
                return float4(finalColor,1.0);
            }
            ENDCG
        }
    }
}
