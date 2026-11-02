Shader "JYTest/BaseGrid"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _LineWidth ("Line Width (pixels)", Range(0.5, 8.0)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent"
        }

        Pass
        {
            Name "DiamondOutlinePass"

            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _LineColor;
                float _LineWidth;
            CBUFFER_END

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
                // uv 0..1 maps to the full 36x18 tile. The diamond is the
                // |x| + |y| <= 0.5 region in this aspect-corrected UV space.
                float2 p = IN.uv - 0.5;
                float diamondDistance = abs(p.x) + abs(p.y);

                // fwidth gives the UV distance covered by one screen pixel, so
                // _LineWidth stays consistent regardless of the diamond edge.
                float pixelSize = max(fwidth(diamondDistance), 0.00001);
                float lineWidth = _LineWidth * pixelSize;
                float inner = 0.5 - lineWidth;

                float lineMask =
                    smoothstep(inner - pixelSize, inner, diamondDistance) *
                    (1.0 - smoothstep(0.5 - pixelSize, 0.5, diamondDistance));

                half4 color = IN.color * _LineColor;
                color.a *= lineMask;
                clip(color.a - 0.001);
                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}