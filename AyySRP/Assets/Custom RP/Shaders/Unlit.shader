Shader "Custom RP/Unlit"
{
    Properties
    {
        _BaseColor("Color", Color) = (1.0,1.0,1.0,1.0)
    }
    
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #include "UnlitPass.hlsl"
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            
            ENDHLSL
        }
    }
}
