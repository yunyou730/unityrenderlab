using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularNoise : MonoBehaviour
{
    public Material _material = null;

    protected float _screenWidth = 0.0f;
    protected float _screenHeight = 0.0f;
        
    private void Update()
    {
        if (_material != null)
        {
            _screenWidth = GetScreenSize().x;
            _screenHeight = GetScreenSize().y;
            float mx = Mathf.Clamp(Input.mousePosition.x / _screenWidth, 0.0f, 1.0f);
            float my = Mathf.Clamp(Input.mousePosition.y / _screenHeight, 0.0f, 1.0f);
            
            Vector4 mousePosition = new Vector4(mx,my, 0.0f,0.0f);
            _material.SetVector(Shader.PropertyToID("_MousePosition"),mousePosition);
        }
    }

    void HandleWindowSizeChange()
    {
        _screenWidth = GetScreenSize().x;
        _screenHeight = GetScreenSize().y;
    }
    
    Vector2 GetScreenSize()
    {
        return new Vector2(Screen.width, Screen.height);
    }

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
