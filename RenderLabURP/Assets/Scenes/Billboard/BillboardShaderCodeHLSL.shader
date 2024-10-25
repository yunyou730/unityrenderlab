Shader "Ayy/AyyUnlit"
{
    Properties {}
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes
            {
                float4 positionOS : POSITION;   // OS:Object Space
            };

            struct Varyings
            {
                float4 positionHCS :SV_POSITION;    // HCS: Homogeneous Clipping Space
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return half4(1.0,1.0,0.0,1.0);
            }
            
            ENDHLSL
        }
    }
}