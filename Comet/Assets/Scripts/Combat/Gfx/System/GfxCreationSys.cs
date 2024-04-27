using System;
using System.Collections.Generic;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class GfxCreationSys : BaseSys,IUpdateSys
    {
        private GfxWorld _gfxWorld = null;
        private GfxGridMap _gfxGridMap = null;
        public GfxGridMap GfxGridMap { get { return _gfxGridMap; } }
        
        private ResManager _resManager = null;
        private Config _config = null;

        private GfxCreationWorldComp _gfxCreation = null;

        private MiniMap _miniMap = null;
        
        public GfxCreationSys(World world) : base(world)
        {
            _gfxWorld = (GfxWorld)world;
            _miniMap = _gfxWorld.MiniMap;
            
            _resManager = Comet.Instance.ServiceLocator.Get<ResManager>();
            _config = Comet.Instance.ServiceLocator.Get<Config>();

            _gfxCreation = world.GetWorldComp<GfxCreationWorldComp>();
        }

        public void OnUpdate(float deltaTime)
        {
            HandleCreateMap();
            HandleCreateActor();
            _gfxGridMap.RefreshTexData();
        }

        protected void HandleCreateMap()
        {
            if (_gfxCreation.bNeedCreateMap)
            {
                _gfxCreation.bNeedCreateMap = false;
                CreateGridMapGfx(_gfxCreation.MapRecord);
                _gfxWorld.GfxGridMap = _gfxGridMap;
                _miniMap.BindGridMap(_gfxGridMap);
            }
        }
        
        protected void CreateGridMapGfx(MapRecord mapRecord)
        {
            var resManager = Comet.Instance.ServiceLocator.Get<ResManager>();
            var prefab = resManager.Load<GameObject>("Prefabs/GridMap");
            _gfxGridMap = GameObject.Instantiate(prefab).GetComponent<GfxGridMap>();
            _gfxGridMap.RefreshWithMapRecord(mapRecord);
        }
        
        protected void HandleCreateActor()
        {
            Type[] comps = {typeof(ActorComp),typeof(PositionComp)};
            var entityList = _world.GetEntities(comps);
            
            foreach (var entity in entityList)
            {
                var actorComp = entity.GetComp<ActorComp>();
                if (!actorComp.bHasCreateGfx)
                {
                    CreateActorGfx(entity,actorComp,entity.GetComp<PositionComp>());
                    actorComp.bHasCreateGfx = true;
                }
            }
        }
        
        protected void CreateActorGfx(Entity entity,ActorComp actorComp,PositionComp gridPosComp)
        {
            var prefab = _resManager.Load<GameObject>("Prefabs/Actor");
            var gfxActorComp = entity.AttachComp<GfxActorComp>(new GfxActorComp());
            gfxActorComp.Init(entity.UUID,prefab,_gfxGridMap,gridPosComp.GridY,gridPosComp.GridX);
        }
    }
}