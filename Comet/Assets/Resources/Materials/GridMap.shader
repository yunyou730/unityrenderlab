Shader "Comet/GridMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridStateTex("Texture",2D) = "white" {}
        
        _TerrainGround("Texture",2D) = "white" {}
        _TerrainGrass("Texture",2D) = "white" {}
        
        [Toggle(ENABLE_GRID_LINE)] _TOGGLE_GRID_LINE("Toggle Grid Line",Float) = 1
        [Toggle(ENABLE_SHOW_WALKABLE)] _TOGGLE_SHOW_WALKABLE("Toggle Show Block",Integer) = 0
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
            
            sampler2D _GridStateTex;
            sampler2D _TerrainGround;
            sampler2D _TerrainGrass;
            
            float4 _GridStateTex_TexelSize;
            
            float _TOGGLE_GRID_LINE;
            float _TOGGLE_SHOW_WALKABLE;

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

            int GetLayer0BlockIndex(float2 uv)
            {
                float2 dataTexelSize = _GridStateTex_TexelSize;
                float2 westNorthDataUV = uv + float2(-dataTexelSize.x,dataTexelSize.y);
                float2 eastNorthDataUV = uv + float2(dataTexelSize.x,dataTexelSize.y);
                float2 westSouthDataUV = uv + float2(-dataTexelSize.x,-dataTexelSize.y);
                float2 eastSouthDataUV = uv + float2(dataTexelSize.x,-dataTexelSize.y);

                int blockIndex = 0;
                if(tex2D(_GridStateTex,westNorthDataUV).g > 0.5)
                {
                    blockIndex += 2;
                }
                if(tex2D(_GridStateTex,eastNorthDataUV).g > 0.5)
                {
                    blockIndex += 1;
                }
                if(tex2D(_GridStateTex,westSouthDataUV).g > 0.5)
                {
                    blockIndex += 8;
                }
                if(tex2D(_GridStateTex,eastSouthDataUV).g > 0.5)
                {
                    blockIndex += 4;
                }

                // if(blockIndex >= 15)
                // {
                //     blockIndex = RandomRange(15,31,uv.x + uv.y);
                // }
                return blockIndex;
            }

            int GetLayer1BlockIndex(float2 uv)
            {
                float2 dataTexelSize = _GridStateTex_TexelSize;
                float2 westNorthDataUV = uv + float2(-dataTexelSize.x,dataTexelSize.y);
                float2 eastNorthDataUV = uv + float2(dataTexelSize.x,dataTexelSize.y);
                float2 westSouthDataUV = uv + float2(-dataTexelSize.x,-dataTexelSize.y);
                float2 eastSouthDataUV = uv + float2(dataTexelSize.x,-dataTexelSize.y);

                int blockIndex = 0;
                if(tex2D(_GridStateTex,westNorthDataUV).b > 0.5)
                {
                    blockIndex += 2;
                }
                if(tex2D(_GridStateTex,eastNorthDataUV).b > 0.5)
                {
                    blockIndex += 1;
                }
                if(tex2D(_GridStateTex,westSouthDataUV).b > 0.5)
                {
                    blockIndex += 8;
                }
                if(tex2D(_GridStateTex,eastSouthDataUV).b > 0.5)
                {
                    blockIndex += 4;
                }
                // if(blockIndex >= 15)
                // {
                //     blockIndex = RandomRange(15,31,uv.x + uv.y);
                // }
                return blockIndex;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float kBorderSize = 0.05;
                
                fixed4 data = tex2D(_GridStateTex,i.uv2);
                fixed4 col = float4(0,0,0,1);

                // terrrain texture ,layer 0, ground
                if(data.g > 0.5)
                {
                    int blockIndex = GetLayer0BlockIndex(i.uv2);
                    float2 uv = terrainBlockIndexToUV(i.uv,blockIndex);
                    col = tex2D(_TerrainGround,uv);
                }
                
                // terrain texture , layer 1, grass 
                if(data.b > 0.5)
                {
                    int blockIndex = GetLayer1BlockIndex(i.uv2);
                    float2 uv = terrainBlockIndexToUV(i.uv,blockIndex);

                    float4 layer1Color = tex2D(_TerrainGrass,uv);
                    col = layer1Color * layer1Color.a + col * (1-layer1Color.a);   // blend
                }

                // walkable
                if(_TOGGLE_SHOW_WALKABLE > 0.5)
                {
                    if(data.r > 0.5)
                    {
                        col = col * fixed4(1,1,0,1);
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
       
                
                

                //col = tex2D(_GridStateTex,i.uv2);
                //col = float4(col.b,col.b,col.b,1.0);
                
                return col;
            }
            ENDCG
        }
    }
}
