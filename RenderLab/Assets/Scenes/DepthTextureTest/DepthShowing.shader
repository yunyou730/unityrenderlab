Shader "Ayy/DepthTextureTest/DepthShowing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "true"
            "DisableBatching" = "true"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members screenPos)
#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float2 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                float4 screenPos = ComputeScreenPos(o.vertex);
                o.screenPos = screenPos.xy / screenPos.w;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                float2 uv = i.screenPos;//i.uv;
                float2 screenPos = i.screenPos;

                float4 depthColor = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,uv);
                float depth = depthColor.r;
                float linearDepth = LinearEyeDepth(depth);//Linear01Depth(depth); // ??
                col = float4(linearDepth,0.0,0.0,1.0);


                fixed4 clipPos = fixed4(screenPos.x * 2 - 1,screenPos.y * 2 - 1,-depth * 2 + 1,1) * linearDepth;
                float4 viewPos = mul(unity_CameraInvProjection,clipPos);
                float4 worldPos = mul(unity_MatrixInvV,viewPos);
                float3 objectPos = mul(unity_WorldToObject,worldPos).xyz;


                //objectPos = frac(objectPos);
                // clip(0.5 - abs(objectPos));

                clip(0.5);
                objectPos += 0.5;
                // objectPos = objectPos * 0.5 + 0.5;
                col = tex2D(_MainTex,objectPos.xy);

                return col;
            }
            ENDCG
        }
    }
}
