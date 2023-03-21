Shader "Ayy/Brush2D/Brush"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FromTo ("FromTo",Vector) = (0.0,0.0,0.0,0.0)
        _ScreenWidth ("Screen Width",Float) = 0.0
        _ScreenHeight ("Screen Height",Float) = 0.0
        _BrushSize("Brush Size",Float) = 0.05
        _BrushColor("Brush Color",Color) = (1,1,1,1)
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

            float _ScreenWidth;
            float _ScreenHeight;
            float4 _FromTo;

            float _BrushSize;
            float4 _BrushColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            float2 handleRatio(float2 pt)
            {
                const float ratioWH = _ScreenWidth / _ScreenHeight;
                return float2(pt.x * ratioWH,pt.y);
            }

            float4 drawPoint(float2 uv,float2 center)
            {
                float4 result = float4(0.0,0.0,0.0,0.0);
                if(length(uv - center) <= _BrushSize)
                {
                    result = float4(_BrushColor.r,_BrushColor.g,_BrushColor.b,1.0);
                }
                return result;
            }
            
            float4 drawLine(float2 P,float2 A,float2 B)
            {
                float4 result = float4(0.0,0.0,0.0,0.0);
                
                float2 ap = P - A;
                float2 ab = B - A;
                float2 bp = P - B;

                // check range 
                if(dot(ap,ab) > 0 && dot(bp,-ab) > 0)
                {
                    // check distance from P to line AB
                    float apLen = length(ap); 
                    ap = normalize(ap);
                    ab = normalize(ab);
                    float theta = acos(dot(ap,ab));
                    float dis = sin(theta) * apLen;
                    if(dis < _BrushSize)
                    {
                        result = float4(_BrushColor.r,_BrushColor.g,_BrushColor.b,1.0);
                    }
                         
                }
                return result;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                const float2 uv = handleRatio(i.uv);
                float2 p1 = handleRatio(_FromTo.xy);
                float2 p2 = handleRatio(_FromTo.zw);

                if(p1.x >= 0 && p1.y >= 0 && p2.x >= 0 && p2.y >= 0)
                {
                    //col = max(drawPoint(uv,p1),col);
                    //col = max(drawPoint(uv,p2),col);
                    //col = max(drawLine(uv,p1,p2),col);

                    float4 tmp = drawPoint(uv,p1);
                    tmp = max(drawPoint(uv,p2),tmp);
                    tmp = max(drawLine(uv,p1,p2),tmp);
                    
                    if(tmp.a > 0.0)
                    {
                        col = tmp;
                    }
                    
                }
                return col;
            }
            ENDCG
        }
    }
}
