using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
    private ScriptableRenderContext _context;
    private Camera _camera = null;

    private const string _bufferName = "Render Camera";
    private CommandBuffer _buffer = new CommandBuffer { name = _bufferName };

    private CullingResults _cullingResults;

    private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    private static ShaderTagId[] legacyShaderTagIds =
    {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM"),
    };


    private static Material errorMaterial = null;

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _context = context;
        _camera = camera;

        if (!Cull())
        {
            return;
        }
        
        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        Submit();
    }

    bool Cull()
    {
        if (_camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            _cullingResults = _context.Cull(ref p);
            return true;
        }
        return false;
    }

    void Setup()
    {
        _buffer.ClearRenderTarget(true,true,Color.clear);
        _buffer.BeginSample(_bufferName);
        ExecuteBuffer();
        _context.SetupCameraProperties(_camera);
    }

    void DrawVisibleGeometry()
    {
        // draw opaque
        var sortingSettings = new SortingSettings(_camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };
        var drawingSettings = new DrawingSettings(unlitShaderTagId,sortingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        _context.DrawRenderers(_cullingResults,ref drawingSettings,ref filteringSettings);
        
        // draw skybox 
        _context.DrawSkybox(_camera);
        
        // draw transparent
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
    }


    void DrawUnsupportedShaders()
    {
        if (errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }

        var drawingSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(_camera))
        {
            overrideMaterial = errorMaterial
        };
            
    for (int i = 0;i < legacyShaderTagIds.Length;i++)
        {
            drawingSettings.SetShaderPassName(i,legacyShaderTagIds[i]);
        }
        var filteringSettings = FilteringSettings.defaultValue;
        _context.DrawRenderers(_cullingResults,ref drawingSettings,ref filteringSettings);
    }

    void Submit()
    {
        _buffer.EndSample(_bufferName);
        ExecuteBuffer();
        _context.Submit();
    }

    void ExecuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
}
