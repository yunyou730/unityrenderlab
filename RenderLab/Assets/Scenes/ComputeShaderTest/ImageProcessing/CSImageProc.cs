using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ayy.cs
{
    public class CSImageProc : MonoBehaviour
    {
        private int _textureWidth = 512;
        private int _textureHeight = 512;

        private MeshRenderer _meshRenderer = null;
        private Material _material = null;

        private int _texW = 0;
        private int _texH = 0;

        private Texture2D _texture;
        private Color[] _pixels;
        
        private RenderTexture _rt = null;

        [SerializeField] private ComputeShader _computeShader = null;

        // Start is called before the first frame update
        void Start()
        {
            if (!PreCheckEnv(out _texW, out _texH))
            {
                Debug.Log("pre check failed");
                return;
            }

            CreatTexture(_texW,_texH);
            CreateRenderTexture(_texW,_texH);

            _meshRenderer = GetComponent<MeshRenderer>();
            _material = _meshRenderer.sharedMaterial;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("CPU");
                UpdateTextureCPU();
                BindTextureCPU();
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("GPU");
                UpdateTextureGPU();
                BindTextureGPU();
            }
        }

        private void OnDestroy()
        {
            if (_rt != null)
            {
                _rt.Release();
                _rt = null;
            }
        }
        
        private bool PreCheckEnv(out int texW,out int texH)
        {
            // Environment check
            Debug.Log("[is support compute shader]" + SystemInfo.supportsComputeShaders);
            if (!SystemInfo.supportsComputeShaders)
            {
                Debug.Log("Don't support compute shader,do nothing");
                texW = 0;
                texH = 0;
                return false;
            }
            
            Debug.Log("Compute Shader Version:" + SystemInfo.graphicsShaderLevel);
            Debug.Log("[max texture size]" + SystemInfo.maxTextureSize);

            texW = _textureWidth;
            texH = _textureHeight;
            if (texW > SystemInfo.maxTextureSize || texH > SystemInfo.maxTextureSize)
            {
                texW = SystemInfo.maxTextureSize;
                texH = SystemInfo.maxTextureSize;
            }
            Debug.Log(String.Format("real create texture size ({0},{1})",texW,texH));
            return true;
        }
        
        private void CreateRenderTexture(int texW,int texH)
        {
            _rt = new RenderTexture(texW,texH,0,RenderTextureFormat.ARGB32);
            _rt.enableRandomWrite = true;   // Can be used in shader RWTexture
            // _rt.Create();
        }

        private void CreatTexture(int texW,int texH)
        {
            _texture = new Texture2D(texW,texH);
            _pixels = new Color[texW * texH];
            _texture.SetPixels(_pixels);
        }


        private void UpdateTextureCPU()
        {
            for (int y = 0;y < _texH;y++)
            {
                for (int x = 0;x < _texW;x++)
                {
                    int index = y * _texW + x;
                    _pixels[index] = Color.cyan;
                }
            }
            _texture.SetPixels(_pixels);
            _texture.Apply();
        }

        void BindTextureCPU()
        {
            _material.SetTexture(Shader.PropertyToID("_MainTex"),_texture);
        }
        
        private void UpdateTextureGPU()
        {
            uint tx, ty, tz;
            int kernel = _computeShader.FindKernel("CSMain");
            _computeShader.GetKernelThreadGroupSizes(kernel,out tx, out ty, out tz);
            
            Debug.Log("tx:" + tx + "ty:" + ty + "tz:" + tz);
            
            _computeShader.SetTexture(kernel,Shader.PropertyToID("Result"),_rt);

            int gx = _texW / (int)tx;
            int gy = _texH / (int)ty;
            int gz = 1;
            _computeShader.Dispatch(kernel,gx,gy,gz);
        }
        
        private void BindTextureGPU()
        {
            _material.SetTexture(Shader.PropertyToID("_MainTex"),_rt);
        }
        

    }
    
}

