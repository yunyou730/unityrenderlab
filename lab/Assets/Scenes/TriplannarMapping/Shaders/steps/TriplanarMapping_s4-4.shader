Shader "ayy/TriplanarMappingSteps/s4-4"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TopTex ("Texture", 2D) = "white" {}

        _BlendOffset ("Blend Offset",Range(0,0.5)) = 0.25
        _BlendExp ("Blend Exp", Range(1,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;

                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _TopTex;
            
            float4 _MainTex_ST;
            float4 _TopTex_ST;

            float _BlendOffset;
            float _BlendExp;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal,true);
                return o;
            }

            float3 getWeights(float3 worldNormal)
            {
                float3 weights = abs(worldNormal);
                weights = saturate(weights - _BlendOffset);
                weights = pow(weights,_BlendExp);

                return weights / (weights.x + weights.y + weights.z);
            }

            half4 frag(v2f i) : SV_Target
            {
                // world normal 用于处理权重, 根据  normal 来看, 究竟是要让哪个方向的贴图, 占比更大一些
                float3 weights = getWeights(i.worldNormal);
                
                // 3平面采样 sample textures
                half4 colX = tex2D(_MainTex,i.worldPos.yz * _MainTex_ST.xy + _MainTex_ST.zw);
                half4 colY = tex2D(_TopTex,i.worldPos.xz * _TopTex_ST.xy + _TopTex_ST.zw);
                half4 colZ = tex2D(_MainTex,i.worldPos.xy * _MainTex_ST.xy + _MainTex_ST.zw);
                
                // 按权重混合
                return colX * weights.x + colY * weights.y + colZ * weights.z;
            }
            ENDHLSL
        }
    }
}