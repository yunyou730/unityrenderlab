using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ayy
{
    [ExecuteInEditMode]
    public class SDF2DV2 : MonoBehaviour
    {
        [SerializeField] private Material _material = null;
        
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

        public void OnDestroy()
        {
            GameObject.Destroy(_material);
            _material = null;
        }
    }
}
