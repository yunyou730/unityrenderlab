Shader "Comet/GridMap"
{
    Properties
    {
        _MapGridsDataTex("Grids Texture",2D) = "white" {}
        _MapPointsDataTex("Points Texture",2D) = "white" {}
        
        _TerrainTextureGround("Terrain Texture - Ground",2D) = "white" {}
        _TerrainTextureGrass("Terrain Texture - Grass",2D) = "white" {}
        _TerrainTextureDirt("Terrain Texture - Dirt",2D) = "white" {}
        _TerrainTextureBlight("Terrain Texture -Blight",2D) = "white" {}
        
        [Toggle(ENABLE_GRID_LINE)] _TOGGLE_GRID_LINE("Toggle Grid Line",Float) = 1
        [Toggle(ENABLE_SHOW_WALKABLE)] _TOGGLE_WALKABLE("Toggle Show Block",Float) = 0
        
        [Toggle(_TOGGLE_GRID_UV)] _TOGGLE_GRID_UV("Toggle Show Grid UV",Float) = 0
        [Toggle(_TOGGLE_MAP_UV)] _TOGGLE_MAP_UV("Toggle Show Map UV",Float) = 0
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
        }
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
            
            sampler2D _MapGridsDataTex;
            sampler2D _MapPointsDataTex;
            
            float4 _MapGridsDataTex_TexelSize;
            float4 _MapPointsDataTex_TexelSize;
            
            sampler2D _TerrainTextureGround;
            sampler2D _TerrainTextureGrass;
            sampler2D _TerrainTextureDirt;
            sampler2D _TerrainTextureBlight;
            
            float _TOGGLE_GRID_LINE;
            float _TOGGLE_WALKABLE;
            float _TOGGLE_GRID_UV;
            float _TOGGLE_MAP_UV;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
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
            

            int MarchingQuadValue(sampler2D valueTex,float threshold,float2 nw,float2 ne,float2 se,float2 sw)
            {
                const float epsilon = 0.01;
                int v = 0;
                if(abs(tex2D(valueTex,nw).r - threshold) < epsilon)
                {
                    v += 2;
                }
                if(abs(tex2D(valueTex,ne).r - threshold) < epsilon)
                {
                    v += 1;
                }
                if(abs(tex2D(valueTex,sw).r - threshold) < epsilon)
                {
                    v += 8;
                }
                if(abs(tex2D(valueTex,se).r - threshold) < epsilon)
                {
                    v += 4;
                }
                return v;
            }

            fixed4 sampleTerrainTexture(v2f i)
            {
                fixed4 col = float4(0,0,0,1);
                
                // Base Layer: Ground
                int tileSetIndex = 15;
                float2 tileSetUV = TileSetIndexToUV(i.uv,tileSetIndex);
                float4 terrainColor = tex2D(_TerrainTextureGround,tileSetUV);
                col = terrainColor * terrainColor.a + col * (1 - terrainColor.a);
                
                // Center Layer: Grass or Dirt
                const float2 centerPointUV = i.uv3;
                float2 nw = centerPointUV + float2(0, _MapPointsDataTex_TexelSize.y);
                float2 ne = centerPointUV + float2(_MapPointsDataTex_TexelSize.x, _MapPointsDataTex_TexelSize.y);
                float2 se = centerPointUV + float2(_MapPointsDataTex_TexelSize.x,0);
                float2 sw = centerPointUV + float2(0,0);
                
                const int grassTileSetIndex = MarchingQuadValue(_MapPointsDataTex,0.1,nw,ne,se,sw);
                const int dirtTileSetIndex = MarchingQuadValue(_MapPointsDataTex,0.2,nw,ne,se,sw);
                const int blightTileSetIndex = MarchingQuadValue(_MapPointsDataTex,0.3,nw,ne,se,sw);
                
                if(grassTileSetIndex > 0)
                {
                    float2 tileSetUV = TileSetIndexToUV(i.uv,grassTileSetIndex);
                    float4 terrainColor = tex2D(_TerrainTextureGrass,tileSetUV);
                    col = terrainColor * terrainColor.a + col * (1 - terrainColor.a);                    
                }
                else if(dirtTileSetIndex > 0)
                {
                    float2 tileSetUV = TileSetIndexToUV(i.uv,dirtTileSetIndex);
                    float4 terrainColor = tex2D(_TerrainTextureDirt,tileSetUV);
                    col = terrainColor * terrainColor.a + col * (1 - terrainColor.a);                    
                }


                // Top Layer , Blight 
                if(blightTileSetIndex > 0)
                {
                    float2 tileSetUV = TileSetIndexToUV(i.uv,blightTileSetIndex);
                    float4 terrainColor = tex2D(_TerrainTextureBlight,tileSetUV);
                    col = terrainColor * terrainColor.a + col * (1 - terrainColor.a);
                    //col = terrainColor;
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

            fixed4 showGridUV(fixed4 col,float2 uv)
            {
                if(_TOGGLE_GRID_UV > 0.5)
                {
                    col = float4(uv,0.0,1.0);
                }
                return col;
            }

            fixed4 showWholeMeshUV(fixed4 col,float2 uv)
            {
                if(_TOGGLE_MAP_UV > 0.5)
                {
                    col = float4(uv,0.0,1.0);
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
                // debug color for GridUV
                col = showGridUV(col,i.uv);
                // debug color for whole mesh UV 
                col = showWholeMeshUV(col,i.uv2);
                // debug color, for grid border
                col = showGridLineColor(col,i.uv);
                
                return col;
            }
            ENDCG
        }
        
        // The ShadowCaster pass ,for enable object write in DepthBuffer  
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            Cull Off
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
            #include "UnityCG.cginc"

            struct v2f
            {
                V2F_SHADOW_CASTER;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG

        }
    }
}
