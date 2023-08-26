Shader "ayy/rpg/VisionRange"
{
    Properties
    {
        _Angle("Angle",float) = 0.785 // PI * 1/4
        _FrontDir("FrontDir",Vector) = (1,0,0,0)
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }a
        Tags {"Queue" = "Transparent"}
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 worldPos : SV_Target0;
            };

            //sampler2D _MainTex;
            //float4 _MainTex_ST;
            
            float _Angle;
            float4 _FrontDir;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // if(i.worldPos.x <= 0.0)
                // {
                //     discard;
                // }
                
                float2 uv = i.uv * 2.0 - 1.0;
                
                float2 frontDirIn2D = normalize(float2(_FrontDir.x,_FrontDir.z));
                float2 uvDir = normalize(uv);
                float dotValue = dot(uvDir,frontDirIn2D);
                float angle = acos(dotValue);

                if(length(uv) > 1.0 || angle > _Angle * 0.5)
                {
                    discard;
                }
                
                return float4(1.0,1.0,0.3,0.5);    
            }
            ENDCG
        }
    }
}
