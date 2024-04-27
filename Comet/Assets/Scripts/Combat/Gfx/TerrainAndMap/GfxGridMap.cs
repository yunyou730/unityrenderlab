using System;
using System.Collections;
using System.Collections.Generic;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class GfxGridMap : MonoBehaviour
    {
        private ResManager _res = null;
        private Config _config = null;
        
        private MapRecord _mapRecord = null;
        public MapRecord MapRecord => _mapRecord;

        private MeshFilter _meshFilter = null;
        private MeshRenderer _meshRenderer = null;
        private MeshCollider _meshCollider = null;
        private Mesh _terrainMesh = null;
        
        private float _gridSize;
        private Material _material = null;

        private GfxMapMeshGenerator _meshGenerator = null;
        
        /*
         Base data texture.
         Channel R: is grid a blocker 
         */
        private Texture2D _gridTexture = null;
        private Color[] _gridData = null;
        public Texture2D GridTexture => _gridTexture;
        
        /*
         * Point data & point texture
         * Channel R: point texture type
         * Channel G: cliff level?
         */
        private Texture2D _mapPointsTexture = null;
        private Color[] _mapPointsData = null;
        
        /*
         * Render State Flags
         */
        private bool _bShowGrid = true;
        private bool _bShowBlock = false;
        private bool _bShowGridUV = false;
        private bool _bShowMapUV = false;
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();
            _material = _meshRenderer.material;

            _res = Comet.Instance.ServiceLocator.Get<ResManager>();
            _config = Comet.Instance.ServiceLocator.Get<Config>();
            
            gameObject.layer = LayerMask.NameToLayer(_config.kTerrainlayerName);
        }
        
        public void RefreshWithMapRecord(MapRecord mapRecord)
        {
            // hold data
            SetMapRecord(mapRecord);
            _gridSize = mapRecord.GridSize;
            
            // refresh gfx
            _meshGenerator = new GfxMapMeshGenerator(mapRecord, _gridSize);
            Mesh mesh = _meshGenerator.CreateMapMesh();
            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
            _terrainMesh = mesh;
            
            
            // Data Texture: blocker and height  
            _gridTexture = new Texture2D(_mapRecord.GridCols,_mapRecord.GridRows);
            _gridTexture.filterMode = FilterMode.Point;
            _gridData = new Color[_mapRecord.GridRows * _mapRecord.GridCols];
            
            // Data Texture: point cliff level, point texture type  
            _mapPointsTexture = new Texture2D(_mapRecord.GridCols + 1,_mapRecord.GridRows + 1);
            _mapPointsTexture.filterMode = FilterMode.Point;
            _mapPointsData = new Color[_mapRecord.PointsInRow * _mapRecord.PointsInCol];
            
            // terrain textures
            //var terrainTextureGrass = _res.Load<Texture2D>("Textures/TerrainTexture/debug");
            var terrainTextureGrass = _res.Load<Texture2D>("Textures/TerrainTexture/grass");
            var terrainTextureGround = _res.Load<Texture2D>("Textures/TerrainTexture/ground");
            var terrainTextureDirt = _res.Load<Texture2D>("Textures/TerrainTexture/dirt_rough");
            var terrainTextureBlight = _res.Load<Texture2D>("Textures/TerrainTexture/blight");
            
            _material.SetTexture(Shader.PropertyToID("_TerrainTextureGrass"),terrainTextureGrass);
            _material.SetTexture(Shader.PropertyToID("_TerrainTextureGround"),terrainTextureGround);
            _material.SetTexture(Shader.PropertyToID("_TerrainTextureDirt"),terrainTextureDirt);
            _material.SetTexture(Shader.PropertyToID("_TerrainTextureBlight"),terrainTextureBlight);
            
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
            _gridTexture.SetPixels(_gridData);
            _gridTexture.Apply();
            
            _mapPointsTexture.SetPixels(_mapPointsData);
            _mapPointsTexture.Apply();

        }
        
        public void BindDataTexturesToMaterial()
        {
            // Grid blockers textures
            _material.SetTexture(Shader.PropertyToID("_MapGridsDataTex"),_gridTexture);
            // Points Texture
            _material.SetTexture(Shader.PropertyToID("_MapPointsDataTex"),_mapPointsTexture);
        }

        private void RefreshColorDataAndTexturesForOneGrid(GridRecord gridRecord,int x,int y)
        {
            int pixelIndex = y * _mapRecord.GridCols + x;

            if (pixelIndex >= _gridData.Length)
            {
                Debug.Log("xxx");
            }

            // Data for each grid.
            // Channel R: Blocker or not
            // Channel G: When grid is flat, which tileSet index shall we use
            Color color = new Color(0, 0, 0, 1);
            color.r = gridRecord.GridType == EGridType.Ground ? 1.0f : 0.0f;
            color.g = gridRecord.FlatTerrainTextureIndex / 100.0f;
            //color.g = (float)gridRecord.FlatTerrainTextureIndex;
            _gridData[pixelIndex] = color;
        }

        private void RefreshColorDataAndTexturesForOnePoint(PointRecord pointRecord,int x,int y)
        {
            int pixelIndex = y * _mapRecord.PointsInCol + x;
            Color col = new Color(0, 0, 0, 1);
            switch (pointRecord.TerrainTextureType)
            {
                case ETerrainTextureType.Grass:
                    col.r = 0.1f;
                    break;
                case ETerrainTextureType.DirtRough:
                    col.r = 0.2f;
                    break;
                case ETerrainTextureType.Blight:
                    col.r = 0.3f;
                    break;
                default:
                    col.r = 0.0f;
                    break;
            }
            _mapPointsData[pixelIndex] = col;
        }

        private void OnDestroy()
        {
            _meshGenerator.Dispose();
            _meshGenerator = null;
            
            // Release Textures & Texture's Data
            Destroy(_gridTexture);
            _gridData = null;
            
            Destroy(_mapPointsTexture);
            _mapPointsData = null;
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
        
        public void ToggleShowGridUV()
        {
            _bShowGridUV = !_bShowGridUV;
            _material.SetFloat(Shader.PropertyToID("_TOGGLE_GRID_UV"),_bShowBlock?1.0f:0.0f);
        }
        
        public void ToggleShowMapUV()
        {
            _bShowMapUV = !_bShowMapUV;
            _material.SetFloat(Shader.PropertyToID("_TOGGLE_MAP_UV"),_bShowBlock?1.0f:0.0f);
        }
        
        public void RebuildMesh()
        {
            Debug.Log("Rebuild Mesh()");
            //_meshGenrator.AdjustTerrainMeshVerticesHeight(_meshFilter.sharedMesh);
            _meshGenerator.AdjustTerrainMeshVerticesHeight(_terrainMesh);

            // @miao @todo
        }
    }    
}


