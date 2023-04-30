using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

[ExecuteInEditMode]
public class RadialBlur : MonoBehaviour
{
    public Material _material = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
