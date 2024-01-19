using System;
using System.Collections.Generic;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class GfxCreationSys : BaseSys,IUpdateSys
    {
        private GfxWorld _gfxWorld = null;
        private GridMap _gridMap = null;
        public GridMap GridMap
        {
            get { return _gridMap; }
        }
        
        private ResManager _resManager = null;
        private Config _config = null;

        private GfxCreationWorldComp _gfxCreation = null;
        
        public GfxCreationSys(World world) : base(world)
        {
            _gfxWorld = (GfxWorld)world;
            
            _resManager = Comet.Instance.ServiceLocator.Get<ResManager>();
            _config = Comet.Instance.ServiceLocator.Get<Config>();

            _gfxCreation = world.GetWorldComp<GfxCreationWorldComp>();
        }

        public void OnUpdate(float deltaTime)
        {
            HandleCreateMap();
            HandleCreateActor();
        }

        protected void HandleCreateMap()
        {
            if (_gfxCreation.bNeedCreateMap)
            {
                _gfxCreation.bNeedCreateMap = false;
                CreateGridMapGfx(_gfxCreation.MapRecord);
                _gfxWorld.GridMap = _gridMap;
            }
        }
        
        protected void CreateGridMapGfx(MapRecord mapRecord)
        {
            var resManager = Comet.Instance.ServiceLocator.Get<ResManager>();
            var prefab = resManager.Load<GameObject>("Prefabs/GridMap");
            _gridMap = GameObject.Instantiate(prefab).GetComponent<GridMap>();
            _gridMap.RefreshWithMapRecord(mapRecord,_config.GridSize);
        }
        
        protected void HandleCreateActor()
        {
            Type[] comps = {typeof(ActorComp),typeof(GridPositionComp)};
            var entityList = _world.GetEntities(comps);
            
            foreach (var entity in entityList)
            {
                var actorComp = entity.GetComp<ActorComp>();
                if (!actorComp.bHasCreateGfx)
                {
                    CreateActorGfx(entity,actorComp,entity.GetComp<GridPositionComp>());
                    actorComp.bHasCreateGfx = true;
                }
            }
        }
        
        protected void CreateActorGfx(Entity entity,ActorComp actorComp,GridPositionComp gridPosComp)
        {
            var prefab = _resManager.Load<GameObject>("Prefabs/Actor");
            GameObject gameObject = GameObject.Instantiate(prefab);
            gameObject.transform.position = Metrics.GetGridCenterPos(_gridMap,gridPosComp.Y,gridPosComp.X);
            gameObject.AddComponent<GfxActor>().UUID = entity.UUID;
        }
    }
}