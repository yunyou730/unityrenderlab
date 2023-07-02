using System;
using UnityEngine;

namespace Scenes.CircleWithNoise
{
    [ExecuteInEditMode]
    public class PostEffect : MonoBehaviour
    {
        public Material _material = null;
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_material != null)
            {
                Graphics.Blit(src,dest,_material);
            }
            else
            {
                Graphics.Blit(src,dest);
            }
        }
    }
}
