using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace comet
{
    public class GridRecord
    {
        public enum EGridType
        {
            Plane,
            Wall,
            Water,
            Grass,
            Max,
        }
        
        public EGridType GridType => _gridType;
        public float Height => _height;
        public void SetHeight(float height) => _height = height;

        private EGridType _gridType = EGridType.Plane;
        private float _height = 0;
        
        public void SetGridType(EGridType gridType)
        {
            _gridType = gridType;
        }
    }   
}
