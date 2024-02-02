Shader "Comet/GridMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _BlockerAndHeightDataTex("Blocker Height Texture",2D) = "white" {}
        _TerrainLayer_0("Terrain Layer 0",2D) = "white" {}
        _TerrainLayer_1("Terrain Layer 1",2D) = "white" {}
        
        _TerrainGround("Texture",2D) = "white" {}
        _TerrainGrass("Texture",2D) = "white" {}
        
        [Toggle(ENABLE_GRID_LINE)] _TOGGLE_GRID_LINE("Toggle Grid Line",Float) = 1
        [Toggle(ENABLE_SHOW_WALKABLE)] _TOGGLE_WALKABLE("Toggle Show Block",Float) = 0
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
                float2 uv2 : TEXCOORD1;
                float4 vertColor : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;      // uv in grid
                float4 vertex : SV_POSITION;
                float4 vertColor : COLOR;
                float2 uv2 : TEXCOORD1;     // uv in whole map mesh
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _BlockerAndHeightDataTex;
            sampler2D _TerrainLayer_0;
            sampler2D _TerrainLayer_1;
            float4 _BlockerAndHeightDataTex_TexelSize;
            
            sampler2D _TerrainGround;
            sampler2D _TerrainGrass;
            
            float _TOGGLE_GRID_LINE;
            float _TOGGLE_WALKABLE;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertColor = v.vertColor;
                o.uv2 = v.uv2;
                return o;
            }

            float RandomRange(float minValue, float maxValue, float seed)
            {
                // seedn
                float random = frac(sin(seed) * 43758.5453);
                
                // mapping into [min,max] range
                float range = maxValue - minValue;
                float value = random * range + minValue;
                
                return value;
            }

            float2 terrainBlockIndexToUV(float2 fromUV,int blockIndex)
            {
                /*
                 from [0,1] , map to : 

                 0: x [0,1/8], y [3/4,1]
                 1: x [0,1/8], y [1/2,3/4]
                 2: x [0,1/8], y [1/4,1/2]
                 3: x [0,1/8], y [0,1/4]

                 4: x [1/8,2/8], y [3/4,1]
                 5: x [1/8,2/8], y [1/2,3/4]
                 6: x [1/8,2/8], y [1/4,1/2]
                 7: x [1/8,2/8], y [0,1/4]

                 8: x [2/8,3/8], y [3/4,1]
                 9: x [2/8,3/8], y [1/2,3/4]
                 10: x [2/8,3/8], y [1/4,1/2]
                 11: x [2/8,3/8[, y [0,1/4]

                 12: x [3/8,4/8], y [3/4,1]
                 13: x [3/8,4/8], y [1/2,3/4]
                 14: x [3/8,4/8], y [1/4,1/2]
                 15: x [3/8,4/8[, y [0,1/4]
                */
                int modBlockIndex = 3 - blockIndex % 4;
                int divBlockIndex = blockIndex / 4;
                
                float2 uv = fromUV;
                uv.x *= 1.0/8.0;
                uv.y *= 1.0/4.0;

                uv.x += (float)divBlockIndex * 1.0/8.0;
                uv.y += (float)modBlockIndex * 1.0/4.0;
                
                return uv;
            }

            int GetLayerBlockIndex(sampler2D tex, float2 uv)
            {
                float2 dataTexelSize = _BlockerAndHeightDataTex_TexelSize;
                // float2 westNorthDataUV = uv + float2(-dataTexelSize.x,dataTexelSize.y);
                // float2 eastNorthDataUV = uv + float2(dataTexelSize.x,dataTexelSize.y);
                // float2 westSouthDataUV = uv + float2(-dataTexelSize.x,-dataTexelSize.y);
                // float2 eastSouthDataUV = uv + float2(dataTexelSize.x,-dataTexelSize.y);
                //
                // float2 northUV = uv + float2(0,dataTexelSize.y);
                // float2 southUV = uv + float2(0,-dataTexelSize.y);
                // float2 westUV = uv + float2(-dataTexelSize.x,0);
                // float2 eastUV = uv + float2(dataTexelSize.x,0);


                float westNorth = tex2D(tex,uv + float2(-dataTexelSize.x,dataTexelSize.y));
                float eastNorth = tex2D(tex,uv + float2(dataTexelSize.x,dataTexelSize.y));
                float westSouth = tex2D(tex,uv + float2(-dataTexelSize.x,-dataTexelSize.y));
                float eastSouth = tex2D(tex,uv + float2(dataTexelSize.x,-dataTexelSize.y));

                float north = tex2D(tex,uv + float2(0,dataTexelSize.y));
                float south = tex2D(tex,uv + float2(0,-dataTexelSize.y));
                float west = tex2D(tex,uv + float2(-dataTexelSize.x,0));
                float east = tex2D(tex,uv + float2(dataTexelSize.x,0));
                
                

                int blockIndex = 0;
                if(westNorth.r > 0.5)
                {
                    blockIndex += 2;
                }
                if(eastNorth.r > 0.5)
                {
                    blockIndex += 1;
                }
                if(westSouth.r > 0.5)
                {
                    blockIndex += 8;
                }
                if(eastSouth.r > 0.5)
                {
                    blockIndex += 4;
                }
                
                // 15 special rule
                if(north.r > 0.5
                    && south.r > 0.5
                    && west.r > 0.5
                    && east.r > 0.5)
                {
                    blockIndex = 15;
                }
                

                if(blockIndex >= 15)
                {
                    blockIndex = ceil(tex2D(tex,uv).g * 100.0f);
                }

                //blockIndex = ceil(tex2D(tex,uv).g * 100.0f);
                return blockIndex;
            }            

            fixed4 frag (v2f i) : SV_Target
            {
                const float kBorderSize = 0.05;
                
                fixed4 blockerAndHeightData = tex2D(_BlockerAndHeightDataTex,i.uv2);
                fixed4 terrainLayer0Data = tex2D(_TerrainLayer_0,i.uv2);
                fixed4 terrainLayer1Data = tex2D(_TerrainLayer_1,i.uv2);
                
                fixed4 col = float4(0,0,0,1);

                if(terrainLayer0Data.r > 0.5)
                {
                    int blockIndex = GetLayerBlockIndex(_TerrainLayer_0,i.uv2);
                    float2 blockUV = terrainBlockIndexToUV(i.uv,blockIndex);
                    float4 terrainCol = tex2D(_TerrainGround,blockUV);
                    col = terrainCol * terrainCol.a + col * (1 - terrainCol.a);
                }

                if(terrainLayer1Data.r > 0.5)
                {
                    int blockIndex = GetLayerBlockIndex(_TerrainLayer_1,i.uv2);
                    float2 blockUV = terrainBlockIndexToUV(i.uv,blockIndex);
                    float4 terrainCol = tex2D(_TerrainGrass,blockUV);
                    col = terrainCol * terrainCol.a + col * (1 - terrainCol.a);
                }                
                
                // walkable
                if(_TOGGLE_WALKABLE > 0.5)
                {
                    if(blockerAndHeightData.r > 0.5)
                    {
                        col *= fixed4(0,1,0,1);
                    }
                    else
                    {
                        col *= fixed4(1,0,0,1);
                    }
                }
                
                // grid border
                if(_TOGGLE_GRID_LINE > 0.5)
                {
                    if(i.uv.x < kBorderSize || i.uv.y < kBorderSize || i.uv.x > 1-kBorderSize || i.uv.y > 1-kBorderSize)
                    {
                        col = fixed4(0,0,0,1);
                    }
                }
                
                return col;
            }
            ENDCG
        }
    }
}
