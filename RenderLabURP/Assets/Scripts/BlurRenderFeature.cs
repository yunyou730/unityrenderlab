using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class BlurSettings
{
    [Range(0,0.4f)] public float horizontalBlur;
    [Range(0,0.4f)] public float verticalBlur;
}

public class BlurRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private BlurSettings _settings;
    [SerializeField] private Shader _shader;
    private Material _material;
    private BlurRenderPass _blurRenderPass;
    
    public override void Create()
    {
        if (_shader == null)
        {
            return;
        }

        _material = new Material(_shader);
        _blurRenderPass = new BlurRenderPass(_settings, _material);
        _blurRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(_blurRenderPass);
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        _blurRenderPass.Dispose();
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            Destroy(_material);
        }
        else
        {
            DestroyImmediate(_material);
        }
#else
            Destroy(material);
#endif
    }    
}
