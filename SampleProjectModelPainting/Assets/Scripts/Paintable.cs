using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace ayy
{
    public class Paintable : MonoBehaviour
    {
        private float _brushSize = 0.2f;
        private Color _brushColor = Color.magenta;
        private float _brushSmooth = 0.03f;
        private bool _bEnableSmooth = true;
        private bool _bEnableBleeding = true;
        
        private RenderTexture _paintingTargetRT1 = null;
        private RenderTexture _paintingTargetRT2 = null;
        private RenderTexture _uvBleedingRT = null;
        
        private Material _presentMaterial = null;
        private Material _paintingMaterial = null;
        private Material _uvBleedingMaterial = null;
        
        private Vector3? _hitWorldPos = null;
        private Vector3? _prevWorldPos = null;
        
        private CommandBuffer _paintingCmdBuf = null;

        private Mesh _dynamicBakeSkinMesh = null;

        public bool _enableAnimationMeshBake = true; 
        
        void Start()
        {
            _paintingTargetRT1 = CreateRenderTargetForPainting();
            _paintingTargetRT2 = CreateRenderTargetForPainting();
            _uvBleedingRT = CreateRenderTargetForPainting();
            
            _paintingMaterial = new Material(Shader.Find("ayy/ModelPainting"));
            _presentMaterial = GetPresentMaterial();
            _uvBleedingMaterial = new Material(Shader.Find("ayy/UVBleeding"));
            
            _paintingMaterial.SetTexture(Shader.PropertyToID("_AdditiveTexture"),GetPaintingBackup());
            _presentMaterial.SetTexture(Shader.PropertyToID("_PaintingTexture"),_bEnableBleeding ? GetUVBleedingTexture() : GetPaintingTarget());
            
            ClearCreatedRenderTextures();

            if (IsNeedSyncAnimMesh())
            {
                _dynamicBakeSkinMesh = new Mesh();
            }
        }
        
        void Update()
        {
            _paintingCmdBuf = BuildPaintingCommandBuffer();
            Graphics.ExecuteCommandBuffer(_paintingCmdBuf);
            
            CheckMousePainting();
            UpdateBrushParameter();

            if (IsNeedSyncAnimMesh())
            {
                SyncColliderMesh(GetComponent<SkinnedMeshRenderer>(),GetComponent<MeshCollider>());
            }
        }

        private void OnDisable()
        {
            if (_paintingCmdBuf != null)
            {
                CommandBufferPool.Release(_paintingCmdBuf);
                _paintingCmdBuf = null;
            }
        }
        
        public void SyncBrushSettings(float brushSize,Color brushColor,float brushSmooth,bool bEnableSmooth,bool bEnableBleeding)
        {
            _brushSize = brushSize;
            _brushColor = brushColor;
            _brushSmooth = brushSmooth;
            _bEnableSmooth = bEnableSmooth;
            _bEnableBleeding = bEnableBleeding;
        }

        private void ClearCreatedRenderTextures()
        {
            CommandBuffer cmdbuf = CommandBufferPool.Get("ayy.CreatePaintingCommandBuffer");
            cmdbuf.Clear();
            cmdbuf.SetRenderTarget(_paintingTargetRT1);
            cmdbuf.ClearRenderTarget(true,true,new Color(0,0,0,0));
            cmdbuf.SetRenderTarget(_paintingTargetRT2);
            cmdbuf.ClearRenderTarget(true,true,new Color(0,0,0,0));
            Graphics.ExecuteCommandBuffer(cmdbuf);
            CommandBufferPool.Release(cmdbuf);
        }

        private CommandBuffer BuildPaintingCommandBuffer()
        {
            CommandBuffer cmdbuf = CommandBufferPool.Get("ayy.CreatePaintingCommandBuffer");
            cmdbuf.Clear();
            cmdbuf.SetRenderTarget(GetPaintingTarget());
            cmdbuf.DrawRenderer(GetRenderer(),GetPaintingMaterial());
            cmdbuf.Blit(GetPaintingTarget(),GetPaintingBackup());
            cmdbuf.Blit(GetPaintingTarget(),GetUVBleedingTexture(),GetUVBleedingMaterial());
            return cmdbuf;
        }

        private Material GetPaintingMaterial()
        {
            return _paintingMaterial;
        }

        private RenderTexture CreateRenderTargetForPainting()
        {
            var ret = new RenderTexture(1024, 1024, 32, DefaultFormat.LDR);
            ret.enableRandomWrite = true;
            ret.filterMode = FilterMode.Bilinear;
            return ret;
        }

        private Material GetPresentMaterial()
        {
            Renderer renderer = GetRenderer();
            return renderer.material;
        }

        private Material GetUVBleedingMaterial()
        {
            return _uvBleedingMaterial;
        }

        private RenderTexture GetPaintingTarget()
        {
            return _paintingTargetRT1;
        }

        private RenderTexture GetPaintingBackup()
        {
            return _paintingTargetRT2;
        }

        private RenderTexture GetUVBleedingTexture()
        {
            return _uvBleedingRT;
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
                // Previous painting pos to material 
                if (_hitWorldPos != null)
                {
                    _prevWorldPos = _hitWorldPos;
                    _paintingMaterial.SetVector(Shader.PropertyToID("_PrevPos"),
                        new Vector4(_prevWorldPos.Value.x,_prevWorldPos.Value.y,_prevWorldPos.Value.z,1.0f));
                    _paintingMaterial.SetFloat(Shader.PropertyToID("_EnablePrevPos"),1.0f);
                }

                // Current Painting Pos to material
                _hitWorldPos = worldPos;
                _paintingMaterial.SetVector(Shader.PropertyToID("_CurPos"),
                    new Vector4(_hitWorldPos.Value.x,_hitWorldPos.Value.y,_hitWorldPos.Value.z,1.0f));
                _paintingMaterial.SetFloat(Shader.PropertyToID("_EnableCurPos"),1.0f);
            }
            else
            {
                // Clear painting pos to material
                _hitWorldPos = null;
                _prevWorldPos = null;
                _paintingMaterial.SetFloat(Shader.PropertyToID("_EnableCurPos"),0.0f);
                _paintingMaterial.SetFloat(Shader.PropertyToID("_EnablePrevPos"),0.0f);
            }
        }
        
        private void UpdateBrushParameter()
        {
            _paintingMaterial.SetFloat(Shader.PropertyToID("_BrushSize"),_brushSize);
            _paintingMaterial.SetColor(Shader.PropertyToID("_BrushColor"),_brushColor);
            _paintingMaterial.SetFloat(Shader.PropertyToID("_BrushSmooth"),_brushSmooth);
            _paintingMaterial.SetFloat(Shader.PropertyToID("_EnableBrushSmooth"),_bEnableSmooth ? 1f:0.0f);
            _presentMaterial.SetTexture(Shader.PropertyToID("_PaintingTexture"),_bEnableBleeding ? GetUVBleedingTexture() : GetPaintingTarget());
        }
        
        private Renderer GetRenderer()
        {
            Renderer renderer = GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                renderer = GetComponent<SkinnedMeshRenderer>();
            }
            Debug.Assert(renderer != null,"There's no renderer on Paintable GameObject");
            return renderer;
        }

        private bool IsNeedSyncAnimMesh()
        {
            if (!_enableAnimationMeshBake)
                return false;
            return GetComponent<SkinnedMeshRenderer>() != null && GetComponent<MeshCollider>() != null;
        }

        private void SyncColliderMesh(SkinnedMeshRenderer skinnedMeshRenderer,MeshCollider meshCollider)
        {
            skinnedMeshRenderer.BakeMesh(_dynamicBakeSkinMesh,true);
            meshCollider.sharedMesh = _dynamicBakeSkinMesh;
        }

    }
    
}
