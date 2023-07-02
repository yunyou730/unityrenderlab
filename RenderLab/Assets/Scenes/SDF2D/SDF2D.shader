Shader "ayy/SDF2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MyFloat ("My Float", Range(0.0, 1.0)) = 1.0
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

            float _MyFloat;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float sdf_circle(float2 p, float r)
            {
                return length(p) - r;
            }

            float sdf_square(float2 p, float s)
            {
                float2 d = abs(p) - s;
                return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
            }

            
            // case1: basic shape,same radius,same center, only show sdf lerp value
            fixed4 case1(float2 uv)
            {
                float v1 = sdf_circle(uv,0.5);
                float v2 = sdf_square(uv,0.5);
            
                float v = lerp(v1,v2,_MyFloat);
                return fixed4(v,0,0,1); 
            }
            
            // case2: basic shape, different center, different radius, show sdf lerp value
            fixed4 case2 (float2 uv)
            {
                float v1 = sdf_circle(uv - float2(0.7,0.6),0.25);
                float v2 = sdf_square(uv - float2(-0.4,-0.1),0.3);
            
                float v = lerp(v1,v2,_MyFloat);
                return fixed4(v,0,0,1); 
            }


            // case3, case2 combine with game image
            fixed4 case3 (float2 uv,float4 imgCol)
            {
                //float v1 = sdf_circle(uv - float2(0.7,0.3),0.1);
                //float v2 = sdf_square(uv - float2(-0.4,-0.7),0.1);

                float v1 = sdf_circle(uv,0.3);
                float v2 = sdf_square(uv,0.08);
                                
                
                float v = lerp(v1,v2,_MyFloat);
                
                v = step(v,0);
                return lerp(float4(1,1,1,1),imgCol,v);
            }
                
            // case2 ,combine with game image
            fixed4 frag (v2f i) : SV_Target
            {
                // from i.uv u[0,1] v[0,1] => short side [-1,+1], long side [-l/s,+l/s]
                float2 screenSize = _ScreenParams.xy;
                float2 uv = (2.0 * i.uv * screenSize.xy - screenSize.xy)/min(screenSize.x,screenSize.y);

                
                float4 imgCol = tex2D(_MainTex,i.uv);
                
                return case1(uv);
                //return case2(uv);
                //return case3(uv,imgCol);
            }
            
            ENDCG
        }
    }
}
