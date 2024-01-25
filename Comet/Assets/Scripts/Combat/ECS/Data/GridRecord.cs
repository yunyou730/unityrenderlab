using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace comet
{
    public class GridRecord
    {
        public enum EGridType
        {
            Ground,
            Wall,
            Water,
            Grass,
            Max,
        }

        public enum ETextureType
        {
            None,
            Ground,
            Grass,
        }

        public EGridType GridType => _gridType;
        public ETextureType TextureType => _textureType;
        
        public float Height => _height;
        public void SetHeight(float height) => _height = height;

        private EGridType _gridType = EGridType.Ground;
        private ETextureType _textureType = ETextureType.Ground;
        
        private float _height = 0;
        
        public void SetGridType(EGridType gridType)
        {
            _gridType = gridType;
        }
    }   
}
