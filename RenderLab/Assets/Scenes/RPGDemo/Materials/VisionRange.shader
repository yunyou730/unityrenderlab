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
            float4x4 _depthCameraProjMatrix;
            float3 _depthCameraPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                return o;
            }

            float getDepthValueByDepthTex(float4 worldPos)
            {
                float4 p1 = mul(_depthCameraProjMatrix,mul(_depthCameraViewMatrix,worldPos));
                p1 = p1 / p1.w;
                p1 = p1 * 0.5 + 0.5;

                // p1.y = 1.0 - p1.y;
                float depth = tex2D(_DepthTex,p1.xy).r;
                depth = Linear01Depth(depth);

                return depth;
            }

            
            float getDepthValueByWorldPos(float4 worldPos)
            {
                float4 p1 = mul(_depthCameraProjMatrix,mul(_depthCameraViewMatrix,worldPos));
                p1 = p1 / p1.w;
                p1 = p1 * 0.5 + 0.5;

                float depth = p1.z;
                //depth = Linear01Depth(depth);
                return depth;
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

                float depthValueFromDepthTex = getDepthValueByDepthTex(i.worldPos);
                float v1 = depthValueFromDepthTex;
                float v2 = getDepthValueByWorldPos(i.worldPos);
                if(v2 > depthValueFromDepthTex)
                {
                    discard;
                }
                
                //return float4(1.0,1.0,0.3,0.5);

                return float4(0.0,v1,0.0,1.0);
            }
            ENDCG
        }
    }
}
