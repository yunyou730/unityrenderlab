Shader "ayy/Decal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque"
            "Queue" = "Geometry+1"
        }
        LOD 100
        
        ZWrite Off 
        ZTest Off
        
        
        //Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        

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
            float4 _MainTex_ST;

            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.screenPos = ComputeScreenPos(o.vertex);
                //o.screenPos.xy = o.screenPos.xy / o.screenPos.w;
                
                return o;
            }

            float3 DepthToWorldPosition(float4 screenPos)
            {
                float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,screenPos)));
                float4 ndcPos = (screenPos/screenPos.w) * 2 - 1;    // map [0,1] => [-1,+1]
                float3 clipPos = float3(ndcPos.x,ndcPos.y,1) * _ProjectionParams.z; // z=1,far plane
                float3 viewPos = mul(unity_CameraInvProjection,clipPos.xyzz).xyz * depth;
                float3 worldPos = mul(UNITY_MATRIX_I_V,float4(viewPos,1)).xyz;
                return worldPos;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 color = fixed4(0.0,0.0,0.0,1.0);
                float3 worldPos = DepthToWorldPosition(i.screenPos);
                float4 localPos = mul(unity_WorldToObject,float4(worldPos,1.0));
                clip(float3(0.5,0.5,0.5) - abs(localPos.xyz)); // cube mesh data ,center at (0,0,0),side len = 1.0

                fixed2 decalUV = fixed2(localPos.x,localPos.z);
                decalUV = decalUV + 0.5;
                color = tex2D(_MainTex,decalUV);
                
                //color = fixed4(localPos.xyz,1.0);
                return color;
            }
            ENDCG
        }
    }
}
