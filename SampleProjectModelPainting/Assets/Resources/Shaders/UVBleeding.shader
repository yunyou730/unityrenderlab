Shader "ayy/UVBleeding"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float4 _MainTex_TexelSize;
        CBUFFER_END            

            Varyings vert(Attributes IN)
            {
                float3 localPos = IN.positionOS;
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(localPos);
                OUT.uv = IN.uv;
                return OUT;
            }
            
            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                const float dis = 2.0f; // 2个像素的膨胀  
                float2 uvs[9];
                uvs[0] = uv + float2(0,0) * dis;
                uvs[1] = uv + float2(-_MainTex_TexelSize.x,0) * dis;
                uvs[2] = uv + float2( _MainTex_TexelSize.x,0) * dis;
                uvs[3] = uv + float2( 0,_MainTex_TexelSize.y) * dis;
                uvs[4] = uv + float2(-_MainTex_TexelSize.x,_MainTex_TexelSize.y) * dis;
                uvs[5] = uv + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * dis;
                uvs[6] = uv + float2(-_MainTex_TexelSize.x,-_MainTex_TexelSize.y) * dis;
                uvs[7] = uv + float2(0,-_MainTex_TexelSize.y) * dis;
                uvs[8] = uv + float2(_MainTex_TexelSize.x,-_MainTex_TexelSize.y) * dis;

                // //float4 res = float4(0,0,0,0);
                // float4 res = tex2D(_MainTex,uvs[0]);
                // const float kThreshold = 0.0001;
                // if(res.a <= kThreshold)
                // {
                //     for(int i = 0;i < 9;i++)
                //     {
                //         float4 tempColor = tex2D(_MainTex,uvs[i]);
                //         if(tempColor.a >= kThreshold)
                //         {
                //             res = tempColor;
                //             break;
                //         }
                //     }   
                // }

                float4 ret = float4(0,0,0,0);
                for(int i = 0;i < 9;i++)
                {
                    float4 tempColor = tex2D(_MainTex,uvs[i]);
                    if(tempColor.a > ret.a)
                    {
                        ret = tempColor;
                    }
                }                   
                
                return ret;   
                // return ret1;
            }
            ENDHLSL
        }
    }
}
