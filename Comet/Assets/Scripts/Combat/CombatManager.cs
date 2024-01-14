using System;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class CombatManager : IDisposable
    {
        public const int kMapRows = 100;
        public const int kMapCols = 100;
        
        private CameraCtrl _cameraCtrl = null;

        private MapRecord _mapRecord = null;
        private GfxWorld _world = null;
        
        public void Init(Camera mainCamera)
        {
            _cameraCtrl = new CameraCtrl();
            _cameraCtrl.Init(mainCamera);
            
            // Create Map
            _mapRecord = CreateMapRecord();
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

        public MapRecord CreateMapRecord()
        {
            var mapRecord = new MapRecord(kMapRows,kMapCols);
            mapRecord.GenerateGrids(0,0);
            mapRecord.RandomizeAllGridsType();
            return mapRecord;
        }
        
        public void Dispose()
        {
            _cameraCtrl = null;
            _mapRecord?.Dispose();
            _world.Dispose();
        }

        public GridMap GetGridMap()
        {
            return _world.GridMap;
        }
    }
}