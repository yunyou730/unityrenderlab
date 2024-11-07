using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    public class PrepareMeshPaintingTextureRenderFeature : ScriptableRendererFeature
    {
        private PrepareMeshPaintingTextureRenderPass _pass = null;
        
        public override void Create()
        {
            // Shader sampleShader = Shader.Find("ayy/ModelPaintingTest");
            // var paintableUVTextureMaterial = new Material(sampleShader);
            
            _pass = new PrepareMeshPaintingTextureRenderPass()
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingOpaques
            };
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.isSceneViewCamera)     // only work in gameplay,not work in scene view
            {
                renderer.EnqueuePass(_pass);                
            }
        }
    }
}


