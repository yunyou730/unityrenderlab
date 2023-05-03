Shader "Custom RP/Unlit"
{
    Properties
    {
        
    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            #include "UnlitPass.hlsl"
            
            /*
            float4 UnlitPassVertex(float3 positionOS : POSITION) : SV_POSITION
            {
                return float4(positionOS,1.0);
            }

            float4 UnlitPassFragment() : SV_TARGET
            {
                return 1.0;
            }
            */
            
            ENDHLSL
        }
    }
}
