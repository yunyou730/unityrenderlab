using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PaintableMesh : MonoBehaviour
{
    [SerializeField] private bool _bBleedingEnable = true;
    [SerializeField] private bool _bDebugUnwrapUVEnable = false;
    
    public Vector3? _curDrawPointWS = null;
    public Vector3? _prevDrawPointsWS = null;
    
    private Material _unwrapUVMaterial = null;
    private RenderTexture _unwrapUVTex = null;
    private Material _3dMaterial = null;

    private Material _bleedingMaterial = null;
    private RenderTexture _bleedingTexture = null;
    
    void Start()
    {
        _unwrapUVTex = CreatePresentRenderTexture();
        _bleedingTexture = CreatePresentRenderTexture();
        
        // 设置最终 需要绘制的 material 
        var meshRenderer = GetComponent<MeshRenderer>();
        _3dMaterial = meshRenderer.material;
        RenderTexture presentTexture = _bBleedingEnable ? GetBleedingTexture() : GetPresentUVTexture();
        _3dMaterial.SetTexture(Shader.PropertyToID("_PaintingChannel"),presentTexture);
        
        // 初始化 unwrap uv 的 material 
        _unwrapUVMaterial = new Material(Shader.Find("ayy/UnwrapUVAndModelPainting"));
        _unwrapUVMaterial.SetTexture(Shader.PropertyToID("_AdditiveTexture"),GetPresentUVTexture());
        _unwrapUVMaterial.SetFloat(Shader.PropertyToID("_DebugUnWrapUV"),_bDebugUnwrapUVEnable ? 1.0f:0.0f);        
        
        // bleeding 材质, 用于给绘制的 texture 做 膨胀 
        _bleedingMaterial = new Material(Shader.Find("ayy/UVBleeding"));
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
        //Debug.Log($"[go]{gameObject.name}: from[A]{_curDrawPointWS} to[B]{_prevDrawPointsWS}");
        
        if (_curDrawPointWS != null)
        {
            Vector4 posInWS = new Vector4(
                _curDrawPointWS.Value.x,
                _curDrawPointWS.Value.y,
                _curDrawPointWS.Value.z,
                1.0f
            );


            Vector4 prevInWS = new Vector4(0, 0, 0, 1);
            float prevPointValid = 0.0f;
            if (_prevDrawPointsWS != null)
            {
                prevInWS = new Vector4(
                    _prevDrawPointsWS.Value.x,
                    _prevDrawPointsWS.Value.y,
                    _prevDrawPointsWS.Value.z,
                    1.0f
                );
                prevPointValid = 1.0f;
            }
            
            _unwrapUVMaterial.SetVector(Shader.PropertyToID("_PaintingPoint"),posInWS);
            _unwrapUVMaterial.SetFloat(Shader.PropertyToID("_PrevPointValid"),prevPointValid);
            _unwrapUVMaterial.SetVector(Shader.PropertyToID("_PrevPoint"),prevInWS);  
        }
        
        _unwrapUVMaterial.SetFloat(Shader.PropertyToID("_DebugUnWrapUV"),_bDebugUnwrapUVEnable ? 1.0f:0.0f);
        
        RenderTexture presentTexture = _bBleedingEnable ? GetBleedingTexture() : GetPresentUVTexture();
        _3dMaterial.SetTexture(Shader.PropertyToID("_PaintingChannel"),presentTexture);        
    }

    public RenderTexture GetPresentUVTexture()
    {
        return _unwrapUVTex;
    }

    public RenderTexture GetBleedingTexture()
    {
        return _bleedingTexture;
    }

    public Material GetUnwrapUVMaterial()
    {
        return _unwrapUVMaterial;
    }

    public Material GetBleedingMaterial()
    {
        return _bleedingMaterial;
    }

    public bool IsBleedingEnable()
    {
        return _bBleedingEnable;
    }

    public void SetBleedingEnable(bool bEnable)
    {
        _bBleedingEnable = bEnable;
    }

    public void SetCurrentDrawPointWS(Vector3 pos)
    {
        // 把 上一次的绘制位置, 记录到 prevDrawPoint 里 
        if (_curDrawPointWS != null)
        {
            _prevDrawPointsWS = new Vector3(
                _curDrawPointWS.Value.x,
                _curDrawPointWS.Value.y,
                _curDrawPointWS.Value.z); 
        }
        
        // 记录 当前绘制位置 到 curDrawPoint 里  
        _curDrawPointWS = pos;
    }

    public void ClearDrawPoints()
    {
        _curDrawPointWS = null;
        _prevDrawPointsWS = null;
    }

    void OnDestroy()
    {
        if (_unwrapUVTex != null)
        {
            _unwrapUVTex.Release();
            Destroy(_unwrapUVTex);
            _unwrapUVTex = null;            
        }
    }
}
