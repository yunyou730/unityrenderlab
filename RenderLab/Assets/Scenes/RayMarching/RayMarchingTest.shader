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

            float cubeSDF(float3 p)
            {
                float3 d = abs(p) - float3(1,1,1);
                float insideDistance = min(max(d.x,max(d.y,d.z)),0);
                float outsideDistance = length(max(d,0));
                return insideDistance + outsideDistance;
            }

            float intersectSDF(float distA,float distB)
            {
                return max(distA,distB);
            }

            float unionSDF(float distA,float distB)
            {
                return min(distA,distB);
            }

            float differenceSDF(float distA,float distB)
            {
                return max(distA,-distB);
            }

            float sceneSDF(float3 samplePoint)
            {
                
                //return sphereSDF(float3(0,0,0),1.0,samplePoint);
                //return sphereSDF(samplePoint);
                //return cubeSDF(samplePoint);

                //float distA = sphereSDF(float3(0,0,0),1.5,samplePoint);
                float distA = sphereSDF(samplePoint / 1.2) * 1.2;
                //float distB = cubeSDF(samplePoint + float3(0,sin(_Time.y),0));
                float distB = sphereSDF(samplePoint + float3(0,sin(_Time.y),0));
                return unionSDF(distA,distB);
                //return intersectSDF(distA,distB);
                //return differenceSDF(distA,distB);
            }
            
            float shortestDistanceToSurface(float3 eye,float3 marchingDir,float start,float end)
            {
                float depth = start;
                for(int i = 0;i < MAX_MARCHING_STEPS;i++)
                {
                    float3 p = eye + depth * marchingDir;
                    float dist = sceneSDF(p);
                    
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

            float3 estimateNormal(float3 p)
            {
                return normalize(float3(
                        sceneSDF(float3(p.x + EPSILON,p.y,p.z)) - sceneSDF(float3(p.x - EPSILON,p.y,p.z)),
                        sceneSDF(float3(p.x,p.y + EPSILON,p.z)) - sceneSDF(float3(p.x,p.y - EPSILON,p.z)), 
                        sceneSDF(float3(p.x,p.y,p.z + EPSILON)) - sceneSDF(float3(p.x,p.y,p.z - EPSILON)))
                );
            }

            float3 phongContribForLight(float3 k_d,float3 k_s,
                                        float alpha,
                                        float3 p,float3 eye,float3 lightPos,
                                        float3 lightIntensity)
            {
                float3 N = estimateNormal(p);
                float3 L = normalize(lightPos - p);
                float3 V = normalize(eye - p);
                float3 R = normalize(reflect(-L,N));

                float dotLN = dot(L,N);
                float dotRV = dot(R,V);
                
                if(dotLN < 0.0)
                {
                    // light not visible from this point on the surface
                    return float3(0,0,0);
                }

                if(dotRV < 0.0)
                {
                    // Light reflection in opposite direction as viewer, apply only diffuse component
                    return lightIntensity * (k_d * dotLN);
                }
                
                return lightIntensity * (k_d * dotLN + k_s * pow(dotRV,alpha));
            }

            float3 phongIllumination(float3 k_a,float3 k_d,float3 k_s,float alpha, float3 p,float3 eye)
            {
                float3 ambientLight = 0.5 * float3(1,1,1);
                float3 color = ambientLight * k_a;
                
                float3 light1Pos = float3(4 * sin(_Time.y),2,4 * cos(_Time.y));
                float3 light1Intensity = float3(0.4,0.4,0.4);
                color += phongContribForLight(k_d,k_s,alpha,p,eye,light1Pos,light1Intensity);

                float3 light2Pos = float3(2.0 * sin(0.37 * _Time.y),2.0 * cos(0.37 * _Time.y),2.0);
                float light2Intensity = float3(0.4,0.4,0.4);
                color += phongContribForLight(k_d,k_s,alpha,p,eye,light2Pos,light2Intensity);
                
                return color;
            }


            float4x4 viewMatrix(float3 eye,float3 center,float3 up)
            {
                float3 f = normalize(center - eye);
                float3 s = normalize(cross(f,up));
                float3 u = cross(s,f);

                return float4x4(
                    float4(s,0.0),
                    float4(u,0.0),
                    float4(f,0.0),
                    float4(0.0,0.0,0.0,1.0)
                    //float4(eye.x,eye.y,eye.z,1.0)
                );
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 resolution = float2(_ScreenParams.x,_ScreenParams.y);
                float2 fragCoord = float2(resolution.x * i.uv.x, resolution.y * i.uv.y);
                float3 viewDir = rayDirection(45.0, resolution, fragCoord);
                
                float3 eye = float3(3,10,8);
                //float3 eye = float3(8,3,7);
                
                float4x4 viewToWorld = viewMatrix(eye,float3(0,0,0),float3(0,1,0));
                //viewToWorld = inverse(viewToWorld);
                float3 worldDir = mul(viewToWorld,float4(viewDir,0)).xyz;
                float dist = shortestDistanceToSurface(eye,worldDir,MIN_DIS,MAX_DIS);
                
                if(dist > MAX_DIS - EPSILON)
                {
                    // hit nothing 
                    return float4(0.0,0.0,0.0,0.0);
                }

                // hit the ball
                float3 p = eye + dist * worldDir;
                
                float3 k_a = float3(0.2,0.2,0.2);
                float3 k_d = float3(0.7,0.2,0.2);
                float3 k_s = float3(1.0,1.0,1.0);
                float shininess = 10.0;
                float3 col = phongIllumination(k_a,k_d,k_s,shininess,p,eye);
                
                return float4(col,1.0);
            }
            ENDCG
        }
    }
}
