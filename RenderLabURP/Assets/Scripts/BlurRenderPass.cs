using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal;

public class BlurRenderPass : ScriptableRenderPass
{
    private BlurSettings _defaultSettings;
    private Material _material;

    private RenderTextureDescriptor _blurTextureDescriptor;
    private RTHandle _blurTextureHandle;
    
    private static readonly int _horizontalBlurId = Shader.PropertyToID("_HorizontalBlur");
    private static readonly int _verticalBlurId = Shader.PropertyToID("_VerticalBlur");
    
    public BlurRenderPass(BlurSettings settings,Material material)
    {
        _defaultSettings = settings;
        _material = material;

        _blurTextureDescriptor =
            new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        _blurTextureDescriptor.width = cameraTextureDescriptor.width;
        _blurTextureDescriptor.height = cameraTextureDescriptor.height;

        RenderingUtils.ReAllocateIfNeeded(ref _blurTextureHandle, _blurTextureDescriptor);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        //Get a CommandBuffer from pool.
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

        UpdateBlurSettings();

        // Blit from the camera target to the temporary render texture,
        // using the first shader pass.
        Blit(cmd, cameraTargetHandle, _blurTextureHandle, _material, 0);
        // Blit from the temporary render texture to the camera target,
        // using the second shader pass.
        Blit(cmd, _blurTextureHandle, cameraTargetHandle, _material, 1);
        //Blit(cmd,_blurTextureHandle,cameraTargetHandle);

        //Execute the command buffer and release it back to the pool.
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

        if (_blurTextureHandle != null) _blurTextureHandle.Release();
    }
    
    // private void UpdateBlurSettings()
    // {
    //     if (_material == null)
    //         return;
    //     _material.SetFloat(_horizontalBlurId,_defaultSettings.horizontalBlur);
    //     _material.SetFloat(_verticalBlurId,_defaultSettings.verticalBlur);
    //     
    //     
    // }
    
    private void UpdateBlurSettings()
    {
        if (_material == null) 
            return;

        // Use the Volume settings or the default settings if no Volume is set.
        var volumeComponent = VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();

        float horizontalBlur = volumeComponent.horizontalBlur.overrideState ?
            volumeComponent.horizontalBlur.value : _defaultSettings.horizontalBlur;
        float verticalBlur = volumeComponent.verticalBlur.overrideState ?
            volumeComponent.verticalBlur.value : _defaultSettings.verticalBlur;
        
        
        Debug.Log($"blur factor: horizontal:{horizontalBlur},vertical:{verticalBlur}");
        _material.SetFloat(_horizontalBlurId, horizontalBlur);
        _material.SetFloat(_verticalBlurId, verticalBlur);
        
        // _material.SetFloat(_horizontalBlurId, 0);
        // _material.SetFloat(_verticalBlurId, 0);        
    }
    
    
}
