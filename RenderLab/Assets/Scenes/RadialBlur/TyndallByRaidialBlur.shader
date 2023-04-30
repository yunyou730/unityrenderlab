Shader "Ayy/PostEffect/TyndallByRadialBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _BrightnessThreshold("brightness threshold",Range(0,1)) = 0.8
        
        _DirX("DirX",Range(-1,1)) = 1
        _DirY("DirY",Range(-1,1)) = 1
        
        _SampleSteps("SampleSteps",Range(1,20)) = 10
        _BlurAmount("BlurAmount",Range(0.0,20.0)) = 1.0
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
            float4 _MainTex_TexelSize;
            
            int _SampleSteps;
            float _BlurAmount;

            float _DirX;
            float _DirY;

            float _BrightnessThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);

                float brightness = color * float4(0.2126, 0.7152, 0.0722,1.0);
                
                float checkBrightness = step(_BrightnessThreshold,brightness);
                //return float4(checkBrightness,0,0,1.0);
                
                
                float2 blurDir = normalize(float2(_DirX,_DirY));
                
                float2 texelSize = _MainTex_TexelSize.xy;

                float2 delta = blurDir * _BlurAmount;

                float checkDirValid = step(0.0,length(delta));
                
                
                float4 sum = color;
                for (int j = 1; j <= _SampleSteps; j++) {
                    float2 offset = delta * j;
                    sum += tex2D(_MainTex, i.uv + offset * texelSize);
                    sum += tex2D(_MainTex, i.uv - offset * texelSize);
                }
                float4 blurColor = sum / (float(_SampleSteps) * 2.0 + 1.0);
                blurColor = lerp(color,blurColor,checkDirValid);
                
                //return lerp(color,blurColor,checkBrightness);

                return (color + blurColor) * 0.5;
            }            
            
            ENDCG
        }
    }
}
