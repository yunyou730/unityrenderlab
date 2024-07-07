using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RedTintRenderPass : ScriptableRenderPass
{
    private Material _material = null;
    private RenderTextureDescriptor _textureDescriptor;
    private RTHandle _textureHandle = null;

    public RedTintRenderPass(Material material)
    {
        _material = material;
        _textureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        //Set the red tint texture size to be the same as the camera target size.
        _textureDescriptor.width = cameraTextureDescriptor.width;
        _textureDescriptor.height = cameraTextureDescriptor.height;
        
        //Check if the descriptor has changed, and reallocate the RTHandle if necessary.
        RenderingUtils.ReAllocateIfNeeded(ref _textureHandle, _textureDescriptor);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    { 
        //Get a CommandBuffer from pool.
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
        
        // Blit from the camera target to the temporary render texture,
        // using the first shader pass
        Blit(cmd,cameraTargetHandle,_textureHandle,_material,0);
        
        // Blit from temprorary render texture,
        // using the second shader pass
        Blit(cmd,_textureHandle,cameraTargetHandle,_material,1);
        
        // Execute the command buffer, and release it back to the pool
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    
    public void Dispose()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            Object.Destroy(_material);
        }
        else
        {
            Object.DestroyImmediate(_material);
        }
#else
            Object.Destroy(material);
#endif

        if (_textureHandle != null)
        {
            _textureHandle.Release();
        }

    }
}
