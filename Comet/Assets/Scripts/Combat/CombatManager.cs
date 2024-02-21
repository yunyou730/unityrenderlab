using System;
using comet.res;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace comet.combat
{
    public class CombatManager : IDisposable
    {
        private MapRecord _mapRecord = null;
        private GfxWorld _world = null;
        public GfxWorld World { get { return _world; } }
        
        // terrain controller
        private TerrainCrossPointSelector _crossPointSelector = null;
        private CameraCtrl _cameraCtrl = null;
        private ActorCtrl _actorCtrl = null;
        private TerrainTextureCtrl _terrainTextureCtrl = null;
        private TerrainHeightCtrl _terrainHeightCtrl = null;
        
        public TerrainTextureCtrl TerrainTextureCtrl => _terrainTextureCtrl;
        public TerrainHeightCtrl TerrainHeightCtrl => _terrainHeightCtrl;
        
        // Fog of war
        // private GfxFogOfWar _fogOfWar = null;
        private TerrainDepthTextureProvider _terrainDepthTextureProvider = null;
        
        
        private Config _config = null;
        
        public void Init(Camera mainCamera,Image imgMiniMap)
        {
            _config = Comet.Instance.ServiceLocator.Get<Config>();
            
            // Create Map
            _mapRecord = CreateMapRecord();
            _world = new GfxWorld(_mapRecord,imgMiniMap);
            _world.Init();
            
            _terrainDepthTextureProvider = new TerrainDepthTextureProvider();
            _terrainDepthTextureProvider.Init(mainCamera);
            
            // Ctrl
            _cameraCtrl = new CameraCtrl();
            _actorCtrl = new ActorCtrl(this);
            _terrainTextureCtrl = new TerrainTextureCtrl(this);
            _terrainHeightCtrl = new TerrainHeightCtrl(this);
            _crossPointSelector = new TerrainCrossPointSelector(_mapRecord,_terrainTextureCtrl,_terrainHeightCtrl,_terrainDepthTextureProvider);
            
            _crossPointSelector.Init(mainCamera);
            _cameraCtrl.Init(mainCamera);
            _actorCtrl.Init(mainCamera);
            _terrainTextureCtrl.Init(mainCamera,_mapRecord,_crossPointSelector);
            _terrainHeightCtrl.Init(mainCamera,_mapRecord);
            
            // Fog of war
            // _fogOfWar = new GfxFogOfWar();
            // _fogOfWar.Init(mainCamera);

            
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
            _crossPointSelector.OnUpdate();
            
            // _fogOfWar?.OnUpdate();
            _terrainDepthTextureProvider?.OnUpdate();
            
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
            mapRecord.Generate(minHeight,maxHeight);
            // mapRecord.RandomizeAllGridsType();
            
            return mapRecord;
        }
        
        public void Dispose()
        {
            _crossPointSelector.Dispose();
            _crossPointSelector = null;
            
            _cameraCtrl = null;
            
            // _fogOfWar.Dispose();
            // _fogOfWar = null;
            
            _terrainDepthTextureProvider.Dispose();
            _terrainDepthTextureProvider = null;
            
            _mapRecord?.Dispose();
            _world.Dispose();
            
        }

        public GfxGridMap GetGfxGridMap()
        {
            return _world.GfxGridMap;
        }

        private void SpawnActor()
        {
            var creationComp = _world.GetWorldComp<CreationComp>();
            
        }
    }
}