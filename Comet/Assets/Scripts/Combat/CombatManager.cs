using System;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class CombatManager : IDisposable
    {
        private MapRecord _mapRecord = null;
        private GfxWorld _world = null;
        public GfxWorld World { get { return _world; } }

        private CameraCtrl _cameraCtrl = null;
        private ActorCtrl _actorCtrl = null;

        private Config _config = null;
        
        public void Init(Camera mainCamera)
        {
            _config = Comet.Instance.ServiceLocator.Get<Config>();
            
            // Create Map
            _mapRecord = CreateMapRecord();
            _world = new GfxWorld(_mapRecord);
            _world.Init();
            
            // Ctrl
            _cameraCtrl = new CameraCtrl();
            _actorCtrl = new ActorCtrl(this);
            _cameraCtrl.Init(mainCamera);
            _actorCtrl.Init(mainCamera);
        }

        public void Start()
        {
            _world.Start();
            _world.GetWorldComp<CreationComp>().AddCreationItem(ECreationType.Actor, 10, 12);
        }

        public void OnUpdate(float deltaTime)
        {
            _cameraCtrl.OnUpdate(deltaTime);
            _actorCtrl.OnUpdate(deltaTime);
            _world?.OnUpdate(deltaTime);
        }

        public MapRecord CreateMapRecord()
        {
            var mapRecord = new MapRecord(
                _config.DefaultGridMapRows,
                _config.DefaultGridMapCols,
                _config.DefaultGridSize);
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

        private void SpawnActor()
        {
            var creationComp = _world.GetWorldComp<CreationComp>();
            
        }
    }
}