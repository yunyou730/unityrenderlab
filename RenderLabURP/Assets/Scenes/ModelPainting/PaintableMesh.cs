using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PaintableMesh : MonoBehaviour
{
    [SerializeField] private bool _bBleedingEnable = true;
    
    public Vector3? _curDrawPointWS = null;
    public Vector3? _prevDrawPointsWS = null;
    
    private Material _unwrapUVMaterial = null;
    private RenderTexture _unwrapUVTex = null;
    private Material _3dMaterial = null;

    private Material _bleedingMaterial = null;
    private RenderTexture _bleedingTexture = null;

    private bool HasTextureInit = false;

    private bool _bNeedUpdateMeshCollider = false;
    private Mesh _colliderMesh = null;
    
    public static Color kUVBgClearColor = new Color(0,0,0,0);
    public static Color kUVMeshPartClearColor = new Color(0, 0, 0, 0);
    
    void Start()
    {
        _unwrapUVTex = CreatePresentRenderTexture();
        _bleedingTexture = CreatePresentRenderTexture();
        
        // 设置最终 需要绘制的 material 
        Renderer renderer = GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = GetComponent<SkinnedMeshRenderer>();
        }
        _3dMaterial = renderer.material;
        RenderTexture presentTexture = _bBleedingEnable ? GetBleedingTexture() : GetPresentUVTexture();
        _3dMaterial.SetTexture(Shader.PropertyToID("_PaintingChannel"),presentTexture);
        
        // 初始化 unwrap uv 的 material 
        _unwrapUVMaterial = new Material(Shader.Find("ayy/UnwrapUVAndModelPainting"));
        _unwrapUVMaterial.SetTexture(Shader.PropertyToID("_AdditiveTexture"),GetPresentUVTexture());
        //_unwrapUVMaterial.SetFloat(Shader.PropertyToID("_ShowUnwrapUVDirectly"), 1.0f);
        _unwrapUVMaterial.SetColor(Shader.PropertyToID("_MeshPartClearColor"),kUVMeshPartClearColor);
        
        // bleeding 材质, 用于给绘制的 texture 做 膨胀 
        _bleedingMaterial = new Material(Shader.Find("ayy/UVBleeding"));
        
        // 是否要 更新 mesh collider 
        // 因为 Skinned Mesh Renderer 每一帧都根据动画在改变 Mesh 的形态,
        // 而 MeshCollider 并不会 跟随 动画改变 mesh 形态 , 
        // 因此 需要 让 Mesh Collider 每一帧都同步 Skinned Mesh 才能正常响应 鼠标点击事件 
        _bNeedUpdateMeshCollider = GetComponent<SkinnedMeshRenderer>() != null && GetComponent<MeshCollider>() != null;
        if (_bNeedUpdateMeshCollider)
        {
            _colliderMesh = new Mesh();
        }
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
        
        // 第1帧 输出 uv展开的 texture; 从第2帧开始, 在第1帧基础上 累加 绘制轨迹图像 
        _unwrapUVMaterial.SetFloat(Shader.PropertyToID("_ShowUnwrapUVDirectly"),!HasTextureInit ? 1.0f:0.0f);
        if (!HasTextureInit)
        { 
            HasTextureInit = true;
        }
        
        // 动态切换 是否展示为 uv-bleeding 的贴图 
        RenderTexture presentTexture = _bBleedingEnable ? GetBleedingTexture() : GetPresentUVTexture();
        _3dMaterial.SetTexture(Shader.PropertyToID("_PaintingChannel"),presentTexture);
        
        // 更新 Skinned Mesh  & Mesh Collider
        if (_bNeedUpdateMeshCollider)
        {
            var skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            var meshCollider = GetComponent<MeshCollider>();
            skinnedMeshRenderer.BakeMesh(_colliderMesh);
            meshCollider.sharedMesh = _colliderMesh;
        }
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

    public void SetBrushSize(float brushSize)
    {
        _unwrapUVMaterial.SetFloat(Shader.PropertyToID("_BrushSize"),brushSize);
    }

    public void SetBrushColor(Color color)
    {
        _unwrapUVMaterial.SetColor(Shader.PropertyToID("_BrushColor"),color);
        //_3dMaterial.SetColor(Shader.PropertyToID("_PaintingColor"),color);
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
