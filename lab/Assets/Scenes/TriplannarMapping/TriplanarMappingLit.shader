Shader "ayy/TriplanarMappingLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _BlendOffset ("Blend Offset",Range(0,0.5)) = 0.25
        _BlendExp ("Blend Exp", Range(1,10)) = 1


        _TopTex ("Top Texture", 2D) = "white" {}

        [Toggle] _DebugWeights("Debug Weights",Float) = 0.0

        // ===== 简单光照参数 =====
        // 光源方向 / 颜色 / 强度都从 URP 主光源 (场景里的 Directional Light) 动态获取, 不在这里写死
        // 漫反射模型切换: Lambert (saturate(N·L)) 或 Half Lambert (N·L*0.5+0.5, 背光面也有 0.5 强度, 过渡更柔和)
        [Toggle] _UseHalfLambert("Use Half Lambert", Float) = 1.0
        _AmbientColor ("Ambient Color", Color) = (0.3, 0.3, 0.3, 1.0)
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"   // 提供 _MainLightPosition / _MainLightColor, 跟随场景主光源

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

            float4 _TopTex_ST,_MainTex_ST;

            float _BlendOffset;
            float _BlendExp;

            float _DebugWeights;        // 0: 不调试 weights; 1: 调试 weights;

            // 简单光照: 光源方向 / 颜色由 URP 主光源提供 (见 Lighting.hlsl 里的 _MainLightPosition / _MainLightColor)
            float  _UseHalfLambert;     // 0: 标准 Lambert; 1: Half Lambert (背光面也保留 0.5, 过渡更柔和)
            float4 _AmbientColor;

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
                // 4. 基础版本, 可以不  * st.xy + st.zw ;  考虑进去 st.xy zw 是用于做 纹理的 tiling offset
                half4 colX = tex2D(_MainTex,i.worldPos.yz * _MainTex_ST.xy + _MainTex_ST.zw);
                //half4 colY = tex2D(_MainTex,i.worldPos.xz * _MainTex_ST.xy + _MainTex_ST.zw);
                half4 colY = tex2D(_TopTex,i.worldPos.xz * _TopTex_ST.xy + _TopTex_ST.zw);
                half4 colZ = tex2D(_MainTex,i.worldPos.xy * _MainTex_ST.xy + _MainTex_ST.zw);

                // 输出根据 法线方向的占比, 混合后的颜色
                half3 baseColor = colX.rgb * weights.x + colY.rgb * weights.y + colZ.rgb * weights.z;

                // ===== 简单 Lambert / Half Lambert 漫反射 (跟随 URP 主光源) =====
                // worldNormal 在顶点阶段已经过 TransformObjectToWorldNormal(normal, true) 归一化; 这里再 normalize 一次更稳健
                // URP: _MainLightPosition.xyz 是 "指向光源" 的单位向量, dot(N, L) 直接就是 N·L
                //     _MainLightColor.rgb 已经乘上了光源强度
                float3 normalWS  = normalize(i.worldNormal);
                float3 lightDir  = _MainLightPosition.xyz;          // 跟随场景主光源方向
                float  NdotL     = dot(normalWS, lightDir);         // [-1, 1]

                // Half Lambert 把 [-1,1] 映射到 [0,1], 背光面也保留 0.5 强度, 过渡更柔和, 适合卡通/写实风格
                // 标准 Lambert 用 saturate 截到 [0,1], 背光面只剩环境光, 边缘更硬
                float  diffuse   = (_UseHalfLambert > 0.5) ? (NdotL * 0.5 + 0.5) : saturate(NdotL);

                // 最终颜色 = 基础色 × (主光源颜色 * diffuse + 环境光)
                half3 lit = baseColor * (_MainLightColor.rgb * diffuse + _AmbientColor.rgb);

                return half4(lit, 1.0);
            }
            ENDHLSL
        }
    }
}