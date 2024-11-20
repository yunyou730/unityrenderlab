using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace ayy.showcase
{
    public class ShowModelWithAdditive : MonoBehaviour
    {
        private RenderTexture _paintingRT = null;
        private RenderTexture _backupRT = null;
        
        private Material _paintingMaterial = null;
        private Material _renderingMaterial = null;
        
        private Vector3? _hitWorldPos = null;
        
        void Start()
        {
            _paintingRT = CreateRenderTargetForPainting();
            _backupRT = CreateRenderTargetForPainting();
            
            _paintingMaterial = new Material(Shader.Find("ayy/UnwrapUVWithPaintingAdditive"));
            _paintingMaterial.SetColor(Shader.PropertyToID("_MainColor"),Color.green);
            _paintingMaterial.SetTexture(Shader.PropertyToID("_AdditiveTexture"),_backupRT);

            Debug.Log("[mat][_EnableCurPos]" + _paintingMaterial.HasProperty(Shader.PropertyToID("_EnableCurPos")));
            Debug.Log("[mat][_CurPos]" + _paintingMaterial.HasProperty(Shader.PropertyToID("_EnableCurPos")));
            
            
            // render modol with paintingRT
            _renderingMaterial = GetComponent<MeshRenderer>().material;
            _renderingMaterial.SetTexture(Shader.PropertyToID("_PaintingTexture"),_paintingRT);
        }
        
        private RenderTexture CreateRenderTargetForPainting()
        {
            var ret = new RenderTexture(1024, 1024, 32, DefaultFormat.LDR);
            ret.enableRandomWrite = true;
            ret.filterMode = FilterMode.Bilinear;
            return ret;
        }

        void Update()
        {
            // Painting to material
            CommandBuffer cmdbuf = CommandBufferPool.Get("ayy.model_painting");
            cmdbuf.SetRenderTarget(_paintingRT);
            cmdbuf.DrawRenderer(GetComponent<MeshRenderer>(),_paintingMaterial);
            cmdbuf.Blit(_paintingRT,_backupRT);
            Graphics.ExecuteCommandBuffer(cmdbuf);
            CommandBufferPool.Release(cmdbuf);
            
            // Mouse Click
            Vector3? clickWorldPos = CheckMousePainting();
            if (clickWorldPos != null)
            {
                _hitWorldPos = clickWorldPos;
                _paintingMaterial.SetVector(Shader.PropertyToID("_CurPos"),
                    new Vector4(_hitWorldPos.Value.x,_hitWorldPos.Value.y,_hitWorldPos.Value.z,1.0f));
                _paintingMaterial.SetFloat(Shader.PropertyToID("_EnableCurPos"),1.0f);
            }
            else
            {
                if (_hitWorldPos != null)
                {
                    _hitWorldPos = null;
                    _paintingMaterial.SetFloat(Shader.PropertyToID("_EnableCurPos"),0.0f);
                }
            } 
        }
        
        private Vector3? CheckMousePainting()
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
            return worldPos;
        }
        
    }
}
