Shader "Ayy/CmdBufTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Texture", 2D) = "white" {}
        
        _DistortionFactor ("distorion factor", Range (0,1)) = 0.1
        _TimeScale ("time scale", Range (0,3)) = 0.2
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

            sampler2D _NoiseTex;
            float _DistortionFactor;
            float _TimeScale;
            //_Time float4 Time (t/20, t, t*2, t*3), use to animate things inside the shaders.
            //float4 _Time;
            
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

                float2 noiseUV = uv + frac(_Time.y * _TimeScale);//abs(sin(frac(_Time.y * 0.3)));
                float4 noiseCol = tex2D(_NoiseTex,noiseUV);

                float2 offset = (noiseCol - 0.5) * _DistortionFactor;
                float2 uv2 = uv + offset;
                fixed4 col = tex2D(_Ayy_GrabTexture,uv2);

                //col = float4(uv.x,uv.y,0.0,1.0);
                return col;
            }
            ENDCG
        }
    }
}
