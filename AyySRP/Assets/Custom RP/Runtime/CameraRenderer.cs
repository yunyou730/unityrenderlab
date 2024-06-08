using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace ayy.srp
{
    public class CameraRenderer
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
        
        public void Render(ScriptableRenderContext context,Camera camera)
        {
            this._context = context;
            this._camera = camera;

            if (!Cull())
            {
                return;
            }

            Setup();
            DrawVisibleGeometry();
            Submit();
        }


        void Setup()
        {
            _context.SetupCameraProperties(_camera);   
            _buffer.ClearRenderTarget(true,true,Color.clear);   
            _buffer.BeginSample(kBufferName);
            ExecuteBuffer();
            
            
        }
        
        private void Submit()
        {
            _buffer.EndSample(kBufferName);
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
            // draw renderer of GameObjects in scene
            //var sortSettings = new SortingSettings(_camera);
            var sortSettings = new SortingSettings { criteria = SortingCriteria.CommonOpaque };
            var drawingSettings = new DrawingSettings(s_unlitShader, sortSettings);
            var filterSettings = new FilteringSettings(RenderQueueRange.opaque);
            _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filterSettings);

            // draw SkyBox of the scene
            _context.DrawSkybox(_camera);


            sortSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortSettings;
            filterSettings.renderQueueRange = RenderQueueRange.transparent;
            _context.DrawRenderers(_cullingResults,ref drawingSettings,ref filterSettings);


        }
    }
}

