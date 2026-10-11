Shader "ayy/TriplanarMappingSteps/s3"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

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
                // abs 是为了防负数
                float3 weights = abs(worldNormal);
                // 减去一个 小数,是为了 让 权重更大的维度,占比更高一些
                // 但注意：normal 经过归一化之后,  xyz值比较小的情况是 (1/sqrt(3), 1/sqrt(3),1/sqrt(3))
                // 也就是说, 如果这个 _BlendOffset的数值  大于  1/sqrt(3),   也就是 0.57735,  会导致 这个值出现负数
                // 即使有  saturate 的帮助， 负数逻辑也是错的。 所以, 需要限制 _BlendOffset 区间在 [0,0.57735) 之间
                weights = saturate(weights - _BlendOffset);

                // pow 是为了把 [0,1] 的线性函数, 转换为更陡峭的曲线, pow的值越多, 上升越快, 边缘越锐利; 否则过度边缘越平缓
                weights = pow(weights,_BlendExp);

                // 这里这样理解, 无论 输入 weights.xyz 的数值是多少, 假设每个维度的值,分别是 a,b,c
                // 输出的结果 weights.x + weights.y + weights.z , 一定是 1.0, 从而构造了 3个加和为 1.0 的权重值
                // 因为
                // a / (a + b + c) + b / (a + b + c) + c / (a + b + c)
                // 可以转换为 (a + b + c) / (a + b + c) = 1 结果一定为 1
                return weights / (weights.x + weights.y + weights.z);
            }

            half4 frag(v2f i) : SV_Target
            {
                // world normal 用于处理权重, 根据  normal 来看, 究竟是要让哪个方向的贴图, 占比更大一些
                float3 weights = getWeights(i.worldNormal);
                // 直接输出 weights 当作颜色, 可以调试过度范围, 让过度范围可视化
                return float4(weights,1.0);
            }
            ENDHLSL
        }
    }
}