using System;
using UnityEngine;

namespace Scenes.SDF2D
{
    [ExecuteInEditMode]
    public class SDF2D : MonoBehaviour
    {
        public Material _material = null;
        
        [SerializeField] [Range(0.0f,1.0f)]
        public float tValue = 0.5f;
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_material != null)
            {
                _material.SetFloat(Shader.PropertyToID("_MyFloat"),tValue);
                Graphics.Blit(src,dest,_material);
            }
            else
            {
                Graphics.Blit(src,dest);
            }
        }
    }
}
