using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ayy
{
    [ExecuteInEditMode]
    public class RayMarchingInScene : MonoBehaviour
    {
        [SerializeField]
        protected Material _material = null;
        
        void Start()
        {
            
        }

        void Update()
        {
            
        }

        public void OnRenderImage(RenderTexture src, RenderTexture dest)
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

