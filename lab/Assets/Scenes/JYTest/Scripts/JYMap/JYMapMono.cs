using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ayy.jy.data;
using ayy.jy.record;
using Newtonsoft.Json;
using UnityEngine;

namespace ayy.jy
{
    public class JYMapMono : MonoBehaviour
    {
        enum LayerType
        {
            Ground = 0,         // 地面图块 TileId
            Building = 1,       // 建筑图块 TileId
            Decoration = 2,     // 装饰图块 TileId
            Event = 3,          // 事件索引 事件下标
            BuildingHeight = 4, // 建筑抬高量 像素
            DecorationHeight = 5,   // 装饰抬高量 像素
        }



        [SerializeField] private string _scenePath = "scene001/scene001.json";
        [SerializeField] // for debugging
        private Texture2D _atlasTexture = null;
        
        private string _sceneFullPath;
        private string _atlasFullPath;
        private string _atlasTextureFullPath;
        private string _sceneDirectory;
        
        private JYSubscene _subscene = null;
        private JYTileAtlas _tileAtlas = null;
        //private JYMapRecord _mapRecord = null;
        
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
            
            // _mapRecord = new JYMapRecord();
            // _mapRecord.TileCountX = _subscene.Size;
            // _mapRecord.TileCountY = _subscene.Size;
        }

        void Start()
        {
            _atlasTexture = LoadTexture(_atlasTextureFullPath);
            _material.SetTexture(Shader.PropertyToID("_MainTex"), _atlasTexture);
            //float ratio = (float)_atlasTexture.width / (float)_atlasTexture.height;
            //transform.localScale = new Vector3(ratio, 1, 1);
            //Debug.Log($"ratio:{ratio}");
            
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
            Mesh mesh = BuildGroundLayerMesh(_subscene.Size,_subscene.Size);
            _meshFilter.mesh = mesh;
        }

        private Mesh BuildGroundLayerMesh(int tilesX,int tilesY)
        {
            int quadCount = tilesX * tilesY;
            
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();
            List<Color> colors = new List<Color>();
            
            
            string groundKey = ((int)LayerType.Ground).ToString();
            int[][] tileIds  = _subscene.Grid[groundKey];
            for(int tileY = 0; tileY < tileIds.Length; tileY++)
            {
                int sizeX = tileIds[tileY].Length;
                for (int tileX = 0; tileX < sizeX; tileX++)
                {
                    int tileId = tileIds[tileY][tileX] / 2;
                    
                    if (!_tileAtlas.Tiles.TryGetValue(tileId.ToString(), out JYTile tile))
                    {
                        Debug.LogWarning($"missing tile at {tileId.ToString()},tileY:{tileY},tileX:{tileX}");
                        continue;
                    }
                    else
                    {
                        
                        // @miao @todo
                        // 这里还没有考虑进去 offset!
                        //tile.Uv
                        //JYTile tile = _tileAtlas.Tiles[tileId.ToString()];
                        // @miao @todo
                        float px, py;
                        JYMapUtils.GetCenterPositionAtTile(tileX,tileY,out px,out py);
                        
                        Vector3 lt = new Vector3(px, 0, py);
                        Vector3 rt = new Vector3(px + JYMapUtils.ISO_TILE_WIDTH, 0, py);
                        Vector3 lb = new Vector3(px, 0, py - JYMapUtils.ISO_TILE_HEIGHT);
                        Vector3 rb = new Vector3(px + JYMapUtils.ISO_TILE_WIDTH, 0, py - JYMapUtils.ISO_TILE_HEIGHT);
                        
                        int vertIndex = vertices.Count;
                        vertices.Add(lt);
                        vertices.Add(rt);
                        vertices.Add(lb);
                        vertices.Add(rb);

                        // @miao @todo
                        // UV 没有计算对
                        // 位置也没有考虑到 Offset!
                        
                        /*
                        uvs.Add(new Vector2(0,1));  // left top
                        uvs.Add(new Vector2(1,1));  // right top
                        uvs.Add(new Vector2(0,0));  // left bottom
                        uvs.Add(new Vector2(0,1));  // right bottom
                        */

                        int w = tile.Rect[2];
                        int h = tile.Rect[3];
                        float uvW = (float)w / (float)_atlasTexture.width;
                        float uvH = (float)h / (float)_atlasTexture.height;

                        float u = tile.Uv[0];
                        float v = tile.Uv[1];
                        
                        uvs.Add(new Vector2(u, v + uvH));
                        uvs.Add(new Vector2(u + uvW, v + uvH));
                        uvs.Add(new Vector2(u, v));
                        uvs.Add(new Vector2(u + uvW, v));
                        
                        
                        triangles.Add(vertIndex);
                        triangles.Add(vertIndex+1);
                        triangles.Add(vertIndex+2);
                        triangles.Add(vertIndex+1);
                        triangles.Add(vertIndex+3);
                        triangles.Add(vertIndex+2);
                    }
                }
            }
            
            
            //mesh.RecalculateNormals();
            //mesh.RecalculateBounds();
            
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0,uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();

            return mesh;
        }
    }
    
}