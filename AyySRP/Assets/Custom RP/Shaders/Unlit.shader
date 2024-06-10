Shader "Custom RP/Unlit"
{
    Properties
    {
        _BaseMap("Texture",2D) = "white" {}
        _BaseColor("Color", Color) = (1.0,1.0,1.0,1.0)
        _CutOff("Alpha CutOff",Range(0.0,1.0)) = 0.5
        
        [Toggle(_CLIPPING)] _Clippint("Alpha Clipping",Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend",Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend",Float) = 0
        [Enum(Off,0,On,1)] _ZWrite("Z Write",Float) = 1
    }
    
    SubShader
    {
        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            HLSLPROGRAM
            #include "UnlitPass.hlsl"

            #pragma shader_feature _CLIPPING
            #pragma multi_compile_instancing
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            
            ENDHLSL
        }
    }
}
