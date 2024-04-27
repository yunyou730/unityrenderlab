Shader "Ayy/PremultiplyAlphaTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        [Enum(UnityEngine.Rendering.BlendMode)]
        _SrcBlend ("Src Blend",Float) = 1
        
        [Enum(UnityEngine.Rendering.BlendMode)]
        _DstBlend ("Dest Blend",Float) = 0
        
        [Enum(UnityEngine.Rendering.BlendOp)]
        _BlendOp ("Blend Op",Float) = 0
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent" }
        LOD 100
        
        //Blend SrcAlpha OneMinusSrcAlpha 
        Blend [_SrcBlend] [_DstBlend] 
        BlendOp [_BlendOp]

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(unity_MatrixMVP,v.vertex);
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                //col.a = 0.5;

                fixed4 uvColor = fixed4(i.uv.x,i.uv.y,0.0,1.0);
                fixed4 result = lerp(uvColor,col,col.a);
                
                //return fixed4(col.r,col.g,col.b,1.0);
                return col;
            }
            ENDCG
        }
    }
}
