using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace comet.combat
{
    public class GridRecord
    {
        public EGridType GridType => _gridType;
        public ETerrainTextureType[] TerrainTextureType => _terrainTextureType;
        
        public float Height => _height;
        public void SetHeight(float height) => _height = height;

        private EGridType _gridType = EGridType.Ground;
        private ETerrainTextureType[] _terrainTextureType = null;
        
        private float _height = 0;

        private int _flatTerrainTextureIndex = 15;  // random from [15,31] ,for terrain texture reason
        public int FlatTerrainTextureIndex => _flatTerrainTextureIndex;

        public GridRecord()
        {
            int kLayerCount = (int)ETerrainTextureLayer.Max;
            _terrainTextureType = new ETerrainTextureType[kLayerCount];
            _terrainTextureType[(int)ETerrainTextureLayer.BaseLayer] = ETerrainTextureType.Ground; 
            for (int layer = 1;layer < kLayerCount;layer++)
            {
                _terrainTextureType[layer] = ETerrainTextureType.None; 
            }
            InitFlagTerrainTextureIndex();
        }
        
        public void SetGridType(EGridType gridType)
        {
            _gridType = gridType;
        }

        public void SetTextureType(int layer,ETerrainTextureType textureType)
        {
            Debug.Assert(layer >= 0 && layer < 3);
            _terrainTextureType[layer] = textureType;
        }

        public ETerrainTextureType GetTerrainTexture(int layer)
        {
            Debug.Assert(layer >= 0 && layer < 3);
            return _terrainTextureType[layer];
        }

        private void InitFlagTerrainTextureIndex()
        {
            _flatTerrainTextureIndex = 15;
            float rate = Random.Range(0.0f, 1.0f);
            if (rate < 0.2f)
            {
                _flatTerrainTextureIndex = Random.Range(15, 32);
            }
        }
    }   
}
