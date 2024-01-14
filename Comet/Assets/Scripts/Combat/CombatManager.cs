using System;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class CombatManager : IDisposable
    {
        public const int kMapRows = 100;
        public const int kMapCols = 100;
        
        private MapRecord _mapRecord = null;
        private GridMap _gridMap = null;
        private CameraCtrl _cameraCtrl = null;
        
        public GridMap GridMap => _gridMap;
        public MapRecord MapRecord => _mapRecord;
        

        private World _world = null;

        public void Init(Camera mainCamera)
        {
            _cameraCtrl = new CameraCtrl();
            _cameraCtrl.Init(mainCamera);
            
            CreateMapRecord();
            
            _world = new GfxWorld(_mapRecord);
            _world.Init();
        }

        public void Start()
        {
            _world.Start();
        }

        public void OnUpdate(float deltaTime)
        {
            _cameraCtrl.OnUpdate(deltaTime);
            _world?.OnUpdate(deltaTime);
        }

        public void CreateMapRecord()
        {
            _mapRecord = new MapRecord(kMapRows,kMapCols);
            _mapRecord.GenerateGrids(0,0);
            _mapRecord.RandomizeAllGridsType();
        }
        
        public void Dispose()
        {
            _cameraCtrl = null;
            _mapRecord?.Dispose();
            _world.Dispose();
        }
    }
}