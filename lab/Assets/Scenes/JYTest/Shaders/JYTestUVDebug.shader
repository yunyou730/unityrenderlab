Shader "JYTest/UVDebug"
{
    SubShader
    {
        // 告诉 URP 这个 SubShader 归它管
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"     = "Opaque"
            "Queue"          = "Geometry"
        }

        Pass
        {
            Name "UVDebugPass"

            Cull Back   // 看不到画面时先改成 Cull Off；如果 Cull Off 才可见 => 三角形绕序反了
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv         = IN.uv;
                OUT.color      = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // u -> 红，v -> 绿，蓝恒为 0，不透明
                float2 uv = IN.uv;
                float2 p = uv - 0.5;
                float d = abs(p.x) + abs(p.y);
                
                float inDiamond = step(d,0.5);
                half4 col = lerp(half4(0.0,0.0,0.0,0.0),IN.color,inDiamond);
                return col;
                
                return half4(IN.uv.x, IN.uv.y, 0.0, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
