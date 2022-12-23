Shader "Ayy/Decal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
            "RenderType"="Transparent"  // ??不懂...
            "RenderQueue"="Geometry+1"  // 必须注意绘制顺序
            "DisableBatching" = "True"  // 必须关闭。1个的时候肯跟体现不出来？会导致 矩阵有可能是不符合预期的
            "LightMode" = "ForwardBase" // 用处没体会到
        }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Off       // ZTest不关闭的话，会导致无法显示
            Cull Front      // 不 Cull Front 的话，默认 cull back ,相机进入立方体之后，就不显示内容了
            
            
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
                // 懂了！ 如果从 我这个盒子里面看， 深度图反映出来的世界坐标，如果在我这个盒子里面，那我给你画出来
                // 没在我这个盒子里面，我就不给你画
                // 所以，就是说，看深度图，所反应的世界坐标，转换成盒子的本地坐标之后， 是否在盒子内部
                // 又因为 盒子的 mesh 边长是1，mesh 中心点在 中间，
                // 所以 用 0.5 和 坐标做比较，即可比较出来， 该坐标是否在盒子内部 
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
