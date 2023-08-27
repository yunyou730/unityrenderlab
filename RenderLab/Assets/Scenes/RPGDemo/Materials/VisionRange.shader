Shader "ayy/rpg/VisionRange"
{
    Properties
    {
        _Angle("Angle",float) = 0.785 // PI * 1/4
        _FrontDir("FrontDir",Vector) = (1,0,0,0)
        _DepthTex ("DepthTex", 2D) = "green" {}
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        Tags {"Queue" = "Transparent"}
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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
                float4 worldPos : SV_Target0;
            };

            //sampler2D _MainTex;
            //float4 _MainTex_ST;
            
            float _Angle;
            float4 _FrontDir;

            sampler2D _DepthTex;

            
            float4x4 _depthCameraViewMatrix;
            float4x4 _depthCameraInvViewMatrix;
            float4x4 _depthCameraProjMatrix;
            float4x4 _depthCameraInvProjMatrix;
            float4 _depthCameraPos;
            float _depthCameraNear;
            float _depthCameraFar;
            float _depthCameraFovY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                return o;
            }

            
            float2 getDepthTexUVByWorldPos(float4 worldPos)
            {
                float4 p1 = mul(_depthCameraProjMatrix,mul(_depthCameraViewMatrix,worldPos));
                p1 = p1 / p1.w;
                p1 = p1 * 0.5 + 0.5;        // uv
                return p1;
            }

            float getDepthValueByDepthTex(float4 worldPos)
            {
                float2 depthUV = getDepthTexUVByWorldPos(worldPos);   
                float depth = SAMPLE_DEPTH_TEXTURE(_DepthTex,depthUV);
                depth = Linear01Depth(depth);
                return depth;
            }
            
            float getDepthValueByDepthTex2(float4 worldPos)
            {
                float2 depthUV = getDepthTexUVByWorldPos(worldPos);   
                float depth = SAMPLE_DEPTH_TEXTURE(_DepthTex,depthUV);
                //depth = Linear01Depth(depth);
                return depth;
            }
            
            
            float getDepthValueByWorldPos(float4 worldPos)
            {
                float4 p1 = mul(_depthCameraProjMatrix,mul(_depthCameraViewMatrix,worldPos));
                p1 = p1 / p1.w;
                p1 = p1 * 0.5 + 0.5;

                float depth = p1.z;
                // depth = Linear01Depth(depth);
                return depth;
            }

            float getDepthValueByWorldPos2(float4 worldPos)
            {
                float4 p1 = mul(_depthCameraProjMatrix,mul(_depthCameraViewMatrix,worldPos));
                p1 = p1 / p1.w;
                p1 = p1 * 0.5 + 0.5;

                float depth = p1.z;
                //depth = Linear01Depth(depth);
                //depth = LinearEyeDepth(depth);
                return depth;
            }
            
            
            /*
             *
            float3 DepthToWorldPosition(float4 screenPos)
            {
                float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,screenPos)));
                float4 ndcPos = (screenPos/screenPos.w) * 2 - 1;    // map [0,1] => [-1,+1]
                float3 clipPos = float3(ndcPos.x,ndcPos.y,1) * _ProjectionParams.z; // z = far plane = mvp result w

                float3 viewPos = mul(unity_CameraInvProjection,clipPos.xyzz).xyz * depth;
                float3 worldPos = mul(UNITY_MATRIX_I_V,float4(viewPos,1)).xyz;
                return worldPos;
            }
             * 
             */

            float3 getWorldPosByDepthTexture(float linearDepth,float3 worldPos)
            {
                float4 ndcPos = mul(_depthCameraProjMatrix,mul(_depthCameraViewMatrix,worldPos));
                ndcPos = ndcPos / ndcPos.w;
                float3 clipPos = float3(ndcPos.x,ndcPos.y,1) * _depthCameraFar; // z = far plane = mvp result w
                
                float3 viewPos = mul(_depthCameraInvProjMatrix,clipPos.xyzz).xyz * linearDepth;
                float3 newWorldPos = mul(_depthCameraInvViewMatrix,float4(viewPos,1)).xyz;
                
                return newWorldPos;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // test matrix
                // return float4(_depthCameraProjMatrix[0]);
                
                // test depth texture
                //float2 originUV = i.uv;
                // float4 colorFromRT = tex2D(_DepthTex,originUV);
                // return float4(colorFromRT.rgb,1.0);
                
                
                float2 uv = i.uv * 2.0 - 1.0;
                
                float2 frontDirIn2D = normalize(float2(_FrontDir.x,_FrontDir.z));
                float2 uvDir = normalize(uv);
                float dotValue = dot(uvDir,frontDirIn2D);
                float angle = acos(dotValue);

                if(length(uv) > 1.0 || angle > _Angle * 0.5)
                {
                    discard;
                }

                float4 worldPos = i.worldPos / i.worldPos.w;

                // Linear depth value,hold in v1 
                float depthValueFromDepthTex = getDepthValueByDepthTex(worldPos);
                float v1 = depthValueFromDepthTex;


                v1 = getDepthValueByDepthTex2(i.worldPos);
                // depth value to compare
                float v2 = getDepthValueByWorldPos(worldPos);
                v2 = getDepthValueByWorldPos2(worldPos);

                if(v2 > 1 - v1)
                  discard;

                return float4(1 - v1,0,0,1);
                return float4(v2,0,0,1);
                //return float4(v2,0,0,1);
                

                // depth value compare
                // float2 depthUV = getDepthTexUVByWorldPos(i.worldPos);
                // depthUV = (depthUV * 2.0 - 1.0);
                //
                // float dirY = tan(_depthCameraFovY) * _depthCameraNear * depthUV.y;
                //
                // float fovx = _ScreenParams.y / _ScreenParams.y * _depthCameraFovY;
                // float dirX = tan(fovx) * _depthCameraNear * depthUV.x;
                // float dirZ = 1;
                //
                // float faceDir = normalize(float3(dirX,dirY,dirZ));
                // float3 worldPosByDepth = faceDir * v1 + _depthCameraPos.xyz;
                //
                // float dis1 = distance(worldPosByDepth,_depthCameraPos.xyz);
                // float dis2 = distance(i.worldPos.xyz,_depthCameraPos.xyz);
                // if(dis2 > dis1)
                // {
                //     discard;
                // }
                
                //return float4(0.0,v1,0.0,1.0);
                // return float4(v2 * 100,0.0,0.0,1.0);
                //
                // if(v2 > v1)
                //     discard;

                //float3 worldPosByDepthTex = getWorldPosByDepthTexture(v1,i.worldPos.xyz);
                //return float4(normalize(worldPosByDepthTex),1.0);
                //return float4(0,0,1,1);
            }
            ENDCG
        }
    }
}
