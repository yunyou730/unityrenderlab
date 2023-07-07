using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ayy.pointsinpolygon
{
    [ExecuteInEditMode]
    public class PointsInPolygon : MonoBehaviour
    {
        [SerializeField]
        private Material _material = null;
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_material != null)
            {
                Graphics.Blit(src,dest,_material);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }   
}
