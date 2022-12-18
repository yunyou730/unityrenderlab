Shader "Ayy/CmdBufTest2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent+100"}
        LOD 100

        Pass
        {
            ZWrite Off
            
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


                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _Ayy_GrabTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                // float4 clipPos = o.vertex / o.vertex.w;
                // clipPos = clipPos * 0.5 + 0.5;
                // o.screenPos.xy = clipPos.xy;

                o.screenPos = ComputeScreenPos(o.vertex)/o.vertex.w;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy;
                fixed4 col = tex2D(_Ayy_GrabTexture,uv);
                col *= fixed4(1.0,0.5,0.5,1.0);
                return col;
            }
            ENDCG
        }
    }
}
