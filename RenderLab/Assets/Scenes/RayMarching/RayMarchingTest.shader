Shader "ayy/RayMarchingTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            static const float MIN_DIS = 0.0;
            static const float MAX_DIS = 100.0;
            static const int MAX_MARCHING_STEPS = 255;
            static const float EPSILON = 0.0001;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            float3 rayDirection(float fieldOfView, float2 size, float2 fragCoord)
            {
                float2 xy = fragCoord - size / 2.0;
                float z = (size.y) / tan(radians(fieldOfView) / 2.0);
                return normalize(float3(xy.x,xy.y,z));
            }

            
            float sphereSDF(float3 samplePoint)
            {
                return length(samplePoint) - 1.0;
            }
            

            float sphereSDF(float3 center,float radius,float3 p)
            {
                return length(p - center) - radius;
            }
            
            float shortestDistanceToSurface(float3 eye,float3 marchingDir,float start,float end)
            {
                float depth = start;
                for(int i = 0;i < MAX_MARCHING_STEPS;i++)
                {
                    float3 p = eye + depth * marchingDir;
                    float dist = sphereSDF(float3(0,0,8),0.7,p);
                    
                    
                    if(dist < EPSILON)
                    {
                        return depth;
                    }

                    depth += dist;

                    if(depth >= end)
                    {
                        return end;
                    }
                }
                return end;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenSize = float2(_ScreenParams.x,_ScreenParams.y);
                float2 screenPos = float2(screenSize.x * i.uv.x, screenSize.y * i.uv.y);
                float3 dir = rayDirection(45.0, screenSize, screenPos);

                
                //float3 eye = float3(0.0,0.0,-5.0);
                float3 eye = float3(0,0,0);
                
                float dist = shortestDistanceToSurface(eye,dir,MIN_DIS,MAX_DIS);
    
                fixed4 col = float4(1.0,0.0,0.0,1.0);
                if(dist > MAX_DIS - EPSILON)
                {
                    col = float4(0.0,0.0,0.0,0.0);
                }
            
                return col;
            }
            ENDCG
        }
    }
}
