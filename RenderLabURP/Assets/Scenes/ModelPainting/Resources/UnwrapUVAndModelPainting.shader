Shader "ayy/UnwrapUVAndModelPainting"
{
    Properties
    {
        _AdditiveTexture ("Texture", 2D) = "white" {}
        _PaintingPoint ("Painting Point", Vector) = (0, 0, 0, 0)
        _PrevPoint("Prev Point",Vector) = (0,0,0,0)
        _PrevPointValid("Prev Point Valid",Range(0,1)) = 0
        
        _ShowUnwrapUVDirectly("Show Unwrap UV Directly",Range(0,1)) = 0
        
        _BrushSize("Brush Size",Range(0,3)) = 0.1
        _BrushColor("Brush Color",Color) = (1,0,0,1)
        
        _ShowDebugColor("Show Debug Color",Range(0,1)) = 0
        
        _MeshPartClearColor("Mesh Part Clear Color",Color) = (0,0,0,0)
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
                //float4 testValue: TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

        CBUFFER_START(UnityPerMaterial)
            sampler2D _AdditiveTexture;
            float4 _PaintingPoint;
            float4 _PrevPoint;
            float _PrevPointValid;
            float _ShowUnwrapUVDirectly;

            float _BrushSize;
            float4 _BrushColor;
            
            float4 _MeshPartClearColor;
            
            float _ShowDebugColor;
        CBUFFER_END            

            Varyings vert(Attributes IN)
            {
                float3 localPos = IN.positionOS;
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

            // 判断点 P 是否在距离线段 AB 的短距离范围内
            bool IsPointWithinDistance(float3 A, float3 B, float3 P, float dis)
            {
                float3 AB = B - A;
                float3 BA = A - B;
                float3 AP = P - A;
                float3 BP = P - B;
                
                //float3 dirAB = normalize(B - A);
                if(dot(AP,AB) > 0.0 && dot(BA,BP) > 0.0) // check direction
                {
                    // check distance
                    float3 nAP = normalize(AP);
                    float3 nAB = normalize(AB);
                    float theta = acos(dot(nAP,nAB));
                    float curDis = sin(theta) * length(AP);
                    return curDis <= dis;
                }
            
                return false;
            }            

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float4 addtiveTexColor = tex2D(_AdditiveTexture,uv);

                // 输出的 R通道 是给下一个 Pass 使用的 绘制轨迹数据
                float4 ret = addtiveTexColor;
                if(distance(IN.positionWS,_PaintingPoint.xyz) < _BrushSize)
                {
                    // 最新绘制的点：线段终点
                    ret = lerp(_BrushColor,half4(1.0,0.0,0.0,1.0),_ShowDebugColor);
                }
                else if(_PrevPointValid > 0.5
                    && distance(IN.positionWS,_PrevPoint.xyz) < _BrushSize)
                {
                    // 上一帧绘制的点：线段起点
                    ret = lerp(_BrushColor,half4(1.0,1.0,0.0,1.0),_ShowDebugColor);
                }
                else if(_PrevPointValid > 0.5
                    && IsPointWithinDistance(_PaintingPoint,_PrevPoint,IN.positionWS,_BrushSize))
                {
                    // 起点 和 终点 中间构成的 线段
                    ret = lerp(_BrushColor,half4(1.0,0.0,1.0,1.0),_ShowDebugColor);
                }

                // _ShowUnwrapUVDirectly 为 1: 结果为 uv 展开的 图像, 前景用纯色 
                // _ShowUnwrapUVDirectly 为 0: 叠加涂鸦笔迹
                //const float4 meshPartColor = float4(0,1,0,1);
                ret = lerp(ret,_MeshPartClearColor,_ShowUnwrapUVDirectly);
                
                return ret;
            }
            ENDHLSL
        }
    }
}
