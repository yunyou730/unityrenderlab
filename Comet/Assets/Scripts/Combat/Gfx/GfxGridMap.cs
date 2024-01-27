using System;
using System.Collections;
using System.Collections.Generic;
using comet.res;
using Unity.VisualScripting;
using UnityEngine;

namespace comet.combat
{
    public class GfxGridMap : MonoBehaviour
    {
        private ResManager _res = null;
        
        private MapRecord _mapRecord = null;
        public MapRecord MapRecord => _mapRecord;

        private MeshFilter _meshFilter = null;
        private MeshRenderer _meshRenderer = null;
        private MeshCollider _meshCollider = null;
        
        private float _gridSize;
        private Material _material = null;
        
        /*
         Base data texture.
         Channel R: is tile a blocker 
         Channel G: tile height
         */
        private Texture2D _blockerAndHeightTexture = null;
        private Color[] _blockerAndHeightData = null;
        public Texture2D BlockerAndHeightTexture => _blockerAndHeightTexture;
        
        /*
         * Terrain data textures
         */
        private const int kTerrainLayers = (int)ETerrainTextureLayer.Max;
        private Texture2D[] _terrainDataTextures = new Texture2D[kTerrainLayers];
        private Dictionary<int, Color[]> _terrainColorData = new Dictionary<int, Color[]>();
        public Texture2D[] TerrainDataTextures => _terrainDataTextures;
        
        /*
         * Render State Flags
         */
        private bool _bShowGrid = true;
        private bool _bShowBlock = false;
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();
            _material = _meshRenderer.material;

            _res = Comet.Instance.ServiceLocator.Get<ResManager>();
        }
        
        public void RefreshWithMapRecord(MapRecord mapRecord)
        {
            // hold data
            SetMapRecord(mapRecord);
            _gridSize = mapRecord.GridSize;
            
            // refresh gfx
            Mesh mesh = CreateMapMesh();
            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
            
            // Data Texture :blocker and height  
            _blockerAndHeightTexture = new Texture2D(_mapRecord.Cols,_mapRecord.Rows);
            _blockerAndHeightTexture.filterMode = FilterMode.Point;
            _blockerAndHeightData = new Color[_mapRecord.Rows * _mapRecord.Cols];
            
            // Data Texture of Terrain 
            for (int i = 0;i < kTerrainLayers;i++)
            {
                _terrainDataTextures[i] = new Texture2D(_mapRecord.Cols,_mapRecord.Rows);
                _terrainDataTextures[i].filterMode = FilterMode.Point;
                _terrainColorData.Add(i, new Color[_mapRecord.Rows * _mapRecord.Cols]);
            }
            
            // terrain textures
            //var terrainTextureGrass = _res.Load<Texture2D>("Textures/TerrainTexture/debug");
            var terrainTextureGrass = _res.Load<Texture2D>("Textures/TerrainTexture/Grass");
            var terrainTextureGround = _res.Load<Texture2D>("Textures/TerrainTexture/Ground");
            _material.SetTexture(Shader.PropertyToID("_TerrainGrass"),terrainTextureGrass);
            _material.SetTexture(Shader.PropertyToID("_TerrainGround"),terrainTextureGround);
            
            // refresh texture data , and pass it to GPU 
            RefreshTexData();
            BindDataTexturesToMaterial();
        }

        public void GetGridCoordBy3DPos(Vector3 pos,out int x,out int y)
        {
            x = (int)Mathf.Floor((pos.x / _gridSize));
            y = (int)Mathf.Floor((pos.z / _gridSize));
            
            x = Mathf.Clamp(x, 0, _mapRecord.Cols);
            y = Mathf.Clamp(y, 0, _mapRecord.Rows);
        }

        private void SetMapRecord(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
        }
        
        private Mesh CreateMapMesh()
        {
            Vector3[] vertices;
            Vector2[] uvs;
            int[] indices;
            Color[] colors;
            Vector2[] uvs2;
            
            GenerateMeshData(out vertices,out uvs,out uvs2,out indices,out colors);
            
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0,uvs);
            mesh.SetUVs(1,uvs2);
            mesh.SetIndices(indices,MeshTopology.Triangles,0);
            mesh.SetColors(colors);
            
