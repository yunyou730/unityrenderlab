using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnqueuePass : MonoBehaviour
{
    [SerializeField] 
    private Material _material;
    
    private RedTintRenderPass _pass = null;

    private void OnEnable()
    {
        _pass = new RedTintRenderPass(_material);
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;

    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;        
        _pass.Dispose();
    }
    
    private void OnBeginCamera(ScriptableRenderContext context, Camera cam)
    {
        // Use the EnqueuePass method to inject a custom render pass
        cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(_pass);
    }    
    
}
