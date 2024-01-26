using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace comet.combat
{
    public class GridRecord
    {
        public EGridType GridType => _gridType;
        public EGridTextureType[] TerrainTextureType => _terrainTextureType;
        
        public float Height => _height;
        public void SetHeight(float height) => _height = height;

        private EGridType _gridType = EGridType.Ground;
        private EGridTextureType[] _terrainTextureType = null;
        
        private float _height = 0;

        public GridRecord()
        {
            _terrainTextureType = new EGridTextureType[3];
            _terrainTextureType[0] = EGridTextureType.Ground;
            _terrainTextureType[1] = EGridTextureType.Ground;
            _terrainTextureType[2] = EGridTextureType.Ground;
        }
        
        public void SetGridType(EGridType gridType)
        {
            _gridType = gridType;
        }

        public void SetTextureType(int layer,EGridTextureType textureType)
        {
            Debug.Assert(layer >= 0 && layer < 3);
            _terrainTextureType[layer] = textureType;
        }

        public EGridTextureType GetTerrainTexture(int layer)
        {
            Debug.Assert(layer >= 0 && layer < 3);
            return _terrainTextureType[layer];
        }
    }   
}
