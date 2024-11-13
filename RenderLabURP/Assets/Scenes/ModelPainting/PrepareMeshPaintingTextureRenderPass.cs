using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.Universal;

namespace ayy
{
    public class PrepareMeshPaintingTextureRenderPass : ScriptableRenderPass
    {
        public PrepareMeshPaintingTextureRenderPass()
        {
            
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //var camera = renderingData.cameraData.camera;
            //var cullResults = renderingData.cullResults;
            CommandBuffer cmd = CommandBufferPool.Get("ayy.PrepareMeshPaintingTextureRenderPass");
            using (new ProfilingScope(cmd, new ProfilingSampler("ayy.PrepareMeshPaintingTextureRenderPass"))) 
            {
                cmd.Clear();
                var renderers = GetRenderers();
                foreach (var renderer in renderers)
                {
                    PaintableMesh paintable = renderer.GetComponent<PaintableMesh>();
                    if(paintable.GetUnwrapUVMaterial() != null)
                    {
                        // 展开 uv 为 贴图, 并在其中 增加 轨迹绘制 的功能  
                        cmd.SetRenderTarget(paintable.GetPresentUVTexture());
                        cmd.ClearRenderTarget(true,true,Color.cyan);
                        cmd.DrawRenderer(renderer,paintable.GetUnwrapUVMaterial());
                        
                        // 给 绘制结果的图片，做一次 uv bleeding ,避免 uv 缝隙处 的问题 
                        cmd.Blit(paintable.GetPresentUVTexture(),
                            paintable.GetBleedingTexture(),
                            paintable.GetBleedingMaterial());
                    }
                }
                context.ExecuteCommandBuffer(cmd);
            }
            CommandBufferPool.Release(cmd);            
        }


        private List<Renderer> GetRenderers()
        {
            List<Renderer> result = new List<Renderer>();
            
            var paintableGameObjects = GameObject.FindGameObjectsWithTag("ayy.paintable");
            foreach (var go in paintableGameObjects)
            {
                PaintableMesh paintable = null;
                if (go.TryGetComponent<PaintableMesh>(out paintable))
                {
                    MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                    SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null)
                    {
                        result.Add(skinnedMeshRenderer);      
                    }
                    else if (meshRenderer != null)
                    {
                        result.Add(meshRenderer);   
                    }
                }
            }
            
            return result;
        }
    }
    
}
