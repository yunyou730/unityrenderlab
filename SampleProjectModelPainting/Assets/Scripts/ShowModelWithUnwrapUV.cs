using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace ayy.showcase
{
    public class ShowModelWithUnwrapUV : MonoBehaviour
    {
        [SerializeField]    // for debug in Inspector 
        private RenderTexture _paintingRT = null;
        private Material _uvUnwrapMaterial = null;
        private Material _renderingMaterial = null;
        
        void Start()
        {
            _paintingRT = CreateRenderTargetForPainting();
            _uvUnwrapMaterial = new Material(Shader.Find("ayy/UnwrapUV"));
            _uvUnwrapMaterial.SetColor(Shader.PropertyToID("_MainColor"),Color.yellow);
            
            // painting uv texture at init
            CommandBuffer cmdbuf = CommandBufferPool.Get("ayy.model_painting");
            cmdbuf.SetRenderTarget(_paintingRT);
            cmdbuf.DrawRenderer(GetComponent<MeshRenderer>(),_uvUnwrapMaterial);
            Graphics.ExecuteCommandBuffer(cmdbuf);
            CommandBufferPool.Release(cmdbuf);
            
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
    }
}
