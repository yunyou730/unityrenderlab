Shader "ayy/ModelPainting"
{
    Properties
    {
        _AdditiveTexture("Additive Texture",2D) = "white"{}
        
        _BrushSize("Brush Size",Range(0,3)) = 1.0
        _BrushColor("Brush Color",Color) = (1,1,0,1)
        
        _EnableCurPos("Enable Cur Pos",Range(0,1)) = 0  // 0 false, 1 true
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
            
            float _BrushSize;
            float4 _BrushColor; 
            
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
                float2 uv = IN.uv;

                float4 texColor = tex2D(_AdditiveTexture,uv);
                
                
                float4 ret = texColor;//float4(0,0,0,0);

                float dis = distance(IN.positionWS.xyz,_CurPos.xyz);
                if(_EnableCurPos > 0.5 && dis < _BrushSize)
                {
                    ret = _BrushColor;
                }
                // else
                // {
                //     ret = texColor;    
                // }
                
                
                return ret;
            }
            ENDHLSL
        }
    }
}
