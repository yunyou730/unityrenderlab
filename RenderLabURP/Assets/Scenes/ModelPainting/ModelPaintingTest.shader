Shader "ayy/ModelPaintingTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PaintingPoint ("Painting Points", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        Cull Off 
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
                float4 testValue: TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

        CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float4 _PaintingPoint;
        CBUFFER_END            

            Varyings vert(Attributes IN)
            {
                float3 localPos = IN.positionOS;
                float2 uv = IN.uv;

                float4 temp = float4(0,0,0,1);
                temp.xy = float2(uv * 2 - 1) * float2(1,_ProjectionParams.x);

                // @miao @temp
                // 让 uv 贴图 有一点点 深透 & 溢出
                //temp.xy *= 1.01;
            
                Varyings OUT;
                //OUT.positionHCS = TransformObjectToHClip(localPos);
                OUT.positionHCS = temp;
                OUT.uv = IN.uv;
                OUT.testValue = _ProjectionParams;
                OUT.positionWS = TransformObjectToWorld(localPos);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float4 ret1 = tex2D(_MainTex,uv);
                if(distance(IN.positionWS,_PaintingPoint.xyz) < 0.1)
                {
                    ret1 = half4(1.0,0.0,0.0,1.0);
                }
                return ret1;
                
                //float2 uv = IN.uv;
                float4 texCol = tex2D(_MainTex,uv);
                half4 ret = texCol;//float4(IN.uv.x,IN.uv.y,0.0,1.0);

                //ret = float4(IN.testValue.x,IN.testValue.x,IN.testValue.x,1.0);
                return ret;
            }
            ENDHLSL
        }
    }
}
