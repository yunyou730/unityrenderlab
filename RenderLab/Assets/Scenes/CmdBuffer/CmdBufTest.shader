Shader "Ayy/CmdBufTest"
{
    Properties
    {
        _NoiseTex ("Texture", 2D) = "white" {}
        
        _DistortionFactor ("distorion factor", Range (0,1)) = 0.1
        _TimeScale ("time scale", Range (0,3)) = 0.2
    }
    SubShader
    {
        Tags { "Queue"="Transparent"}
        LOD 100

        Pass
        {
            ZWrite Off
            
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members screenPos)
#pragma exclude_renderers d3d11
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

            sampler2D _NoiseTex;
            float _DistortionFactor;
            float _TimeScale;
            
            
            sampler2D _Ayy_GrabTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex)/o.vertex.w;
                return o;
            }

            // Testcase 1 , pure color
            // fixed4 frag (v2f i) : SV_Target
            // {
            //     float2 uv = i.screenPos.xy;
            //     fixed4 col = fixed4(uv.x,uv.y,0.0,1.0);
            //     return col;
            // }
            
            // Testcase 2, whole grab texture
            // fixed4 frag (v2f i) : SV_Target
            // {
            //     fixed4 col = tex2D(_Ayy_GrabTexture,i.uv);
            //     col *= fixed4(1.0,0.0,0.0,1.0);
            //     return col;
            // }

            // Testcase 3,grab texture with Screen UV
            // fixed4 frag (v2f i) : SV_Target
            // {
            //     float2 screenUV = i.screenPos.xy;
            //     fixed4 col = tex2D(_Ayy_GrabTexture,screenUV);
            //     col *= fixed4(1.0,0.0,0.0,1.0);
            //     return col;
            // }
            
            // Testcase , final result
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy;
            
                //_Time float4 Time (t/20, t, t*2, t*3), use to animate things inside the shaders.
            
                float2 noiseUV = uv + frac(_Time.y * _TimeScale);
                float4 noiseCol = tex2D(_NoiseTex,noiseUV);
            
                float2 offset = (noiseCol - 0.5) * _DistortionFactor;
                float2 uv2 = uv + offset;
                fixed4 col = tex2D(_Ayy_GrabTexture,uv2);
            
                return col;
            }
            ENDCG
        }
    }
}
