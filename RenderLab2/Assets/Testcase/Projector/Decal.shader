Shader "Ayy/Decal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {

        LOD 100

        Pass
        {
            Tags {"RenderType"="Transparent" "RenderQueue"="Geometry+1" "DisableBatching" = "True"  "LightMode" = "ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Off 
            Cull Front      // notice here
            
            
            //Blend Off
//            ZWrite On
//            ZTest On
//            Cull Off
                        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


            sampler2D _CameraDepthTexture;
            
            float3 DepthToWorldPosition(float4 screenPos)
            {
                float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,screenPos)));
                float4 ndcPos = (screenPos/screenPos.w) * 2 - 1;    // map [0,1] => [-1,+1]
                float3 clipPos = float3(ndcPos.x,ndcPos.y,1) * _ProjectionParams.z;
                float3 viewPos = mul(unity_CameraInvProjection,clipPos.xyzz).xyz * depth;
                float3 worldPos = mul(UNITY_MATRIX_I_V,float4(viewPos,1)).xyz;
                return worldPos;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // pos 深度图，所表现出来的世界位置
                float3 pos = DepthToWorldPosition(i.screenPos);

                // localPos: 深度图，所表现出来的世界位置，相对于当前物体的位置.颜色表现了相对位置关系  
                float3 localPos = mul(unity_WorldToObject,float4(pos,1)).xyz;

                // 这里没看明白, 加上这一句之后，就能贴在地面上了
                clip(0.5 - abs(localPos));

                // decal UV 
                float2 decalUV = localPos.xz;   // range [-0.5,+0.5]
                decalUV = decalUV + 0.5;    // map from [-0.5,0.5] => [0,1]
                
                //return float4(pos,1.0);
                // return float4(decalUV,0.0,1.0);
                
                // sample texture
                float4 col = tex2D(_MainTex,decalUV);
                return col;   
            }
            ENDCG
        }
    }
}
