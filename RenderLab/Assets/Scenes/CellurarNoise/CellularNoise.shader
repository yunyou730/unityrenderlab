Shader "ayy/CellularNoise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //_ScreenWidth ("ScreenWidth",Float) = 0.0
        //_ScreenHeight("ScreenHeight",Float) = 0.0
        _MousePosition("MousePosition",Vector) = (0.0,0.0,0.0,0.0)
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
            float4 _MainTex_TexelSize;
            
            float4 _MousePosition;

            //float4 _Time;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 UVStretchByY(float2 originUV)
            {
                // texelSize .z = width of texture size, .w = height of texture size
                float ratioWH = _MainTex_TexelSize.z / _MainTex_TexelSize.w;
                
                // map UV into y=>[0,1]
                // x=>[0,>1] when width > height; x=>[0,<1] when width < height
                float2 strechedUV = originUV;
                strechedUV.x *= ratioWH;    // y ->[0,1], x -> [0, >1]

                return strechedUV;
            }

            // random generator
            float2 random2(float2 p)
            {
                return frac(sin(float2(dot(p,float2(127.1,311.7)),dot(p,float2(0.400,0.970))))* 43758.5453);
            }
            
            /*
             *  Cellular Noise by fixed feature points
             */
            float4 cellularNoiseByFixedPoints(float2 uv)
            {
                const int POINT_NUM = 5;
                float2 featurePoints[POINT_NUM];
                featurePoints[0] = float2(0.83,0.75);
                featurePoints[1] = float2(0.60,0.64);
                featurePoints[2] = float2(0.28,0.64);
                featurePoints[3] = float2(0.31,0.26);
                featurePoints[4] = _MousePosition.xy;
                
                float3 col = float3(0.0,0.0,0.0);
                float minDis = 1.0;
                for(int i = 0;i < POINT_NUM;i++)
                {
                    float dist = distance(uv,featurePoints[i]);
                    minDis = min(minDis,dist);
                }

                col += minDis;
                return float4(col,1.0);
            }

            float4 cellularNoiseByTiling(float2 uv)
            {
                uv *= 3.0;
                
                float3 col = float3(0.0,0.0,0.0);

                float2 i_st = floor(uv);
                float2 f_st = frac(uv);

                float m_dist = 1.0;

                for(int y = -1;y <= 1;y++)
                {
                    for(int x = -1;x <= 1;x++)
                    {
                        float2 neighbor = float2(float(x),float(y));

                        // pt.x, pt.y range in [0,1]
                        float2 pt = random2(i_st + neighbor);

                        // pt range [0,1], but now it has relative with Time
                        pt = 0.5 + 0.5 * sin(_Time.y + 6.2823 * pt);
                        
                        float2 diff = neighbor + pt - f_st;
                        float dist = length(diff);
                        m_dist = min(m_dist,dist);
                    }
                }

                // draw cellular
                col += m_dist;

                // draw grid line
                col.r += step(0.98,f_st.x) + step(0.98,f_st.y);

                // draw feature points
                col.g += 1.0 - step(0.02,m_dist);
                
                return float4(col,1.0);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                //return cellularNoiseByFixedPoints(i.uv);
                return cellularNoiseByTiling(i.uv);
            }
            ENDCG
        }
    }
}
