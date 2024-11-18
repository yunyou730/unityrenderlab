using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace ayy
{
    public class Paintable : MonoBehaviour
    {
        [SerializeField,Range(0,2)]
        private float BrushSize = 0.2f;
        [SerializeField]
        private Color BrushColor = Color.magenta;
        
        private bool _paintingRTFlag = true;
        [SerializeField]
        private RenderTexture _paintingTargetRT1 = null;
        [SerializeField]
        private RenderTexture _paintingTargetRT2 = null;
        
        private Material _presentMaterial = null;
        private Material _paintingMaterial = null;
        
        private Vector3? _hitWorldPos = null;
        private Vector3? _prevWorldPos = null;


        private CommandBuffer _paintingCmdBuf = null;        
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _paintingTargetRT1 = CreateRenderTargetForPainting();
            _paintingTargetRT2 = CreateRenderTargetForPainting();
            
            _paintingMaterial = new Material(Shader.Find("ayy/ModelPainting"));
            
            _presentMaterial = GetPresentMaterial();


            var cmdbuf = CommandBufferPool.Get("ayy.CreatePaintingCommandBuffer");
            cmdbuf.SetRenderTarget(_paintingTargetRT1);
            cmdbuf.ClearRenderTarget(true,true,new Color(0,1,0,0));
            cmdbuf.SetRenderTarget(_paintingTargetRT2);
            cmdbuf.ClearRenderTarget(true,true,new Color(0,0,1,0));
            Graphics.ExecuteCommandBuffer(cmdbuf);
            CommandBufferPool.Release(cmdbuf);
        }

        // Update is called once per frame
        void Update()
        {
            _paintingCmdBuf = BuildPaintingCommandBuffer();
            Graphics.ExecuteCommandBuffer(_paintingCmdBuf);
            
            CheckMousePainting();
            UpdateBrushParameter();
            
            _paintingMaterial.SetTexture(Shader.PropertyToID("_AdditiveTexture"),GetPaintingBackup());
            _presentMaterial.SetTexture(Shader.PropertyToID("_PaintingTexture"),GetPaintingTarget());
            
            //SwapPaintingTexture();
        }

        private void OnDisable()
        {
            if (_paintingCmdBuf != null)
            {
                CommandBufferPool.Release(_paintingCmdBuf);
                _paintingCmdBuf = null;
            }
        }

        private CommandBuffer BuildPaintingCommandBuffer()
        {
            CommandBuffer cmdbuf = CommandBufferPool.Get("ayy.CreatePaintingCommandBuffer");
            cmdbuf.Clear();
            cmdbuf.SetRenderTarget(GetPaintingTarget());
            cmdbuf.SetGlobalTexture(Shader.PropertyToID("_AdditiveTexture"),GetPaintingBackup());
            cmdbuf.DrawRenderer(GetRenderer(),GetPaintingMaterial());
            cmdbuf.Blit(GetPaintingTarget(),GetPaintingBackup());
            return cmdbuf;
        }

        public Material GetPaintingMaterial()
        {
            return _paintingMaterial;
        }

        private RenderTexture CreateRenderTargetForPainting()
        {
            var ret = new RenderTexture(1024, 1024, 32, DefaultFormat.LDR);
            ret.enableRandomWrite = true;
            //ret.filterMode = FilterMode.Bilinear;
            return ret;
        }

        private Material GetPresentMaterial()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                return meshRenderer.material;
            }
            return null;
        }

        public RenderTexture GetPaintingTarget()
        {
            return _paintingRTFlag ? _paintingTargetRT1 : _paintingTargetRT2;
            // return _paintingTargetRT1;
        }

        public RenderTexture GetPaintingBackup()
        {
            return _paintingRTFlag ? _paintingTargetRT2 : _paintingTargetRT1;
        }

        private void CheckMousePainting()
        {
            Vector3? worldPos = null;
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray,out hit))
                {
                    worldPos = hit.point;
                } 
            }

            if (worldPos != null)
            {
                _hitWorldPos = worldPos;
                _paintingMaterial.SetVector(Shader.PropertyToID("_CurPos"),
                    new Vector4(_hitWorldPos.Value.x,_hitWorldPos.Value.y,_hitWorldPos.Value.z,1.0f));
                _paintingMaterial.SetFloat(Shader.PropertyToID("_EnableCurPos"),1.0f);
            }
            else
            {
                _hitWorldPos = null;
                _paintingMaterial.SetFloat(Shader.PropertyToID("_EnableCurPos"),0.0f);
            }
        }


        private void UpdateBrushParameter()
        {
            _paintingMaterial.SetFloat(Shader.PropertyToID("_BrushSize"),BrushSize);
            _paintingMaterial.SetColor(Shader.PropertyToID("_BrushColor"),BrushColor);
        }
        
        private Renderer GetRenderer()
        {
            return GetComponent<MeshRenderer>();
        }

        private void SwapPaintingTexture()
        {
            _paintingRTFlag = !_paintingRTFlag;
        }

    }
    
}
