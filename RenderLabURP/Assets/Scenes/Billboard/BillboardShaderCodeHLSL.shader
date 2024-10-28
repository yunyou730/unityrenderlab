Shader "Ayy/AyyUnlit"
{
    Properties
    {
        _BaseColor("Base Color",Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
                
        _CameraPosition("Camera Position",Vector) = (0.0,0.0,0.0)
        _VerticalBillboard("Vertical Billboarding",Range(0,1)) = 1
        _FuncMode("Func Mode",Range(0,3)) = 0.5
        
        _LocalXOffset("Local X",Range(-10.0,10.0)) = 0.0
        _LocalYOffset("Local Y",Range(-10.0,10.0)) = 0.0
        _LocalZOffset("Local Z",Range(-10.0,10.0)) = 0.0
        
        _ShowTexOrUV("Show Texture Or UV",Range(0,1)) = 0
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        Cull Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes
            {
                float4 positionOS : POSITION;   // OS:Object Space
                float2 uv : TEXCOORD0;
                
            };

            struct Varyings
            {
                float4 positionHCS :SV_POSITION;    // HCS: Homogeneous Clipping Space
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            sampler2D _MainTex;
            
            float _DebugDistance;
            float3 _CameraPosition;
            float _VerticalBillboard;
            float _FuncMode;

            float _LocalXOffset;
            float _LocalYOffset;
            float _LocalZOffset;

            float _ShowTexOrUV;
            CBUFFER_END
            
            float3 RecalculateAsBillboard1(float3 originLocalPos)
            {
                float3 center = float3(0.0,0.0,0.0);
                float3 viewer = TransformWorldToObject(_CameraPosition);
                
                float3 normalDir = viewer - center;
                normalDir.y = normalDir.y * _VerticalBillboard;     // _VerticalBillboard,0 or 1 
                normalDir = normalize(normalDir);

                float3 upDir = abs(normalDir.y) > 0.999 ? float3(0,0,1) : float3(0,1,0);
                float3 rightDir = normalize(cross(upDir,normalDir));
                upDir = normalize(cross(normalDir,rightDir));
                rightDir = -rightDir;   // fix invert uv.x
                
                float3 centerOffset = originLocalPos - center;

                /*
                 * 这里 用 加法，而不用 矩阵 变换的原因是 
                 * 矩阵变换的几何含义：一个坐标在坐标系A的描述, 改为坐标系B 的描述;
                 * 而这里, 并不是换个B坐标系 来描述, 而是 在 B 坐标系下  每个顶点的相对位置
                 * 因此 方法2 是错误的 , 这个方法是正确的. 必须用坐标加法 来做 
                 */
                float3 localPos = center + rightDir * centerOffset.x + upDir * centerOffset.y + normalDir * centerOffset.z;

                return localPos;
            }

            float3 RecalculateAsBillboard2(float3 originLocalPos)
            {
                float3 center = float3(0.0,0.0,0.0);
                float3 viewer = TransformWorldToObject(_CameraPosition);

                float3 normalDir = viewer - center;
                normalDir.y = normalDir.y * _VerticalBillboard;     // _VerticalBillboard
                normalDir = normalize(normalDir);
                
                float3 upDir = abs(normalDir.y) > 0.999 ? float3(0,0,1) : float3(0,1,0);
                float3 rightDir = normalize(cross(upDir,normalDir));
                upDir = normalize(cross(normalDir,rightDir));
            
                // 这一步明显是错误的 
                float3x3 rotationMatrix = float3x3(rightDir,upDir,normalDir);
                float3 localPos = mul(rotationMatrix,originLocalPos.xyz);
                
                return localPos;
            }            

            Varyings vert(Attributes IN)
            {
                float3 localPos;
                if(_FuncMode >= 0.0 && _FuncMode <= 1.0)
                {
                    localPos = RecalculateAsBillboard1(IN.positionOS.xyz);   
                }
                else if(_FuncMode > 1.0 && _FuncMode <= 2.0)
                {
                    localPos = RecalculateAsBillboard2(IN.positionOS.xyz);   
                }
                else if(_FuncMode > 2.0 && _FuncMode <= 3.0)
                {
                    localPos = IN.positionOS.xyz;
                }

                /*
                float3 cameraDir = normalize(cameraPosInObjectSpace);
                float3 tempUpDir = float3(0.0,1.0,0.0);
                
                float3 right = normalize(cross(cameraDir,tempUpDir));
                float3 up = normalize(cross(right,cameraDir));
                
    
                float3x3 rotMat = float3x3(right,up,cameraDir);
                float3 pos = IN.positionOS.xyz;
                pos = right * pos.x + up * pos.y + cameraDir * pos.z;
                //pos = mul(rotMat,pos);
                
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(pos);
                OUT.uv = IN.uv;
                */
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(localPos);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //float3 pos = _WorldSpaceCameraPos;
                //_WorldSpaceCameraPos
                //float3 cameraPosInObjectSpace = TransformWorldToObject(_CameraPosition);
                //float3 cameraDir = normalize(cameraPosInObjectSpace);
                //cameraDir = cameraDir * 2 - 1;
                
                //float4 ret = float4(cameraDir,1.0); 
                //ret *= _BaseColor;


                float4 ret = float4(IN.uv.x,IN.uv.y,0.0,1.0);
                if(_ShowTexOrUV <= 0.5)
                {
                    ret = tex2D(_MainTex,IN.uv);
                }                
                
                return ret;
            }
            
            ENDHLSL
        }
    }
}