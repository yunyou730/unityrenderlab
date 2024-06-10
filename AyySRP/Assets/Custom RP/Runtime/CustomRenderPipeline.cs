using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace ayy.srp
{
    public class CustomRenderPipeline : RenderPipeline
    {
        private CameraRenderer _renderer = new CameraRenderer();

        private bool _useDynamicBatching;
        private bool _useGPUInstancing;
        private bool _useSRPBatcher;


        public CustomRenderPipeline(bool useDynamicBatching,bool useGPUInstancing,bool useSRPBatcher)
        {
            _useDynamicBatching = useDynamicBatching;
            _useGPUInstancing = useGPUInstancing;
            _useSRPBatcher = useSRPBatcher;
            GraphicsSettings.useScriptableRenderPipelineBatching = _useSRPBatcher;
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            for (int i = 0;i < cameras.Length;i++)
            {
                _renderer.Render(context,cameras[i],_useDynamicBatching,_useGPUInstancing);
            }

        }
    }
    
}
