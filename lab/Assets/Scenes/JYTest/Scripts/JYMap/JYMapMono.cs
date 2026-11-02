using System.Collections;
using System.Collections.Generic;
using System.IO;
using ayy.jy.data;
using ayy.jy.record;
using Newtonsoft.Json;
using UnityEngine;

namespace ayy.jy
{
    public class JYMapMono : MonoBehaviour
    {
        [SerializeField] private string _scenePath = "scene001/scene001.json";
        [SerializeField] // for debugging
        private Texture2D _atlasTexture = null;
        
        private string _sceneFullPath;
        private string _atlasFullPath;
        private string _atlasTextureFullPath;
        private string _sceneDirectory;
        
        private JYSubscene _subscene = null;
        private JYTileAtlas _tileAtlas = null;
        private JYMapRecord _mapRecord = null;
        
        private MeshFilter _meshFilter = null;
        private MeshRenderer _meshRenderer = null;
        private Material _material = null;

        void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _material = _meshRenderer.material;
            
            _sceneFullPath = JYPaths.FullPath(_scenePath);            
            _subscene = JsonConvert.DeserializeObject<JYSubscene>(File.ReadAllText(_sceneFullPath));
            
            _sceneDirectory = Path.GetDirectoryName(_sceneFullPath);
            _atlasFullPath = Path.Combine(_sceneDirectory, _subscene.AtlasJson);
            _atlasTextureFullPath = Path.Combine(_sceneDirectory, _subscene.Atlas);
            
            Debug.Log($"path: {_sceneFullPath}, atlas: {_atlasFullPath},atlasTexture: {_atlasTextureFullPath}");
            _tileAtlas = JsonConvert.DeserializeObject<JYTileAtlas>(File.ReadAllText(_atlasFullPath));
            
            _mapRecord = new JYMapRecord();
            _mapRecord.TileCountX = _subscene.Size;
            _mapRecord.TileCountY = _subscene.Size;
        }

        void Start()
        {
            _atlasTexture = LoadTexture(_atlasTextureFullPath);
            _material.SetTexture(Shader.PropertyToID("_MainTex"), _atlasTexture);
            float ratio = (float)_atlasTexture.width / (float)_atlasTexture.height;
            transform.localScale = new Vector3(ratio, 1, 1);
            Debug.Log($"ratio:{ratio}");
            
            BuildMesh();
        }
        

        private Texture2D LoadTexture(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            tex.name = Path.GetFileName(path);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.anisoLevel = 0;
            tex.hideFlags = HideFlags.HideAndDontSave;
            
            return tex;
        }

        private void BuildMesh()
        {
            // @miao @todo
            // 参考 JYTilePreview, 加载 texture, 做对应 tile 的渲染
            
            //Mesh mesh = new Mesh();
            
        }
    }
    
}