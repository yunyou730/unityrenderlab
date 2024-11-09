using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PaintableMesh : MonoBehaviour
{
    public Vector3? _curDrawPointWS = null;
    public Vector3? _prevDrawPointsWS = null;
    
    //private RenderTexture _unwrapTexture = null; // public only for visualize debug in Inspector panel
    private Material _unwrapUVMaterial = null;
    
    private bool _bPresentSlot1 = true;
    private RenderTexture _unwrapTex1 = null;
    private RenderTexture _unwrapTex2 = null;
    
    void Start()
    {
        //_unwrapTexture = CreatePresentRenderTexture();
        _unwrapTex1 = CreatePresentRenderTexture();
        _unwrapTex2 = CreatePresentRenderTexture();
        
        // 设置最终 需要绘制的 material 
        var meshRenderer = GetComponent<MeshRenderer>();
        var material = meshRenderer.material;
        //material.SetTexture(Shader.PropertyToID("_PaintingChannel"),_unwrapTexture);
        material.SetTexture(Shader.PropertyToID("_PaintingChannel"),GetPresentUVTexture());
        
        // 初始化 unwrap uv 的 material 
        _unwrapUVMaterial = new Material(Shader.Find("ayy/ModelPaintingTest"));
    }
    
    private RenderTexture CreatePresentRenderTexture()
    {
        var rt = new RenderTexture(1024,1024,32,DefaultFormat.LDR);
        rt.enableRandomWrite = true;
        rt.filterMode = FilterMode.Point;
        return rt;
    }

    void Update()
    {
        if (_curDrawPointWS != null)
        {
            Vector4 posInWS = new Vector4(
                _curDrawPointWS.Value.x,
                _curDrawPointWS.Value.y,
                _curDrawPointWS.Value.z,
                1.0f
            );
            
            if(_unwrapUVMaterial.HasProperty(Shader.PropertyToID("_PaintingPoint")))
            {
                _unwrapUVMaterial.SetVector(Shader.PropertyToID("_PaintingPoint"),posInWS);   
            }
        }
        if (_unwrapUVMaterial.HasProperty(Shader.PropertyToID("_MainTex")))
        {
            _unwrapUVMaterial.SetTexture(Shader.PropertyToID("_MainTex"),GetPresentUVTexture());
        }        
    }

    // public RenderTexture GetUnwrapUVTexture()
    // {
    //     return _unwrapTexture;
    // }

    public RenderTexture GetPresentUVTexture()
    {
        return _bPresentSlot1 ? _unwrapTex1 : _unwrapTex2;
    }

    public RenderTexture GetBackupUVTexture()
    {
        return (!_bPresentSlot1) ? _unwrapTex1 : _unwrapTex2;
    }

    public void SwapUVTexture()
    {
        _bPresentSlot1 = !_bPresentSlot1;
    }

    public Material GetUnwrapUVMaterial()
    {
        return _unwrapUVMaterial;
    }
    
    public void SetCurrentDrawPointWS(Vector3 pos)
    {
        _curDrawPointWS = pos;
    }
    
    //public Vector3? _curDrawPointWS = null;
    //public Vector3? _prevDrawPointsWS = null;

    void OnDestroy()
    {
        /*
        if (_unwrapTexture != null)
        {
            _unwrapTexture.Release();
            Destroy(_unwrapTexture);
            _unwrapTexture = null;            
        }
        */
    }
}
