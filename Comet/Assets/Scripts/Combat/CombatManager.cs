using System;
using comet.res;
using UnityEngine;
using UnityEngine.UI;

namespace comet.combat
{
    public class CombatManager : IDisposable
    {
        private MapRecord _mapRecord = null;
        private GfxWorld _world = null;
        public GfxWorld World { get { return _world; } }
        
        private CameraCtrl _cameraCtrl = null;
        private ActorCtrl _actorCtrl = null;
        private TerrainTextureCtrl _terrainTextureCtrl = null;

        public TerrainTextureCtrl TerrainTextureCtrl => _terrainTextureCtrl;

        private Config _config = null;
        
        public void Init(Camera mainCamera,Image imgMiniMap)
        {
            _config = Comet.Instance.ServiceLocator.Get<Config>();
            
            // Create Map
            _mapRecord = CreateMapRecord();
            _world = new GfxWorld(_mapRecord,imgMiniMap);
            _world.Init();
            
            // Ctrl
            _cameraCtrl = new CameraCtrl();
            _actorCtrl = new ActorCtrl(this);
            _terrainTextureCtrl = new TerrainTextureCtrl(this);
            _cameraCtrl.Init(mainCamera);
            _actorCtrl.Init(mainCamera);
            _terrainTextureCtrl.Init(mainCamera,_mapRecord);
        }

        public void Start()
        {
            _world.Start();
            _world.GetWorldComp<CreationComp>().AddCreationItem(ECreationType.Actor, 20, 18);
        }

        public void OnUpdate(float deltaTime)
        {
            _cameraCtrl.OnUpdate(deltaTime);
            _actorCtrl.OnUpdate(deltaTime);
            _terrainTextureCtrl.OnUpdate();
            _world?.OnUpdate(deltaTime);
        }

        public MapRecord CreateMapRecord()
        {
            //float minHeight = -1;
            float minHeight = 0;
            float maxHeight = 0;
            var mapRecord = new MapRecord(
                _config.DefaultGridMapRows,
                _config.DefaultGridMapCols,
                _config.DefaultGridSize);
            //mapRecord.GenerateGrids(-1,0);
            mapRecord.GenerateGrids(minHeight,maxHeight);
            mapRecord.RandomizeAllGridsType();
            return mapRecord;
        }
        
        public void Dispose()
        {
            _cameraCtrl = null;
            _mapRecord?.Dispose();
            _world.Dispose();
        }

        public GfxGridMap GetGridMap()
        {
            return _world.GfxGridMap;
        }

        private void SpawnActor()
        {
            var creationComp = _world.GetWorldComp<CreationComp>();
            
        }
    }
}