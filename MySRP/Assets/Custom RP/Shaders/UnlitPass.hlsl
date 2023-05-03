#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"

float4 UnlitPassVertex(float3 positionOS : POSITION) : SV_POSITION
{
    float3 positionWS = TransformObjectToWorld(positionOS);
    return TransformWorldToHClip(positionWS);
}

float4 UnlitPassFragment() : SV_TARGET
{
    return float4(1.0,0.0,0.0,1.0);
}

#endif
