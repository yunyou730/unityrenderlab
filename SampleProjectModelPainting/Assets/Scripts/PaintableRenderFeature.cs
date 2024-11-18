// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.RenderGraphModule;
// using UnityEngine.Rendering.RenderGraphModule.Util;
// using UnityEngine.Rendering.Universal;
// using UnityEngine.Rendering.Universal.Internal;
//
// namespace  ayy
// {
//     public class PaintableRenderFeature : ScriptableRendererFeature
//     {
//         private PaintableRenderPass _pass = null;
//         
//         public override void Create()
//         {
//             _pass = new PaintableRenderPass()
//             {
//                 renderPassEvent = RenderPassEvent.BeforeRenderingOpaques
//             };
//         }
//     
//         public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//         {
//             if (renderingData.cameraData.cameraType == CameraType.Game)
//             {
//                 renderer.EnqueuePass(_pass);
//             }
//         }
//     }
//
//
//     class PassData
//     {
//         
//     }
//
//     class PaintableRenderPass : ScriptableRenderPass
//     {
//         public PaintableRenderPass()
//         {
//             
//         }
//         
//         [Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
//         public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//         {
//             CommandBuffer cmd = CommandBufferPool.Get("ayy.PaintablePass");
//             using (new ProfilingScope(cmd, new ProfilingSampler("ayy.PaintablePass")))
//             {
//                 cmd.Clear();
//                 var renderers = GetRenderers();
//                 foreach (var renderer in renderers)
//                 {
//                     Paintable paintable = renderer.GetComponent<ayy.Paintable>();
//                     if (paintable.GetPaintingTarget() != null)
//                     {
//
//                         RenderTextureDescriptor rtd = new RenderTextureDescriptor();
//                         cmd.SetRenderTarget(rtd);
//                         
//                         //cmd.SetRenderTarget(paintable.GetPaintingTarget());
//                         // cmd.DrawRenderer(renderer,paintable.GetPaintingMaterial());    
//                     }
//                 }
//                 context.ExecuteCommandBuffer(cmd);
//
//                 
//                 
//                 cmd.Clear();
//                 context.ExecuteCommandBuffer(cmd);
//             }
//             CommandBufferPool.Release(cmd);            
//         }
//         
//         private List<Renderer> GetRenderers()
//         {
//             List<Renderer> result = new List<Renderer>();
//             
//             var paintableGameObjects = GameObject.FindGameObjectsWithTag("ayy.paintable");
//             foreach (var go in paintableGameObjects)
//             {
//                 MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
//                 SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();
//                 if (skinnedMeshRenderer != null)
//                 {
//                     result.Add(skinnedMeshRenderer);      
//                 }
//                 else if (meshRenderer != null)
//                 {
//                     result.Add(meshRenderer);   
//                 }
//             }
//             
//             return result;
//         }
//     }
// }
