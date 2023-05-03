using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
    private ScriptableRenderContext _context;
    private Camera _camera = null;
    
    private string SampleName { get; set; }
    private CommandBuffer _buffer = new CommandBuffer();

    private CullingResults _cullingResults;

    private static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

#if UNITY_EDITOR
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
#endif

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _context = context;
        _camera = camera;
        
        PrepareBuffer();
        PrepareForSceneWindow();
        if (!Cull())
        {
            return;
        }
        
        Setup();
        DrawVisibleGeometry();
#if UNITY_EDITOR
        DrawUnsupportedShaders();
        DrawGizmo();
#endif
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
        _buffer.BeginSample(_camera.name);
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

    #if UNITY_EDITOR
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


    void DrawGizmo()
    {
        if (Handles.ShouldRenderGizmos())
        {
            _context.DrawGizmos(_camera,GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera,GizmoSubset.PostImageEffects);
        }
    }

    void PrepareForSceneWindow()
    {
        if (_camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
        }
    }
    
    void PrepareBuffer()
    {
        _buffer.name = SampleName = _camera.name;
    }

#endif
    
    
    

    void Submit()
    {
        _buffer.EndSample(_camera.name);
        ExecuteBuffer();
        _context.Submit();
    }

    void ExecuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
}
