using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MyRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader _shader = null;
    private Material _material;
    private RedTintRenderPass _redTintRenderPass = null;
    public override void Create()
    {
        if (_shader == null)
        {
            return;
        }
        _material = CoreUtils.CreateEngineMaterial(_shader);
        _redTintRenderPass = new RedTintRenderPass(_material);
        _redTintRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(_redTintRenderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_material);
    }
}
