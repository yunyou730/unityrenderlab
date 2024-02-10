Shader "Unlit/MiniMap"
{
    Properties
    {
//        _MainTex ("Texture", 2D) = "white" {}
        _WalkableTex ("Walkable Texture",2D) = "white" {}
        //_TerrainLayer0Tex ("Terrain Layer 0",2D) = "white" {}
        //_TerrainLayer1Tex ("Terrain Layer 1",2D) = "white" {}
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
            
            sampler2D _WalkableTex;
            //sampler2D _TerrainLayer0Tex;
            //sampler2D _TerrainLayer1Tex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 walkableCol = tex2D(_WalkableTex, i.uv);
                //fixed4 layer0Col = tex2D(_TerrainLayer0Tex,i.uv);
                //fixed4 layer1Col = tex2D(_TerrainLayer1Tex,i.uv);

                float walkable = step(0.5,walkableCol.r);
                //float layer0 = step(0.5,layer0Col.r);
                //float layer1 = step(0.5,layer1Col.r);

                //float4 col = float4(walkable,layer0,layer1,1.0);
                float4 col = float4(walkable,walkable,walkable,1.0);
                return col;
            }
            ENDCG
        }
    }
}
