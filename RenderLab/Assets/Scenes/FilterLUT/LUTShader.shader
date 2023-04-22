Shader "ayy/LUTShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _LutTex ("LUT Texture",2D) = "white" {}
        _Factor ("factor",Range(0,1)) = 1
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


            #define X_BLOCK_NNUM 8.0
            #define Y_BLOCK_NNUM 8.0
            #define BLOCK_PIXELS 64.0
            

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

            sampler2D _LutTex;
            float4 _LUT_TexelSize;
            float _Factor;

            const float kTextureSize = 512.0;
            const float kGridSize = 64.0;
            const float kGridCntPerSide = 8.0;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
            	o.uv = v.uv;
                return o;
            }

			
            float2 getGrid(float gridIndex)
            {
            	float2 grid;
            	grid.y = floor(gridIndex / 8.0);
            	grid.x = gridIndex - grid.y * 8.0;
            	return grid;
            }

            float2 getGridPixelIndex(float2 grid)
            {
	            return float2(grid.x * 64.0,grid.y * 64.0);
            }

            float2 getOffsetPixelIndex(float4 col)
            {
				float2 offset = float2(0.0,0.0);
            	offset.x = floor(col.r * 63.0);
            	offset.y = floor(col.g * 63.0);
            	return offset;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
            	
            	float blueColor = col.b * 63.0;

            	float2 grid1 = getGrid(floor(blueColor));
            	float2 pixelIndex = getGridPixelIndex(grid1);
            	float2 pixelIndexOffset = getOffsetPixelIndex(col);
            	pixelIndex = pixelIndex + pixelIndexOffset + float2(0.5,0.5);
            	float2 uv1 = float2(pixelIndex.x / 512.0,pixelIndex.y / 512.0);

            	float4 lutCol1 = tex2D(_LutTex,uv1);
             	//float2 quad1 = getGrid(floor(blueColor));
				//float2 uv1 = getUVByGrid(quad1,col);
             	//float4 lutCol1 = tex2D(_LutTex,uv1);

            	/*
             	float2 quad2 = getGrid(ceil(blueColor));
				float2 uv2 = getUVByGrid(quad2,col);
             	float4 lutCol2 = tex2D(_LutTex,uv2);
             	*/

            	return lutCol1;

            	//return lerp(lutCol1,lutCol2,frac(blueColor));
            	//return float4(1.0,1.0,0.0,1.0);
            	
            	//return fixed4(i.uv.x,i.uv.y,0.0,1.0);
            }
            ENDCG
        }
    }
}
