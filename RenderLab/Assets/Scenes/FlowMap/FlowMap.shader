Shader "Ayy/FlowMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlowTex ("Texture", 2D) = "white" {}
        _FlowIntensity ("Flow Intensity",Float) = 0.3
        _FlowSpeed ("Flow Speed",Float) = 0.4
        
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
            float _FlowIntensity;
            float _FlowSpeed;

            float _MixFactor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            // no phase lerp
            // fixed4 frag (v2f i) : SV_Target
            // {
            //     // map [0,1] => [-1,+1], Flow Color as Flow Dir
            //     float2 flowDir = tex2D(_FlowTex,i.uv).rg * 2.0 - 1.0;
            //
            //     // Flow Dir with intensity.
            //     // Here should notice MINUS SIGN.Move dir & uv dir need be inverted
            //     flowDir *= -_FlowIntensity;
            //
            //     // phase0 has relation with time,and affect on uv offset 
            //     float phase0 = frac(_Time.y * _FlowSpeed);
            //     float3 tex0 = tex2D(_MainTex,i.uv + flowDir * phase0);
            //     
            //     // for visualize debugging
            //     float3 finalColor = tex0;
            //     float4 originFlowColor = tex2D(_FlowTex,i.uv);
            //     finalColor = lerp(originFlowColor,finalColor,_MixFactor);
            //     
            //     return float4(finalColor,1.0);
            // }            

            // with phase lerp
            fixed4 frag (v2f i) : SV_Target
            {
                // map [0,1] => [-1,+1], Flow Color as Flow Dir
                float2 flowDir = tex2D(_FlowTex,i.uv).rg * 2.0 - 1.0;
            
                // Flow Dir with intensity.
                // Here should notice MINUS SIGN.Move dir & uv dir need be inverted
                flowDir *= -_FlowIntensity;
                
                // Animate with time , and solve suddenly change
                float phase0 = frac(_Time.y * _FlowSpeed);  // phase0 & phase1 means life-time, value between [0,1]
                float phase1 = frac(_Time.y * _FlowSpeed + 0.5); // 0.5 means half-life
                float flowLerp = abs((0.5 - phase0) / 0.5); // how far from phase0 to 0.5 
                
                float3 tex0 = tex2D(_MainTex,i.uv + flowDir * phase0);
                float3 tex1 = tex2D(_MainTex,i.uv + flowDir * phase1);                
                float3 finalColor = lerp(tex0,tex1,flowLerp);

                // for visualize debugging
                float4 originFlowColor = tex2D(_FlowTex,i.uv);
                finalColor = lerp(originFlowColor,finalColor,_MixFactor);
                
                return float4(finalColor,1.0);
            }
            ENDCG
        }
    }
}
