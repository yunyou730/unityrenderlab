using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ayy.srp
{
    [CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
    public class CustomRenderPipelineAsset : RenderPipelineAsset
    {
        [SerializeField] private bool _useDynamicBatching = true;
        [SerializeField] private bool _useGPUInstancing = true;
        [SerializeField] private bool _useSRPBatcher = true;
        
        protected override RenderPipeline CreatePipeline()
        {
            return new CustomRenderPipeline(_useDynamicBatching,_useGPUInstancing,_useSRPBatcher);
        }
    }

}
