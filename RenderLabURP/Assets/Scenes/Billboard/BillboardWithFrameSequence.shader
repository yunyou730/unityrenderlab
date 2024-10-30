Shader "Ayy/BillboardWithFrameSequence"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
                
        _CameraPosition ("Camera Position",Vector) = (0.0,0.0,0.0)
        _VerticalBillboard ("Vertical Billboarding",Range(0,1)) = 1
        
        _FrameCountInRow ("Frame Count In Row", Float) = 0
        _FrameCountInCol ("Frame Count In Col", Float) = 0
        _FrameIndex ("Frame Index", Float) = 0
    }
    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"
        }
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        
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

            // Billboard parameters
            float3 _CameraPosition;
            float _VerticalBillboard;

            // Frame sequence parameters
            float _FrameCountInRow;
            float _FrameCountInCol;
            float _FrameIndex;
            
            CBUFFER_END
            
            float3 RecalculateAsBillboard1(float3 originLocalPos)
            {
                float3 center = float3(0.0,0.0,0.0);
                float3 viewer = TransformWorldToObject(_CameraPosition);
                
                float3 normalDir = viewer - center;
                normalDir.y = normalDir.y * _VerticalBillboard;     // _VerticalBillboard,0 or 1 
                normalDir = normalize(normalDir);

                float3 upDir = abs(normalDir.y) > 0.999 ? float3(0,0,1) : float3(0,1,0);
                float3 rightDir = normalize(cross(upDir,normalDir));
                upDir = normalize(cross(normalDir,rightDir));
                rightDir = -rightDir;   // fix invert uv.x
                
                float3 centerOffset = originLocalPos - center;

                /*
                 * 这里 用 加法，而不用 矩阵 变换的原因是 
                 * 矩阵变换的几何含义：一个坐标在坐标系A的描述, 改为坐标系B 的描述;
                 * 而这里, 并不是换个B坐标系 来描述, 而是 在 B 坐标系下  每个顶点的相对位置
                 * 因此 方法2 是错误的 , 这个方法是正确的. 必须用坐标加法 来做 
                 */
                float3 localPos = center + rightDir * centerOffset.x + upDir * centerOffset.y + normalDir * centerOffset.z;

                return localPos;
            }
            

            Varyings vert(Attributes IN)
            {
                float3 localPos = RecalculateAsBillboard1(IN.positionOS.xyz);
                
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(localPos);
                OUT.uv = IN.uv;
                return OUT;
            }


            float2 GetUVWithFrameIndex(float2 uv)
            {
                int atRow = (int)(_FrameIndex / _FrameCountInRow);
                int atCol = (int)(_FrameIndex - atRow * _FrameCountInCol);


                float tileHeight = 1.0 / _FrameCountInRow;
                float tileWidth = 1.0 / _FrameCountInCol;
                float2 tileSize = float2(tileWidth,tileHeight);
                
                
                float2 leftBottom;
                leftBottom.x = tileWidth * atCol;
                leftBottom.y = 1.0 - tileHeight * atRow;

                //float2 rightTop
                //rightTop = leftBottom + float2(tileWidth,tileHeight);
                
                float2 ret = leftBottom +  tileSize * uv;
                
                return ret;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                float2 tileUV = GetUVWithFrameIndex(uv);
                float4 texColor = tex2D(_MainTex,tileUV);
                //float4 texColor = tex2D(_MainTex,uv);
                //float4 ret = float4(texColor.a,texColor.a,texColor.a,texColor.a); //float4(uv.x,uv.y,0.0,1.0);
                float4 ret = texColor;
                return ret;
            }
            
            ENDHLSL
        }
    }
}