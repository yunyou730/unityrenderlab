Shader "ayy/SDF_2D_V2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Value ("LerpValue", Range(0.0,1.0)) = 0.0
        _EdgeWidth("EdgeWidth",Range(0.0,1.0)) = 0.1
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

            float _Value;
            float _EdgeWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float sdCircle(float2 p, float r )
            {
                return length(p) - r;
            }

            float sdBox( in float2 p, in float2 b )
            {
                float2 d = abs(p)-b;
                return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
            }


            float4 sdfEdgeTest(float sdf)
            {
                float f1 = step(sdf,0.0);
                float f2 = step(_EdgeWidth,sdf);
                float f3 = step(0.0,sdf) * step(sdf,_EdgeWidth);
                
                return float4(1,0,0,1) * f1
                        + float4(0,1,0,1) * f2
                        + float4(0,0,1,1) * f3;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // remap uv 
                float2 screenSize = _ScreenParams.xy;
                float2 uv = ((i.uv * 2.0 - 1.0) * screenSize.xy) / min(screenSize.x,screenSize.y);


                float sdf1 = sdCircle(uv,0.5);
                float sdf2 = sdBox(uv,float2(0.3,0.5));

                
                float sdf = lerp(sdf1,sdf2,_Value);
                
                //float v = step(sdf,0.01);
                //return float4(v,0.0,0.0,1.0);
                
                return sdfEdgeTest(sdf);
                
            }
            ENDCG
        }
    }
}
