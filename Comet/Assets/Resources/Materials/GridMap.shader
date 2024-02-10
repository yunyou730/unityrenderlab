Shader "Comet/GridMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        //_BlockerAndHeightDataTex("Blocker Height Texture",2D) = "white" {}
        _MapGridsDataTex("Grids Texture",2D) = "white" {}
        _MapPointsDataTex("Points Texture",2D) = "white" {}
        
        //_TerrainLayer_0("Terrain Layer 0",2D) = "white" {}
        //_TerrainLayer_1("Terrain Layer 1",2D) = "white" {}
        
        _TerrainTextureGround("Terrain Texture, Ground",2D) = "white" {}
        _TerrainTextureGrass("Terrain Texture, Grass",2D) = "white" {}
        
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
                float2 uv3 : TEXCOORD2;
                float4 vertColor : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;      // uv: uv in each grid
                float4 vertex : SV_POSITION;
                float4 vertColor : COLOR;
                float2 uv2 : TEXCOORD1;     // uv2: uv in whole map mesh, in gridRows x gridCols
                float2 uv3 : TEXCOORD2;     // uv3: uv in whole map mesh, in pointRows x pointCols
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _MapGridsDataTex;
            sampler2D _MapPointsDataTex;
            
            //sampler2D _TerrainLayer_0;
            //sampler2D _TerrainLayer_1;
            
            float4 _MapGridsDataTex_TexelSize;
            float4 _MapPointsDataTex_TexelSize;
            
            sampler2D _TerrainTextureGround;
            sampler2D _TerrainTextureGrass;
            
            float _TOGGLE_GRID_LINE;
            float _TOGGLE_WALKABLE;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                o.vertColor = v.vertColor;
                o.uv2 = v.uv2;
                o.uv3 = v.uv3;
                return o;
            }
            
            float2 TileSetIndexToUV(float2 fromUV,int tileSetIndex)
            {
                /*
                 from [0,1] , depend on tileSetIndex, map to , 

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
                int modTileSetIndex = 3 - tileSetIndex % 4;
                int divTileSetIndex = tileSetIndex / 4;
                
                float2 uv = fromUV;
                uv.x *= 1.0/8.0;
                uv.y *= 1.0/4.0;
                uv.x += (float)divTileSetIndex * 1.0/8.0;
                uv.y += (float)modTileSetIndex * 1.0/4.0;
                
                return uv;
            }

//             int GetTileSetIndex(sampler2D tex, float2 uvByGrid,float2 uvByPoints)
//             {
//                 //float2 gridTexelSize = _BlockerAndHeightDataTex_TexelSize;
//                 
//                 float2 pointTexelSize = _MapPointsDataTex_TexelSize;
//
//                 float westNorth = tex2D(tex,uvByPoints + float2(-pointTexelSize.x,pointTexelSize.y));
//                 float eastNorth = tex2D(tex,uvByPoints + float2(pointTexelSize.x,pointTexelSize.y));
//                 float westSouth = tex2D(tex,uvByPoints + float2(-pointTexelSize.x,-pointTexelSize.y));
//                 float eastSouth = tex2D(tex,uvByPoints + float2(pointTexelSize.x,-pointTexelSize.y));
//                 
//                 /*
//                 float westNorth = tex2D(tex,uvByGrid + float2(-gridTexelSize.x,gridTexelSize.y));
//                 float eastNorth = tex2D(tex,uvByGrid + float2(gridTexelSize.x,gridTexelSize.y));
//                 float westSouth = tex2D(tex,uvByGrid + float2(-gridTexelSize.x,-gridTexelSize.y));
//                 float eastSouth = tex2D(tex,uvByGrid + float2(gridTexelSize.x,-gridTexelSize.y));
//
//                 float north = tex2D(tex,uvByGrid + float2(0,gridTexelSize.y));
//                 float south = tex2D(tex,uvByGrid + float2(0,-gridTexelSize.y));
//                 float west = tex2D(tex,uvByGrid + float2(-gridTexelSize.x,0));
//                 float east = tex2D(tex,uvByGrid + float2(gridTexelSize.x,0));
//                 */
//                 
//                 
//                 int blockIndex = 0;
//                 if(westNorth.r > 0.5)
//                 {
//                     blockIndex += 2;
//                 }
//                 if(eastNorth.r > 0.5)
//                 {
//                     blockIndex += 1;
//                 }
//                 if(westSouth.r > 0.5)
//                 {
//                     blockIndex += 8;
//                 }
//                 if(eastSouth.r > 0.5)
//                 {
//                     blockIndex += 4;
//                 }
//                 //   texture's G channel, saved the pre - randomized tileset index, so we use the G channel
//                 if(blockIndex >= 15)
//                 {
//                     blockIndex = ceil(tex2D(tex,uvByGrid).g * 100.0f);  // @miao @temp
//                 }
//
//                 //blockIndex = ceil(tex2D(tex,uv).g * 100.0f);
//                 return blockIndex;
//             }

            int RemapTileSetIndex(int tileSetIndex,sampler2D dataTexture,float2 uvByGrid)
            {
                if(tileSetIndex == 15)
                {
                //     tileSetIndex = floor(tex2D(dataTexture,uvByGrid).g * 100.0f);
                }
                return tileSetIndex;
            }

            fixed4 sampleTerrainTexture(v2f i)
            {
                // layer 0: whether we have ground 
                // layer 1: whether we have grass 
                //fixed4 terrainLayer0Data = tex2D(_TerrainLayer_0,i.uv2);        
                //fixed4 terrainLayer1Data = tex2D(_TerrainLayer_1,i.uv2);
                //fixed4 terrainTextureData = tex2D(_MapPointsDataTex,i.uv3);
                
                fixed4 col = float4(0,0,0,1);


                // Ground
                // make flat pattern ,not pure one part of terrain texture 
                int tileSetIndex = 15;
                tileSetIndex = RemapTileSetIndex(tileSetIndex,_MapGridsDataTex,i.uv2);
                
                float2 tileSetUV = TileSetIndexToUV(i.uv,tileSetIndex);
                float4 terrainColor = tex2D(_TerrainTextureGround,tileSetUV);
                col = terrainColor * terrainColor.a + col * (1 - terrainColor.a);
                
                // Grass
                const float2 centerPointUV = i.uv3;
                float2 nw = centerPointUV + float2(0, _MapPointsDataTex_TexelSize.y);
                float2 ne = centerPointUV + float2(_MapPointsDataTex_TexelSize.x, _MapPointsDataTex_TexelSize.y);
                float2 se = centerPointUV + float2(_MapPointsDataTex_TexelSize.x,0);
                float2 sw = centerPointUV + float2(0,0);
                
                tileSetIndex = 0;
                if(tex2D(_MapPointsDataTex,nw).r > 0.5)
                {
                    tileSetIndex += 2;
                }
                if(tex2D(_MapPointsDataTex,ne).r > 0.5)
                {
                    tileSetIndex += 1;
                }
                if(tex2D(_MapPointsDataTex,sw).r > 0.5)
                {
                    tileSetIndex += 8;
                }
                if(tex2D(_MapPointsDataTex,se).r > 0.5)
                {
                    tileSetIndex += 4;
                }

                // make flat pattern ,not pure one part of terrain texture
                tileSetIndex = RemapTileSetIndex(tileSetIndex,_MapPointsDataTex,i.uv2);   
                
                if(tileSetIndex > 0)
                {
                    // Grass
                    float2 tileSetUV = TileSetIndexToUV(i.uv,tileSetIndex);
                    float4 terrainColor = tex2D(_TerrainTextureGrass,tileSetUV);
                    col = terrainColor * terrainColor.a + col * (1 - terrainColor.a);
                }
                
                
                return col;
            }

            fixed4 showWalkableColor(fixed4 col,fixed4 blockerAndHeightData)
            {
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
                return col;
            }

            fixed4 showGridLineColor(fixed4 col,float2 uvInGrid)
            {
                const float kBorderSize = 0.05;
                if(_TOGGLE_GRID_LINE > 0.5)
                {
                    if(uvInGrid.x < kBorderSize || uvInGrid.y < kBorderSize || uvInGrid.x > 1-kBorderSize || uvInGrid.y > 1-kBorderSize)
                    {
                        col = fixed4(0,0,0,1);
                    }
                }
                return col;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 gridsData = tex2D(_MapGridsDataTex,i.uv2);
                // sample tileset texture
                fixed4 col = sampleTerrainTexture(i);
                // debug color, for walkable
                col = showWalkableColor(col,gridsData);
                // debug color, for grid border
                col = showGridLineColor(col,i.uv);
                
                return col;
            }
            ENDCG
        }
    }
}
