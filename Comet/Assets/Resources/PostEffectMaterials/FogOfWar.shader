Shader "Unlit/FogOfWar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TerrainDepthTexture ("Texture",2D) = "white" {}
        
        // x: grid columns,y: grid rows, 
        _TerrainSizeParam ("Terrain Size",Vector) = (0,0,0,0)
        //_CameraFarPlane ("Camera Far Plane",Float) = 0
        
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
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            //float4 _MainTex_ST;
            
            sampler2D _CameraDepthTexture;
            sampler2D _TerrainDepthTexture;

            float4 _TerrainSizeParam;
            float _CameraFarPlane;
            float4x4 _CameraInvProjMatrix;
            float4x4 _CameraViewToWorldMatrix;

            float4x4 _CameraWorldToViewMatrix;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            

            float GetDepth(sampler2D depthTexture,float2 screenUV)
            {
                float4 depthTexCol = tex2D(depthTexture,screenUV);
                float depth = 1.0 - Linear01Depth(UNITY_SAMPLE_DEPTH(depthTexCol));
                return depth;
            }

            float4 DepthToViewPosition(float2 screenUV)
            {
                float depth = GetDepth(_TerrainDepthTexture,screenUV);
                float2 ndcPos = screenUV * 2 - 1;    // map [0,1] => [-1,+1]
                float3 clipPos = float3(ndcPos.x,ndcPos.y,1) * _CameraFarPlane; // z = far plane = mvp result w

                float4 viewPos = mul(_CameraInvProjMatrix,clipPos.xyzz) * depth;
                return viewPos/viewPos.w;
            }

            float4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 viewPos = DepthToViewPosition(i.uv);
                //float4 worldPos = mul(_CameraViewToWorldMatrix,viewPos);
                //return viewPos;

                return col;
            }
            ENDCG
        }
    }
}
