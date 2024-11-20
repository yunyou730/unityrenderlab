Shader "ayy/UnwrapUVWithPaintingAdditive"
{
    Properties
    {
        _AdditiveTexture("Additive Texture",2D) = "black" {}
        
        _EnableCurPos("Enable Cur Pos",Range(0,1)) = 0  // 0 disable, 1 enable
        _CurPos("Current Pos",Vector) = (0,0,0,0)
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
                float3 positionWS : TEXCOORD2;                
            };

        CBUFFER_START(UnityPerMaterial)
            sampler2D _AdditiveTexture;

            float _EnableCurPos;
            float4 _CurPos;            
        CBUFFER_END            

            Varyings vert(Attributes IN)
            {
                float3 localPos = IN.positionOS.xyz;
                float2 uv = IN.uv;

                float4 temp = float4(0,0,0,1);
                temp.xy = float2(uv * 2 - 1) * float2(1,_ProjectionParams.x);
            
                Varyings OUT;
                //OUT.positionHCS = TransformObjectToHClip(localPos);
                OUT.positionHCS = temp;
                OUT.uv = IN.uv;
                OUT.positionWS = TransformObjectToWorld(localPos);
            
                return OUT;
            }

            
            float4 frag(Varyings IN) : SV_Target
            {
                float disToCurPos = distance(IN.positionWS.xyz,_CurPos.xyz);
                const float brushSize = 0.1;
                float4 ret = tex2D(_AdditiveTexture,IN.uv);
                if(_EnableCurPos > 0.5 && disToCurPos < brushSize)
                {
                    ret = float4(1.0,0.0,0.0,1.0);
                }
                return ret;
            }
            ENDHLSL
        }
    }
}
