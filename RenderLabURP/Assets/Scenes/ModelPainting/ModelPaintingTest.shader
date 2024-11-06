Shader "ayy/ModelPaintingTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PaintingPoints ("Painting Points", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        // Cull Off 
        // ZTest Off 
        // ZWrite Off        
        
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
            sampler2D _MainTex;
            float4 _PaintingPoints;
        CBUFFER_END            

            Varyings vert(Attributes IN)
            {
                float3 localPos = IN.positionOS;
                float2 uv = IN.uv;

                float4 temp = float4(0,0,0,1);
                float2 factor = uv * 2 - 1;
                temp.xy = float2(1,_ProjectionParams.x) * factor;
                
                Varyings OUT;
                //OUT.positionHCS = TransformObjectToHClip(localPos);
                OUT.positionHCS = temp;
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //return 1;
                
                float2 uv = IN.uv;
                float4 texCol = tex2D(_MainTex,uv);
                float4 ret = texCol;//float4(IN.uv.x,IN.uv.y,0.0,1.0);
                return ret;
            }
            ENDHLSL
        }
    }
}
