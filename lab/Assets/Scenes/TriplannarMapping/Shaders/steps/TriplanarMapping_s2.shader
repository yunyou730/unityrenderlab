Shader "ayy/TriplanarMappingSteps/s2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                // abs 是为了防负数
                float3 weights = abs(worldNormal);

                // 这里这样理解, 无论 输入 weights.xyz 的数值是多少, 假设每个维度的值,分别是 a,b,c
                // 输出的结果 weights.x + weights.y + weights.z , 一定是 1.0, 从而构造了 3个加和为 1.0 的权重值
                // 因为
                // a / (a + b + c) + b / (a + b + c) + c / (a + b + c)
                // 可以转换为 (a + b + c) / (a + b + c) = 1 结果一定为 1
                return weights / (weights.x + weights.y + weights.z);
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 weights = getWeights(i.worldNormal);
                return float4(weights,1.0);
            }
            ENDHLSL
        }
    }
}