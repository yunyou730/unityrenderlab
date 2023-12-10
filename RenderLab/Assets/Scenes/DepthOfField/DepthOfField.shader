Shader "ayy/DepthOfField"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex,_CameraDepthTexture;
    float4 _MainTex_TexelSize;

    float _FocusDistance,_FocusRange;


    float _blurKernelSize;
    float _BlurRadius;

    struct VertexData
    {
        float4 vertex: POSITION;
        float2 uv : TEXCOORD0;
    };

    struct Interpolators
    {
        float4 pos : SV_POSITION;
        float2 uv :TEXCOORD0;
    };

    Interpolators VertexProgram(VertexData v)
    {
        Interpolators i;
        i.pos = UnityObjectToClipPos(v.vertex);
        i.uv = v.uv;
        return i;
    }
    

    ENDCG
    
    
    SubShader
    {
        // No culling or depth
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass	//CircleOfConfusionPass
        {
            CGPROGRAM
            
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			half FragmentProgram (Interpolators i) : SV_Target {
			    float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
			    depth = LinearEyeDepth(depth);
			    //depth = Linear01Depth(depth);

			    float coc = (depth - _FocusDistance) / _FocusRange;
                coc = clamp(coc,-1,1);

				return coc;
			}
            
            ENDCG
        }
    	
    	Pass // Bokeh pass
    	{
    		CGPROGRAM
			#pragma vertex VertexProgram
    		#pragma fragment FragmentProgram

    // 		half4 FragmentProgram(Interpolators i) : SV_Target
    // 		{
				// //half3 color = tex2D(_MainTex,i.uv).rgb;
		  //
    // 			half3 color = 0;
    // 			float weight = 0;
    // 			for(int u = -4;u <= 4;u++)
    // 			{
    // 				for(int v = -4;v <= 4;v++)
    // 				{
    // 					float2 o = float2(u,v);
    // 					if(length(o) <= 4)
    // 					{
    // 						o *= _MainTex_TexelSize.xy * _blurKernelSize;
    // 						color += tex2D(_MainTex,i.uv + o).rgb;
    // 						weight += 1;
    // 					}
    // 				}
    // 			}
				// //color *= 1.0 / 81;
    // 			color *= 1.0/weight;
    // 			
    // 			return half4(color,1);
    // 		}
    		
    		half4 FragmentProgram(Interpolators i) : SV_Target
    		{
				half3 color = 0;
				
				static const int kernelSampleCount = 16;
				static const float2 kernel[kernelSampleCount] = {
					float2(0, 0),
					float2(0.54545456, 0),
					float2(0.16855472, 0.5187581),
					float2(-0.44128203, 0.3206101),
					float2(-0.44128197, -0.3206102),
					float2(0.1685548, -0.5187581),
					float2(1, 0),
					float2(0.809017, 0.58778524),
					float2(0.30901697, 0.95105654),
					float2(-0.30901703, 0.9510565),
					float2(-0.80901706, 0.5877852),
					float2(-1, 0),
					float2(-0.80901694, -0.58778536),
					float2(-0.30901664, -0.9510566),
					float2(0.30901712, -0.9510565),
					float2(0.80901694, -0.5877853),
				};
				
				for (int k = 0; k < kernelSampleCount; k++) {
					float2 o = kernel[k];
					o *= _MainTex_TexelSize.xy * _blurKernelSize;
					color += tex2D(_MainTex, i.uv + o).rgb;
				}
				color *= 1.0 / kernelSampleCount;
    			
    			return half4(color,1);
    		}
    		
    		
    		ENDCG
        }
    }
}
