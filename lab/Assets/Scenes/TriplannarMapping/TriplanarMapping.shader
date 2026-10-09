Shader "ayy/TriplanarMapping"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _BlendOffset ("Blend Offset",Range(0,0.5)) = 0.25
        _BlendExp ("Blend Exp", Range(1,10)) = 1
        
        
        _TopTex ("Texture", 2D) = "white" {}
        
        [Toggle] _DebugWeights("Debug Weights",Float) = 0.0
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
            
            float _BlendOffset;
            float _BlendExp;
            
            float _DebugWeights;        // 0: 不调试 weights; 1: 调试 weights;

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
                if (_DebugWeights > 0.5)
                {
                    // 直接输出 weights 当作颜色, 可以调试过度范围, 让过度范围可视化
                    return float4(weights,1.0);    
                }

                // 1. world Pos 采样贴图, 依赖于 texture 的 wrap mode 是 repeat, 并且贴图是 4方连续,  就可以让贴图看起来比较自然
                // 2. 根据实际需要, 可以让 top texture 和 side texture 不一样
                // 3. 如果 side texture 有比较强的 "方向感", 会造成 横向 x , z的纹理，在衔接处, 一个横向，一个纵向，不自然. 
                //   解决办法 (1) 让 x 或 z 方向的  uv 旋转90度; (2) 或者干脆就直接用 4方连续, 没有那么强方向感的贴图
                half4 colX = tex2D(_MainTex,i.worldPos.yz);
                //half4 colY = tex2D(_MainTex,i.worldPos.xz);
                half4 colY = tex2D(_TopTex,i.worldPos.xz);
                half4 colZ = tex2D(_MainTex,i.worldPos.xy);
                
                // 输出根据 法线方向的占比, 混合后的颜色
                return colX * weights.x + colY * weights.y + colZ * weights.z;
            }
            ENDHLSL
        }
    }
}
