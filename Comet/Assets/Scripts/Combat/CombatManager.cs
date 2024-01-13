using System;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class CombatManager : IDisposable
    {
        public const float kGridSize = 1.0f;
        public const int kMapRows = 100;
        public const int kMapCols = 100;
        
        private MapRecord _mapRecord = null;
        private GridMap _gridMap = null;
        
        public GridMap GridMap => _gridMap;
        public MapRecord MapRecord => _mapRecord;
        
        public void CreateMapRecord()
        {
            _mapRecord = new MapRecord(kMapRows,kMapCols);
            _mapRecord.GenerateGrids(0,0);   
        }

        public void CreateGridMapGfx()
        {
            var resManager = Comet.Instance.ServiceLocator.Get<ResManager>();
            var prefab = resManager.Load<GameObject>("Prefabs/GridMap");
            var gridMap = GameObject.Instantiate(prefab).GetComponent<GridMap>();
            gridMap.RefreshWithMapRecord(_mapRecord,kGridSize);
        }
        
        public void Dispose()
        {
            _mapRecord?.Dispose();
        }
    }
}