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
                //cmd.ClearRenderTarget(true,true,Color.green);      
                
                var renderers = GetRenderers();
                foreach (var renderer in renderers)
                {
                    PaintableMesh paintable = renderer.GetComponent<PaintableMesh>();
                    if(paintable.GetUnwrapUVMaterial() != null)
                    {
                        //cmd.Blit(paintable.GetPresentUVTexture(),paintable.GetBackupUVTexture());
                        cmd.Blit(paintable.GetBackupUVTexture(),paintable.GetPresentUVTexture());
                        
                        cmd.SetRenderTarget(paintable.GetPresentUVTexture());
                        //cmd.ClearRenderTarget(true,true,Color.cyan);
                        // @miao @todo 
                        // 这样为什么 材质的 MainTex 参数传不进去 
                        //cmd.SetGlobalTexture(Shader.PropertyToID("_MainTex"),paintable.GetPresentUVTexture());
                        cmd.DrawRenderer(renderer,paintable.GetUnwrapUVMaterial());
                        
                        //
                        paintable.SwapUVTexture();
                    }
                }
                context.ExecuteCommandBuffer(cmd);
            }
            CommandBufferPool.Release(cmd);            
        }


        private List<MeshRenderer> GetRenderers()
        {
            List<MeshRenderer> result = new List<MeshRenderer>();
            
            var paintableGameObjects = GameObject.FindGameObjectsWithTag("ayy.paintable");
            foreach (var go in paintableGameObjects)
            {
                PaintableMesh paintalbe = null;
                if (go.TryGetComponent<PaintableMesh>(out paintalbe))
                {
                    result.Add(go.GetComponent<MeshRenderer>());                    
                }
            }
            
            return result;
        }
    }
    
}
