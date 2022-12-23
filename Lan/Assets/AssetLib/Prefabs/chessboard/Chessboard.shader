Shader "_lan/Chessboard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RowCnt ("Row Count",Float) = 5
        _ColCnt ("Col Count",Float) = 5
        
        _Color1 ("Color1",Color) = (0,0,0,1)
        _Color2 ("Color2",Color) = (1,1,1,1)
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _RowCnt;
            float _ColCnt;
            
            float4 _Color1;
            float4 _Color2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                //uv = frac(uv * 5.0);//float2(floor(i.uv.x * _ColCnt),floor(i.uv.y * _RowCnt));
                float2 cell = float2(floor(i.uv.x * _ColCnt),floor(i.uv.y * _RowCnt));

                float4 colors[2];
                colors[0] = _Color1;
                colors[1] = _Color2;
                
                int baseColorIdx = 0;
                if(cell.x % 2 == 1)
                {
                    baseColorIdx = 1;
                }

                int resultColorIdx = baseColorIdx;
                if(cell.y % 2 == 1)
                {
                    resultColorIdx = 1 - baseColorIdx;
                }
                
                return colors[resultColorIdx];
                
            }
            ENDCG
        }
    }
}
