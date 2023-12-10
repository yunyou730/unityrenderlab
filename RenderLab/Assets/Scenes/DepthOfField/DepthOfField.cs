using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ayy
{
    [ExecuteInEditMode]
    public class DepthOfField : MonoBehaviour
    {
        [SerializeField,HideInInspector]
        private Shader _shader;

        private Material _material = null;

        [SerializeField,Range(0.1f,100f)] 
        public float focusDistance = 10;
        
        [SerializeField, Range(0.1f, 10f)] 
        public float focusRange = 3.0f;

        [SerializeField,Range(0.0f,10f)] 
        private float _blurKernelSize = 1.0f;

        [Range(0.0f,1.0f)]
        public float _BlurRadius = 0.5f;
        
        
        // CoC pass index = 0
        private const int circleOfConfusionPass = 0;
        // Bokeh pass index = 1
        private const int bokehPass = 1;


        void Start()
        {
            _material = new Material(_shader);
        }
        
        void Update()
        {
        
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            RenderTexture coc = RenderTexture.GetTemporary(
                src.width, src.height, 0,
                RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
            );
            
            _material.SetFloat(Shader.PropertyToID("_FocusDistance"), focusDistance);
            _material.SetFloat(Shader.PropertyToID("_FocusRange"),focusRange);
            _material.SetFloat(Shader.PropertyToID(("_blurKernelSize")),_blurKernelSize);
            _material.SetFloat(Shader.PropertyToID(("_BlurRadius")),_BlurRadius);
            
            
            // specify which material and which pass we are using in the material
            Graphics.Blit(src,coc,_material,circleOfConfusionPass);
            Graphics.Blit(src,dest,_material,bokehPass);
            //Graphics.Blit(coc,dest);
            
            RenderTexture.ReleaseTemporary(coc);

        }
    }

}

