//UNITY_SHADER_NO_UPGRADE
#ifndef AYY_POLAR_COORDINATES_INCLUDED
#define AYY_POLAR_COORDINATES_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"

void AyyPolarUV_float(float2 uv,out float2 Out)
{
    const float TAU = 2 * 3.14;
    float2 delta = uv;                     // if uv[0,1], center = vec2(0.5,0.5),then uv [-0.5,0.5] 
    float radius = length(delta) * 2.0;             // length(delta) [0, 0.5 * 1.414]
    float angle = atan2(delta.y,delta.x) / TAU;    // angle [-pi,+pi] , over 2*pi [-0.5,+0.5]
    
    Out = float2(radius,angle);
    
    //angle += 0.5;
    //Out = float2(0.0,angle);
}

void AyyCartesianUV_float(float2 polarUV,out float2 Out)
{
    const float TAU = 2 * 3.14;
    
    float theta = polarUV.y * TAU;
    float radius = polarUV.x * 0.5;

    float x = cos(theta) * radius;
    float y = sin(theta) * radius;
    
    Out = float2(x,y);
}

void AyyTestFunc1_float(UnityTexture2D tex,UnitySamplerState samplerState,float2 uv,float Time,out float3 Out)
{
    float2 polarUV,cartesianUV;
    AyyPolarUV_float(uv - float2(0.5,0.5),polarUV);

    polarUV.x *= 1.0 + sin(Time) * 0.2;
    polarUV.y += Time * 0.1;
    
    AyyCartesianUV_float(polarUV,cartesianUV);
    cartesianUV += float2(0.5,0.5);


    float2 sampleUV = cartesianUV;
    float4 col;
    col = SAMPLE_TEXTURE2D(tex,samplerState,sampleUV);
    Out = col.rgb;
}

void AyyTestFunc2_float(UnityTexture2D tex,UnitySamplerState samplerState,float2 uv,float Time,out float3 Out)
{
    float2 polarUV,cartesianUV;
    AyyPolarUV_float(uv - float2(0.5,0.5),polarUV);

    polarUV.y += sin(Time) * polarUV.x;
    
    AyyCartesianUV_float(polarUV,cartesianUV);
    cartesianUV += float2(0.5,0.5);


    float2 sampleUV = cartesianUV;
    float4 col;
    col = SAMPLE_TEXTURE2D(tex,samplerState,sampleUV);
    Out = col.rgb;
}

void AyyTestFunc3_float(UnityTexture2D tex,UnitySamplerState samplerState,float2 uv,float Time,out float3 Out)
{
    float2 polarUV;;
    AyyPolarUV_float(uv - float2(0.5,0.5),polarUV);
    
    polarUV.y *= 3.0;
    
    float2 sampleUV = polarUV;
    float4 col;
    col = SAMPLE_TEXTURE2D(tex,samplerState,sampleUV);
    Out = col.rgb;
}


#endif //AYY_POLAR_COORDINATES_INCLUDED