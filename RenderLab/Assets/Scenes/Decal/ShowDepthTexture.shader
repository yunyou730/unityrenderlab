Shader "ayy/ShowDepthTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        ZTest Off

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

            
            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);

                
                float2 pos;
                if(v.vertex.x < 0)
                    pos.x = -1.0;
                if(v.vertex.x < 0)
                    pos.x = 1.0;
                if(v.vertex.y < 0)
                    pos.y = -1.0;
                if(v.vertex.y > 0)
                    pos.y = 1.0;
                //pos.x = sign(v.vertex.x);
                //pos.y = sign(v.vertex.y); 
                o.vertex = float4(pos.x,pos.y,0.0,1.0);//float4(sign(v.vertex.x),sign(v.vertex.y),0.0,1.0);

                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                float2 uv = float2(1.0-i.uv.x,1.0 - i.uv.y);

                //Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,screenPos)));
                
                float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,uv)));
                fixed4 col = fixed4(depth,0,0,1);

                //col = fixed4(1.0,1.0,0.0,1.0);
                //col = fixed4(i.uv.x,i.uv.y,0.0,1.0);
                return col;
            }
            ENDCG
        }
    }
}