            return mesh;
        }

        private void GenerateMeshData(out Vector3[] vertices,
            out Vector2[] uvs,
            out Vector2[] uvs2,
            out int[] indices,
            out Color[] colors)
        {
            int verticesNum = _mapRecord.Rows * _mapRecord.Cols * 4;
            vertices = new Vector3[verticesNum]; 
            uvs = new Vector2[verticesNum];
            indices = new int[verticesNum * 6];
            colors = new Color[verticesNum];
            uvs2 = new Vector2[verticesNum];
            
            int vertIdx = 0;
            int indiceIdx = 0;
            for (int row = 0;row < _mapRecord.Rows;row++)
            {
                for (int col = 0; col < _mapRecord.Cols; col++)
                {
                    // decide height of grid each vertices
                    GridRecord gridRecord = _mapRecord.GetGridAt(row, col);

                    GridRecord north = _mapRecord.GetGridAt(row + 1, col);
                    GridRecord westNorth = _mapRecord.GetGridAt(row + 1, col - 1);
                    GridRecord eastNorth = _mapRecord.GetGridAt(row + 1, col + 1);
                    GridRecord west = _mapRecord.GetGridAt(row, col - 1);
                    GridRecord east = _mapRecord.GetGridAt(row, col + 1);
                    GridRecord south = _mapRecord.GetGridAt(row - 1, col);
                    GridRecord westSouth = _mapRecord.GetGridAt(row - 1, col - 1);
                    GridRecord eastSouth = _mapRecord.GetGridAt(row - 1, col + 1);

                    float y0 = GetGridsMaxHeight(gridRecord, west, westSouth, south);
                    float y1 = GetGridsMaxHeight(gridRecord, east, eastSouth, south);
                    float y2 = GetGridsMaxHeight(gridRecord, west, westNorth, north);
                    float y3 = GetGridsMaxHeight(gridRecord, east, eastNorth, north);
                    // y0 = y1 = y2 = y3 = gridRecord.Height 
                    
                    
                    /*
                     * 2 - 3
                     * |   |
                     * 0 - 1 
                     */

                    // vert position
                    float baseX = col * _gridSize;
                    float baseZ = row * _gridSize;
                    vertices[vertIdx] = new Vector3(baseX, y0, baseZ);
                    vertices[vertIdx + 1] = new Vector3(baseX + _gridSize, y1, baseZ);
                    vertices[vertIdx + 2] = new Vector3(baseX, y2, baseZ + _gridSize);
                    vertices[vertIdx + 3] = new Vector3(baseX + _gridSize, y3, baseZ + _gridSize);
                    
                    // vert uvs 1 , uv in grid
                    uvs[vertIdx] = new Vector2(0, 0);
                    uvs[vertIdx + 1] = new Vector2(1, 0);
                    uvs[vertIdx + 2] = new Vector2(0, 1);
                    uvs[vertIdx + 3] = new Vector2(1, 1);
                    
                    // vert uvs 2 , uv in whole mesh
                    uvs2[vertIdx] = new Vector2(
                        (float)col / (float)_mapRecord.Cols,
                        (float)row / (float)_mapRecord.Rows);
                    uvs2[vertIdx + 1] = new Vector2(
                        (float)(col + 1) / (float)_mapRecord.Cols,
                        (float)row / (float)_mapRecord.Rows);
                    uvs2[vertIdx + 2] = new Vector2(
                        (float)(col) / (float)_mapRecord.Cols,
                        (float)(row + 1)/ (float)_mapRecord.Rows);                    
                    uvs2[vertIdx + 3] = new Vector2(
                        (float)(col + 1) / (float)_mapRecord.Cols,
                        (float)(row + 1)/ (float)_mapRecord.Rows);    

                    // vert color , pass Grid Type data to vertices attribute
                    Color color = Color.white;
                    
                    switch (gridRecord.GridType)
                    {
                        case EGridType.Grass:
                            color = Color.green;
                            break;
                        case EGridType.Ground:
                            color = Color.yellow;
                            break;
                        case EGridType.Wall:
                            color = Color.gray;
                            break;
                        case EGridType.Water:
                            color = Color.cyan;
                            break;
                    }
                    colors[vertIdx] = colors[vertIdx + 1] = colors[vertIdx + 2] = colors[vertIdx + 3] = color;


                    // indices
                    indices[indiceIdx++] = vertIdx;
                    indices[indiceIdx++] = vertIdx + 2;
                    indices[indiceIdx++] = vertIdx + 1;
                    indices[indiceIdx++] = vertIdx + 1;
                    indices[indiceIdx++] = vertIdx + 2;
                    indices[indiceIdx++] = vertIdx + 3;
                    
                    // next vert group 
                    vertIdx += 4;
                }
            }
        }

        private float GetGridsMaxHeight(GridRecord g,GridRecord g1,GridRecord g2,GridRecord g3)
        {
            float h = g.Height;
            if (g1 != null && g1.Height > h) h = g1.Height;
            if (g2 != null && g2.Height > h) h = g2.Height;
            if (g3 != null && g3.Height > h) h = g3.Height;
            return h;
        }

        public Rect GetBounds()
        {
            Rect rect = Rect.zero;
            rect.x = 0;
            rect.y = 0;
            rect.width = _mapRecord.Cols * _gridSize;
            rect.height = _mapRecord.Rows * _gridSize;
            return rect;
        }
        
        /*
         * Texture has multiple meanings 
         * Texture Pixel Type: RGBA
         * R: >= 0.5 can walk;  < 0.5 can not walk
         * G: >= 0.5 ground texture; < 0.5 no ground texture
         * B: >= 0.5 grass texture; < 0.5 no ground texture
         * A: temp for the 3rd terrain texture 
         */
        public void RefreshTexData()
        {
            // Refresh Colors
            for (int y = 0;y < _mapRecord.Rows;y++)
            {
                for (int x = 0;x < _mapRecord.Cols;x++)
                {
                    GridRecord gridRecord = _mapRecord.GetGridAt(y, x);
                    RefreshColorDataAndTexturesForOneGrid(gridRecord,x,y);
                }
            }
            
            // Colors to texture
            _blockerAndHeightTexture.SetPixels(_blockerAndHeightData);
            _blockerAndHeightTexture.Apply();
            for (int i = 0;i < kTerrainLayers;i++)
            {
                _terrainDataTextures[i].SetPixels(_terrainColorData[i]);
                _terrainDataTextures[i].Apply();
            }
        }
        
        public void BindDataTexturesToMaterial()
        {
            _material.SetTexture(Shader.PropertyToID("_BlockerAndHeightDataTex"),_blockerAndHeightTexture);
            for (int i = 0;i < kTerrainLayers;i++)
            {
                string layerUniformName = "_TerrainLayer_" + i;
                _material.SetTexture(Shader.PropertyToID(layerUniformName),_terrainDataTextures[i]);
            }
        }

        private void RefreshColorDataAndTexturesForOneGrid(GridRecord gridRecord,int x,int y)
        {
            int pixelIndex = y * _mapRecord.Rows + x;
            
            // blocker walkable data.
            // Channel R: Blocker or not
            // Channel G: Height value, temp only 0
            Color color = new Color(0, 0, 0, 1);
            color.r = gridRecord.GridType == EGridType.Ground ? 1.0f : 0.0f;
            color.g = 0.0f;
            _blockerAndHeightData[pixelIndex] = color;
            
            // layer data
            // Channel R: whether have data
            // Channel G: When grid is in terrain texture center(not in corner), which UV index should we use
            for (int i = 0;i < kTerrainLayers;i++)
            {
                EGridTextureType gridTextureType = gridRecord.GetTerrainTexture(i);
                Color c = Color.black;
                c.r = gridTextureType == EGridTextureType.None ? 0.0f : 1.0f;
                c.g = gridRecord.FlagTerrainTextureIndex / 100.0f;
                
                _terrainColorData[i][pixelIndex] = c;
            }
        }
        
        private void OnDestroy()
        {
            // Release Blocker And Height Data Texture 
            GameObject.Destroy(_blockerAndHeightTexture);
            _blockerAndHeightData = null;
            
            // Release Terrain Data Textures
            for (int i = 0;i < kTerrainLayers;i++)
            {
                GameObject.Destroy(_terrainDataTextures[i]);
            }
            _terrainDataTextures = null;
            _terrainColorData = null;
        }
        
        public void ToggleShowGrid()
        {
            _bShowGrid = !_bShowGrid;
            _material.SetFloat(Shader.PropertyToID("_TOGGLE_GRID_LINE"),_bShowGrid?1.0f:0.0f);
        }

        public void ToggleShowBlock()
        {
            _bShowBlock = !_bShowBlock;
            _material.SetFloat(Shader.PropertyToID("_TOGGLE_WALKABLE"),_bShowBlock?1.0f:0.0f);
        }
    }    
}


