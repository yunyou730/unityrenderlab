using System;
using UnityEngine;

namespace comet.combat
{
    public class TerrainHeightCtrl : ITerrainModifier,IDisposable
    {
        public ETerrainHeightCtrlType _terrainHeightCtrlType = ETerrainHeightCtrlType.None;
        
        private GameObject _selector = null;
        
        private CombatManager _combat = null;

        private Camera _camera = null;
        private MapRecord _mapRecord = null;

        public TerrainHeightCtrl(CombatManager combat)
        {
            _combat = combat;
        }

        public void Init(Camera mainCamera, MapRecord mapRecord)
        {
            _camera = mainCamera;
            _mapRecord = mapRecord;
        }

        public bool ShouldWorking()
        {
            return _terrainHeightCtrlType != ETerrainHeightCtrlType.None;
        }

        public void DoJob(int crossPointX, int crossPointY)
        {
            
        }
        
        public void Dispose()
        {
            
                    
        }
    }
}