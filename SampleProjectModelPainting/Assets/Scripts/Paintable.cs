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
        
        [SerializeField]
        private RenderTexture _paintingTargetRT1 = null;
        [SerializeField]
        private RenderTexture _paintingTargetRT2 = null;
        
        private Material _presentMaterial = null;
        private Material _paintingMaterial = null;
        
        private Vector3? _hitWorldPos = null;
        private Vector3? _prevWorldPos = null;


        private CommandBuffer _paintingCmdBuf = null;        
        
        void Start()
        {
            _paintingTargetRT1 = CreateRenderTargetForPainting();
            _paintingTargetRT2 = CreateRenderTargetForPainting();
            
            _paintingMaterial = new Material(Shader.Find("ayy/ModelPainting"));
            _presentMaterial = GetPresentMaterial();
            
            _paintingMaterial.SetTexture(Shader.PropertyToID("_AdditiveTexture"),GetPaintingBackup());
            _presentMaterial.SetTexture(Shader.PropertyToID("_PaintingTexture"),GetPaintingTarget());
        }
        
        void Update()
        {
            _paintingCmdBuf = BuildPaintingCommandBuffer();
            Graphics.ExecuteCommandBuffer(_paintingCmdBuf);
            
            CheckMousePainting();
            UpdateBrushParameter();
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
            return _paintingTargetRT1;
        }

        public RenderTexture GetPaintingBackup()
        {
            return _paintingTargetRT2;
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
                // Previous painting pos
                if (_hitWorldPos != null)
                {
                    _prevWorldPos = _hitWorldPos;
                    _paintingMaterial.SetVector(Shader.PropertyToID("_PrevPos"),
                        new Vector4(_prevWorldPos.Value.x,_prevWorldPos.Value.y,_prevWorldPos.Value.z,1.0f));
                    _paintingMaterial.SetFloat(Shader.PropertyToID("_EnablePrevPos"),1.0f);
                }

                // Current Painting Pos
                _hitWorldPos = worldPos;
                _paintingMaterial.SetVector(Shader.PropertyToID("_CurPos"),
                    new Vector4(_hitWorldPos.Value.x,_hitWorldPos.Value.y,_hitWorldPos.Value.z,1.0f));
                _paintingMaterial.SetFloat(Shader.PropertyToID("_EnableCurPos"),1.0f);
            }
            else
            {
                _hitWorldPos = null;
                _prevWorldPos = null;
                _paintingMaterial.SetFloat(Shader.PropertyToID("_EnableCurPos"),0.0f);
                _paintingMaterial.SetFloat(Shader.PropertyToID("_EnablePrevPos"),0.0f);
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

    }
    
}
