using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ayy.srp
{
    public partial class CameraRenderer
    {
        #if UNITY_EDITOR
        
        static ShaderTagId[] s_legacyShaderTagIds = {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM")
        };

        private static Material _errorMaterial;

        private void PrepareBuffer()
        {
            _buffer.name = _sampleName = "[ayy]" + _camera.name;
        }

        private void PrepareForSceneWindow()
        {
            if (_camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
            }
        }

        private void DrawUnsupportedSettings()
        {
            if (_errorMaterial == null)
            {
                _errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
            }

            var drawingSettings = new DrawingSettings(s_legacyShaderTagIds[0], new SortingSettings(_camera))
            {
                overrideMaterial = _errorMaterial
            };
            
            for (int i = 1;i < s_legacyShaderTagIds.Length;i++) // from 2nd pass
            {
                drawingSettings.SetShaderPassName(i, s_legacyShaderTagIds[i]);
            }
            var filterSettings = FilteringSettings.defaultValue;
            _context.DrawRenderers(_cullingResults,ref drawingSettings,ref filterSettings);
        }

        private void DrawGizmos()
        {
            if (Handles.ShouldRenderGizmos())
            {
                _context.DrawGizmos(_camera,GizmoSubset.PreImageEffects);
                _context.DrawGizmos(_camera,GizmoSubset.PostImageEffects);
            }
        }

#endif
    }

}
