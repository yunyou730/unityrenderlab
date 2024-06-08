using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ayy.srp
{
    public class CustomRenderPipeline : RenderPipeline
    {
        private CameraRenderer _renderer = new CameraRenderer();
        
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            for (int i = 0;i < cameras.Length;i++)
            {
                _renderer.Render(context,cameras[i]);
            }

        }
    }
    
}
