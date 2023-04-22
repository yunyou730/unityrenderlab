using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class LUTFilter : MonoBehaviour
{
    public Material _lutMaterial = null;
    public float _intensity = 1.0f;

    private void Update()
    {
        if (_lutMaterial != null)
        {
            
        }
    }

    void OnRenderImage(RenderTexture source,RenderTexture dest)
    {
        if (_lutMaterial != null)
        {
            _lutMaterial.SetTexture(Shader.PropertyToID("Main Texture"),source);
            Graphics.Blit(source,dest,_lutMaterial);
        }
        else
        {
            Graphics.Blit(source,dest);
        }
    }
}
