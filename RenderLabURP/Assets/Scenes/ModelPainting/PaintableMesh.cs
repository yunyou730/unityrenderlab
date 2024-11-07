using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PaintableMesh : MonoBehaviour
{
    public Vector3? DrawPointWorldPos = null;
    
    public RenderTexture _unwrapTexture = null; // public only for visualize debug in Inspector panel
    private Material _unwrapUVMaterial = null;
    
    void Start()
    {
        _unwrapTexture = new RenderTexture(1024,1024,32,DefaultFormat.LDR);
        _unwrapTexture.enableRandomWrite = true;
        
        // 设置最终 需要绘制的 material 
        var meshRenderer = GetComponent<MeshRenderer>();
        var material = meshRenderer.material;
        material.SetTexture(Shader.PropertyToID("_PaintingChannel"),_unwrapTexture);
        
        // 初始化 unwrap uv 的 material 
        _unwrapUVMaterial = new Material(Shader.Find("ayy/ModelPaintingTest"));
    }
    
    void Update()
    {
        if (DrawPointWorldPos != null)
        {
            Vector4 posInWS = new Vector4(
                DrawPointWorldPos.Value.x,
                DrawPointWorldPos.Value.y,
                DrawPointWorldPos.Value.z,
                1.0f
            );
            
            if(_unwrapUVMaterial.HasProperty(Shader.PropertyToID("_PaintingPoint")))
            {
                _unwrapUVMaterial.SetVector(Shader.PropertyToID("_PaintingPoint"),posInWS);   
            }
        }
    }

    public RenderTexture GetUnwrapUVTexture()
    {
        return _unwrapTexture;
    }
    
    public Material GetUnwrapUVMaterial()
    {
        return _unwrapUVMaterial;
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
