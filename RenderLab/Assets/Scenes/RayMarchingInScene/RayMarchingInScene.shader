Shader "ayy/RayMarchingInScene"
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            
            float sphereSDF(float3 samplePoint)
            {
                return length(samplePoint) - 1.0;
            }

            float3 nearPlanePosInCameraSpace(float2 uv)
            {
                float z = _ProjectionParams.y;
                float fov = (60.0/360.0) * 3.1415;  // 60 temp hard code
                float nearPlaneHeight = tan(fov) * z * 2.0;
                float nearPlaneWidth = _ScreenParams.x / _ScreenParams.y * nearPlaneHeight;

                uv = 2 * uv - uv;
                float x = nearPlaneWidth * uv.x;
                float y = nearPlaneHeight * uv.y;
                
                return float3(x,y,z);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 nearPlanePos = nearPlanePosInCameraSpace(i.uv);
                float3 rayInCameraSpace = nearPlanePos;

                
                
                return 1 - col;
            }
            ENDCG
        }
    }
}
