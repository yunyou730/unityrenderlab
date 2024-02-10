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

        private GfxMapMeshGenerator _meshGenrator = null;
        
        /*
         Base data texture.
         Channel R: is tile a blocker 
         Channel G: tile height
         */
        private Texture2D _blockerAndHeightTexture = null;
        private Color[] _blockerAndHeightData = null;
        public Texture2D BlockerAndHeightTexture => _blockerAndHeightTexture;
        
        
        /*
         * Point data & point texture
         * Channel R: point texture type
         * Channel G: cliff level?
         */
        private Texture2D _mapPointsTexture = null;
        private Color[] _mapPointsData = null;
        
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
            _meshGenrator = new GfxMapMeshGenerator(mapRecord, _gridSize);
            Mesh mesh = _meshGenrator.CreateMapMesh();
            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
            
            // Data Texture: blocker and height  
            _blockerAndHeightTexture = new Texture2D(_mapRecord.GridCols,_mapRecord.GridRows);
            _blockerAndHeightTexture.filterMode = FilterMode.Point;
            _blockerAndHeightData = new Color[_mapRecord.GridRows * _mapRecord.GridCols];
            
            // Data Texture: point cliff level, point texture type  
            _mapPointsTexture = new Texture2D(_mapRecord.GridCols + 1,_mapRecord.GridRows + 1);
            _mapPointsTexture.filterMode = FilterMode.Point;
            _mapPointsData = new Color[_mapRecord.PointsInRow * _mapRecord.PointsInCol];
            
            // Data Texture of Terrain 
            for (int i = 0;i < kTerrainLayers;i++)
            {
                _terrainDataTextures[i] = new Texture2D(_mapRecord.GridCols,_mapRecord.GridRows);
                _terrainDataTextures[i].filterMode = FilterMode.Point;
                _terrainColorData.Add(i, new Color[_mapRecord.GridRows * _mapRecord.GridCols]);
            }
            
            // terrain textures
            //var terrainTextureGrass = _res.Load<Texture2D>("Textures/TerrainTexture/debug");
            var terrainTextureGrass = _res.Load<Texture2D>("Textures/TerrainTexture/Grass");
            var terrainTextureGround = _res.Load<Texture2D>("Textures/TerrainTexture/Ground");
            _material.SetTexture(Shader.PropertyToID("_TerrainTextureGrass"),terrainTextureGrass);
            _material.SetTexture(Shader.PropertyToID("_TerrainTextureGround"),terrainTextureGround);
            
            // refresh texture data , and pass it to GPU 
            RefreshTexData();
            BindDataTexturesToMaterial();
        }

        public void GetGridCoordBy3DPos(Vector3 pos,out int x,out int y)
        {
            x = (int)Mathf.Floor((pos.x / _gridSize));
            y = (int)Mathf.Floor((pos.z / _gridSize));
            
            x = Mathf.Clamp(x, 0, _mapRecord.GridCols);
            y = Mathf.Clamp(y, 0, _mapRecord.GridRows);
        }

        private void SetMapRecord(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
        }
        

        public Rect GetBounds()
        {
            Rect rect = Rect.zero;
            rect.x = 0;
            rect.y = 0;
            rect.width = _mapRecord.GridCols * _gridSize;
            rect.height = _mapRecord.GridRows * _gridSize;
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
            // Refresh Grids & Points Texture 
            for (int y = 0;y < _mapRecord.PointsInRow;y++)
            {
                for (int x = 0;x < _mapRecord.PointsInCol;x++)
                {
                    // Grid Texture
                    if (y < _mapRecord.GridRows && x < _mapRecord.GridCols)
                    {
                        GridRecord gridRecord = _mapRecord.GetGridAt(y, x);
                        RefreshColorDataAndTexturesForOneGrid(gridRecord,x,y);
                    }
                
                    // Point Texture
                    PointRecord pointRecord = _mapRecord.GetPointAt(y, x);
                    RefreshColorDataAndTexturesForOnePoint(pointRecord, x, y);
                }
            }
            
            // Color Data to textures
            _blockerAndHeightTexture.SetPixels(_blockerAndHeightData);
            _blockerAndHeightTexture.Apply();
            
            for (int i = 0;i < kTerrainLayers;i++)
            {
                _terrainDataTextures[i].SetPixels(_terrainColorData[i]);
                _terrainDataTextures[i].Apply();
            }
            
            _mapPointsTexture.SetPixels(_mapPointsData);
            _mapPointsTexture.Apply();

        }
        
        public void BindDataTexturesToMaterial()
        {
            // Grid blockers textures
            _material.SetTexture(Shader.PropertyToID("_BlockerAndHeightDataTex"),_blockerAndHeightTexture);
            
            // Terrain Data Textures
            for (int i = 0;i < kTerrainLayers;i++)
            {
                string layerUniformName = "_TerrainLayer_" + i;
                _material.SetTexture(Shader.PropertyToID(layerUniformName),_terrainDataTextures[i]);
            }
            
            // Points Texture
            _material.SetTexture(Shader.PropertyToID("_MapPointsDataTex"),_mapPointsTexture);
        }

        private void RefreshColorDataAndTexturesForOneGrid(GridRecord gridRecord,int x,int y)
        {
            int pixelIndex = y * _mapRecord.GridRows + x;
            
            // blocker walkable data.
            // Channel R: Blocker or not
            // Channel G: Height value, temp only 0
            Color color = new Color(0, 0, 0, 1);
            color.r = gridRecord.GridType == EGridType.Ground ? 1.0f : 0.0f;
            color.g = 0.0f;
            _blockerAndHeightData[pixelIndex] = color;
            
            // layer data
            // Channel R: whether have Terrain Texture Type data
            // Channel G: When grid is in terrain texture center(not in corner), which UV index should we use
            for (int i = 0;i < kTerrainLayers;i++)
            {
                ETerrainTextureType gridTextureType = gridRecord.GetTerrainTexture(i);
                Color c = Color.black;
                c.r = gridTextureType == ETerrainTextureType.None ? 0.0f : 1.0f;
                c.g = gridRecord.FlagTerrainTextureIndex / 100.0f;
                
                _terrainColorData[i][pixelIndex] = c;
            }
        }

        private void RefreshColorDataAndTexturesForOnePoint(PointRecord pointRecord,int x,int y)
        {
            int pixelIndex = y * _mapRecord.PointsInRow + x;
            Color col = new Color(0, 0, 0, 1);
            col.r = pointRecord.TerrainTextureType == ETerrainTextureType.Grass ? 1.0f : 0.0f;
            _mapPointsData[pixelIndex] = col;
        }

        private void OnDestroy()
        {
            _meshGenrator.Dispose();
            _meshGenrator = null;
            
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


