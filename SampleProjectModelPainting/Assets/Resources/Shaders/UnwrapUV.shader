Shader "ayy/UnwrapUV"
{
    Properties
    {   
        _MainColor("Main Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }
        Cull Off 
        ZTest Off 
        //ZWrite Off        
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes
            {
                float4 positionOS : POSITION;   // OS:Object Space
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS :SV_POSITION;    // HCS: Homogeneous Clipping Space
                float2 uv : TEXCOORD0;
            };

        CBUFFER_START(UnityPerMaterial)
            float4 _MainColor;
        CBUFFER_END            

            Varyings vert(Attributes IN)
            {
                //float3 localPos = IN.positionOS;
                float2 uv = IN.uv;

                float4 temp = float4(0,0,0,1);
                temp.xy = float2(uv * 2 - 1) * float2(1,_ProjectionParams.x);
            
                Varyings OUT;
                //OUT.positionHCS = TransformObjectToHClip(localPos);
                OUT.positionHCS = temp;
                OUT.uv = IN.uv;
            
                return OUT;
            }

            
            float4 frag(Varyings IN) : SV_Target
            {
                //float2 uv = IN.uv;
                float4 ret = _MainColor;
                return ret;
            }
            ENDHLSL
        }
    }
}
