Shader "ayy/CircleWithNoise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius",Float) = 0.7
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

            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // simple circle
            fixed4 shape1(float2 uv,fixed4 imgCol)
            {
                const float gradient = 0.04;
                float sq = sqrt(uv.x * uv.x + uv.y * uv.y);
                float blendFactor = smoothstep(_Radius,_Radius - gradient,sq);
                return lerp(imgCol,fixed4(1.0,1.0,1.0,1.0),blendFactor);
            }

            // circle with multiple sine 
            fixed4 shape2(float2 uv,fixed4 imgCol)
            {
                
                const float gradient = 0.04;

                const float maxOffset = 0.1;

                float sq = sqrt(uv.x * uv.x + uv.y * uv.y);
                
                float rad = atan2(uv.y,uv.x);
                float offset = sin(rad * 5.0) * maxOffset;
                
                float blendFactor = smoothstep(_Radius + offset,_Radius - gradient + offset,sq);
                return lerp(imgCol,fixed4(1.0,1.0,1.0,1.0),blendFactor);
            }

            float random(float x)
            {
                return frac(sin(x) * 10000.0);
            }

            // circle with noise
            fixed4 shape3(float2 uv,fixed4 imgCol)
            {
                const float maxOffset = 0.1;
                const float radScale = 3.0;

                float dis = sqrt(uv.x * uv.x + uv.y * uv.y);

                float rad = atan2(uv.y,uv.x) * radScale;
                
                float i = floor(rad);
                float f = frac(rad);
                f = f * f * (3 - 2 * f);
                float v = lerp(random(i),random(i + 1.0),f);
                float offset = v * maxOffset;

                float blendFactor = step(dis,_Radius + offset);
                return lerp(imgCol,fixed4(1.0,1.0,1.0,1.0),blendFactor);
            }
            
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenSize = _ScreenParams.xy;
                
                // from i.uv u[0,1] v[0,1] => short side [-1,+1], long side [-l/s,+l/s]
                float2 uv = (2.0 * i.uv * screenSize.xy - screenSize.xy)/min(screenSize.x,screenSize.y);
            
                fixed4 col = tex2D(_MainTex, i.uv);
                
                //return shape1(uv,col);
                //return shape2(uv,col);
                return shape3(uv,col);
            }
            
            /*
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.x += sin(uv.y * 10.0 + _Time.y * 0.3) / 10.0;

                if(uv.x < 0.0)
                {
                    uv.x = 1.0 + uv.x;
                }
                if(uv.x > 1.0)
                {
                    uv.x = uv.x - 1.0;
                }
                
                fixed4 col = tex2D(_MainTex,uv);
                return col;
            }
            */
            
            ENDCG
        }
    }
}
