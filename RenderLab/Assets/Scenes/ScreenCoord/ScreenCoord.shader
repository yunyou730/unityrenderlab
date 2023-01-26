Shader "Ayy/ScreenCoord"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent"}
        LOD 100

        Pass
        {
            ZWrite Off
            
            CGPROGRAM
            
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


                float4 sPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                /*
                float4 clipPos = o.vertex / o.vertex.w;
                clipPos = clipPos * 0.5 + 0.5;
                o.sPos.xy = clipPos.xy;
                o.sPos.y = 1.0 - o.sPos.y;
                */
                
                
                o.sPos = ComputeScreenPos(o.vertex)/o.vertex.w;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(i.sPos.x,i.sPos.y,0.0,1.0);
                return col;
            }
            ENDCG
        }
    }    
    
}
