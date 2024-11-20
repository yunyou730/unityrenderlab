Shader "ayy/UnwrapUVWithTwoPoints"
{
    Properties
    {
        _AdditiveTexture("Additive Texture",2D) = "black" {}
        
        _EnableCurPos("Enable Cur Pos",Range(0,1)) = 0  // 0 disable, 1 enable
        _CurPos("Current Pos",Vector) = (0,0,0,0)
        
        _EnablePrevPos("Enable Cur Pos",Range(0,1)) = 0  // 0 disable, 1 enable
        _PrevPos("Current Pos",Vector) = (0,0,0,0)
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

            float _EnablePrevPos;
            float4 _PrevPos;
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
                const float brushSize = 0.1;
                
                float disToCurPos = distance(IN.positionWS.xyz,_CurPos.xyz);
                float disToPrevPos = distance(IN.positionWS.xyz,_PrevPos.xyz);
                float disToLineSeg = 0.0;
                
                float4 ret = tex2D(_AdditiveTexture,IN.uv);
                if(_EnableCurPos > 0.5 && disToCurPos < brushSize)
                {
                    // Current point 
                    ret = float4(1.0,0.0,0.0,1.0);
                }
                else if(_EnablePrevPos > 0.5 && disToPrevPos < brushSize)
                {
                    // Previous point
                    ret = float4(1.0,1.0,0.0,1.0);
                }
                else if(_EnablePrevPos > 0.5 && IsPointWithinDistance(_CurPos.xyz,_PrevPos.xyz,IN.positionWS.xyz,brushSize,disToLineSeg))
                {
                    // line segment
                    ret = float4(0.0,1.0,1.0,1.0);
                }
                
                return ret;
            }
            ENDHLSL
        }
    }
}
