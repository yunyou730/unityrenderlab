Shader "ayy/PointsInPoly"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
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

            const int maxPolyPoints = 500;
            int _polygonPointCount = 0;
            float _polygonPoints[1000];


            int _bFill = 1;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Drawing with fixed points
            /*
            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uv = i.uv;
                // define polygon
                const int len = 5;
                fixed2 polygon[len];
                polygon[0] = fixed2(0.0,0.7);
                polygon[1] = fixed2(0.7,0.7);
                polygon[2] = fixed2(0.6,0.1);
                polygon[3] = fixed2(0.2,0.3);
                polygon[4] = fixed2(0.0,0.0);
                
                const float y = uv.y;
                const float checkX = uv.x;

                int intersect = 0;
                int j = len - 1;
                for(int i = 0;i < len;i++)
                {
                    fixed2 p1 = polygon[j];
                    fixed2 p2 = polygon[i];

                    if((p1.y > y && p2.y < y) || (p1.y < y && p2.y > y))
                    {
                        float x = (y - p2.y) * (p1.x - p2.x) / (p1.y - p2.y) + p2.x;
                        //if(checkX < x)
                        if(checkX > x)
                        {
                            intersect++;
                        }    
                    }
                    
                    j = i;
                }
                intersect = intersect % 2;
                
                fixed4 col = tex2D(_MainTex,uv);
                if(intersect == 1)
                {
                    col = fixed4(1.0,0.0,0.0,1.0);   
                }
                return col;
            }
            */

            // drawing with dynamic points
            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uv = i.uv;
                fixed4 col = tex2D(_MainTex,uv);

                // fill color
                const float y = uv.y;
                const float checkX = uv.x;

                int intersect = 0;
                int j = _polygonPointCount - 1;
                for(int i = 0;i < _polygonPointCount;i++)
                {
                    fixed2 p1 = fixed2(_polygonPoints[j * 2],_polygonPoints[j * 2 + 1]);
                    fixed2 p2 = fixed2(_polygonPoints[i * 2],_polygonPoints[i * 2 + 1]);

                    // draw points
                    if(length(uv - p2) < 0.01)
                    {
                        col = fixed4(1.0,1.0,0.0,1.0);
                    }
                    

                    if((p1.y > y && p2.y < y) || (p1.y < y && p2.y > y))
                    {
                        float x = (y - p2.y) * (p1.x - p2.x) / (p1.y - p2.y) + p2.x;
                        if(checkX < x)
                        {
                            intersect++;
                        }    
                    }
                    j = i;
                }
                intersect = intersect % 2;
                
                
                
                // draw fill color
                if(_bFill == 1 && intersect == 1)
                {
                    col = fixed4(1.0,0.0,0.0,1.0);   
                }
                return col;
            }

            
            ENDCG
        }
    }
}
