using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace comet.combat
{
    public class PointRecord
    {
        private ETerrainTextureType _terrainTextureType = ETerrainTextureType.Ground;
        private float _terrainHeight = 0;
        private int _cliffLevel = 0;
        
        public ETerrainTextureType TerrainTextureType { get => _terrainTextureType; set => _terrainTextureType = value; }
        public float TerrainHeight { get => _terrainHeight; set => _terrainHeight = value; }
        public int CliffLevel { get => _cliffLevel; set => _cliffLevel = value; }
        
        public PointRecord()
        {
            _terrainHeight = 0.0f;
            _cliffLevel = 0;
            _terrainTextureType = ETerrainTextureType.Ground;
        }
        
        public void AddTerrainHeight(float deltaValue)
        {
            _terrainHeight += deltaValue;
        }


    }   
}
