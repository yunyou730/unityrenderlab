using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PaintableMesh : MonoBehaviour
{
    private RenderTexture _unwrapTexture = null;
    void Start()
    {
        _unwrapTexture = new RenderTexture(1024,1024,32,GraphicsFormat.R8G8B8A8_UInt);
        _unwrapTexture.enableRandomWrite = true;
    }
    
    void Update()
    {
        
    }

    public RenderTexture GetUnwrapUVTexture()
    {
        return _unwrapTexture;
    }

    void OnDestroy()
    {
        if (_unwrapTexture != null)
        {
            _unwrapTexture.Release();
            Destroy(_unwrapTexture);
            _unwrapTexture = null;            
        }
    }
}
