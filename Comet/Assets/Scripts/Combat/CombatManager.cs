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

        private Config _config = null;

        public void Init(Camera mainCamera)
        {
            _cameraCtrl = new CameraCtrl();
            _cameraCtrl.Init(mainCamera);

            _config = Comet.Instance.ServiceLocator.Get<Config>();
        }

        public void OnUpdate(float deltaTime)
        {
            _cameraCtrl.OnUpdate(deltaTime);
        }

        public void CreateMapRecord()
        {
            _mapRecord = new MapRecord(kMapRows,kMapCols);
            _mapRecord.GenerateGrids(0,0);
            _mapRecord.RandomizeAllGridsType();
        }

        public void CreateGridMapGfx()
        {
            var resManager = Comet.Instance.ServiceLocator.Get<ResManager>();
            var prefab = resManager.Load<GameObject>("Prefabs/GridMap");
            _gridMap = GameObject.Instantiate(prefab).GetComponent<GridMap>();
            _gridMap.RefreshWithMapRecord(_mapRecord,_config.GridSize);
        }
        
        public void Dispose()
        {
            _cameraCtrl = null;
            _mapRecord?.Dispose();
        }
    }
}