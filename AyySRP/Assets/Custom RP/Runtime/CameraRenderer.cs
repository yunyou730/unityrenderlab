using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace ayy.srp
{
    public partial class CameraRenderer
    {
        private ScriptableRenderContext _context;
        private Camera _camera;

        private CullingResults _cullingResults;

        private static ShaderTagId s_unlitShader = new ShaderTagId("SRPDefaultUnlit");
        
        
        private const string kBufferName = "ayy Render Camera";
        private CommandBuffer _buffer = new CommandBuffer
        {
            name = kBufferName
        };
        
        
#if UNITY_EDITOR
        private string _sampleName { get; set; }
#else
        private string _sampleName = kBufferName;  
#endif
        
        public void Render(ScriptableRenderContext context,Camera camera)
        {
            this._context = context;
            this._camera = camera;
            
#if UNITY_EDITOR
            PrepareBuffer();
            PrepareForSceneWindow();
#endif

            if (!Cull())
            {
                return;
            }

            Setup();
            DrawVisibleGeometry();
#if UNITY_EDITOR
            DrawUnsupportedSettings();
            DrawGizmos();
#endif
            Submit();
        }


        void Setup()
        {
            _context.SetupCameraProperties(_camera);
            CameraClearFlags flags = _camera.clearFlags;
            // _buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth,
            //     flags == CameraClearFlags.Color,
            //     flags == CameraClearFlags.Color ? _camera.backgroundColor.linear : Color.clear);
            
            // _buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth,
            //     flags == CameraClearFlags.Color,
            //     flags == CameraClearFlags.Color ? _camera.backgroundColor.linear : Color.clear);
            
            _buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth,
                flags <= CameraClearFlags.Color,
                flags == CameraClearFlags.Color ? _camera.backgroundColor.linear : Color.clear);               
            _buffer.BeginSample(_sampleName);
            ExecuteBuffer();
        }
        
        private void Submit()
        {
            _buffer.EndSample(_sampleName);
            ExecuteBuffer();
            _context.Submit();
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

        private void ExecuteBuffer()
        {
            _context.ExecuteCommandBuffer(_buffer);
            _buffer.Clear();
        }

        private void DrawVisibleGeometry()
        {
            // Drawing GameObject's renderers. Opaque. 
            var sortSettings = new SortingSettings { criteria = SortingCriteria.CommonOpaque };
            var drawingSettings = new DrawingSettings(s_unlitShader, sortSettings);
            var filterSettings = new FilteringSettings(RenderQueueRange.opaque);
            _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filterSettings);

            // Drawing SkyBox of the scene
            _context.DrawSkybox(_camera);

            // Drawing GameObject's renderers. Transparent.
            sortSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortSettings;
            filterSettings.renderQueueRange = RenderQueueRange.transparent;
            _context.DrawRenderers(_cullingResults,ref drawingSettings,ref filterSettings);
        }
    }
}

