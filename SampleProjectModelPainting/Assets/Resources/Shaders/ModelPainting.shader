Shader "ayy/ModelPainting"
{
    Properties
    {
        _AdditiveTexture("Additive Texture",2D) = "white"{}
        
        _BrushSize("Brush Size",Range(0,3)) = 1.0
        _BrushColor("Brush Color",Color) = (1,1,0,1)
        
        _EnableCurPos("Enable Cur Pos",Range(0,1)) = 0  // 0 disable, 1 enable
        _CurPos("Current Pos",Vector) = (0,0,0,0)
        
        _EnablePrevPos("Enable Prev Pos",Range(0,1)) = 0 // 0 disable, 1 enable
        _PrevPos("Prev Pos",Vector) = (0,0,0,0)
        
        // Debug colors
        _EnableDebugColor("Enable Debug Color",Range(0,1)) = 0  // 0/1 disable/enable debug color
        _DebugCurPosColor("Debug Cur Pos Color",Color) = (1,0,0,1)
        _DebugPrevPosColor("Debug Prev Pos Color",Color) = (0,0,1,1)
        _DebugLineSegColor("Debug Line Seg Color",Color) = (0,1,1,1)
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

            float _EnablePrevPos;
            float4 _PrevPos;

            float _EnableDebugColor;
            float4 _DebugCurPosColor;
            float4 _DebugPrevPosColor;
            float4 _DebugLineSegColor;
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

            // 判断点 P 是否在距离线段 AB 的短距离范围内
            bool IsPointWithinDistance(float3 A, float3 B, float3 P, float dis,out float curDis)
            {
                curDis = 0.0;
            
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
                    curDis = sin(theta) * length(AP);
                    return curDis <= dis;
                }
            
                return false;
            }            

            
            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                float4 texColor = tex2D(_AdditiveTexture,uv);
                
                
                float4 ret = texColor;//float4(0,0,0,0);

                float disToCurPos = distance(IN.positionWS.xyz,_CurPos.xyz);
                float disToPrevPos = distance(IN.positionWS.xyz,_PrevPos.xyz);
                float disToLineSeg = 0.0;
                bool isNearLineSeg = IsPointWithinDistance(_CurPos.xyz,_PrevPos.xyz,IN.positionWS.xyz,_BrushSize,disToLineSeg);
                
                if(_EnableCurPos > 0.5 && disToCurPos < _BrushSize)
                {
                    float4 col = lerp(_BrushColor,_DebugCurPosColor,step(0.5,_EnableDebugColor));
                    ret = col;
                }
                else if(_EnablePrevPos > 0.5 && disToPrevPos < _BrushSize)
                {
                    float4 col = lerp(_BrushColor,_DebugPrevPosColor,step(0.5,_EnableDebugColor));
                    ret = col;
                }
                else if(_EnablePrevPos > 0.5 && isNearLineSeg)
                {
                    float4 col = lerp(_BrushColor,_DebugLineSegColor,step(0.5,_EnableDebugColor));
                    ret = col;
                }
                
                return ret;
            }
            ENDHLSL
        }
    }
}
