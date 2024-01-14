using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class GfxCreationSys : BaseSys
    {
        private GridMap _gridMap = null;
        
        private ResManager _resManager = null;
        private Config _config = null;

        private GfxCreationWorldComp _creation = null;
        
        public GfxCreationSys(World world) : base(world)
        {
            _resManager = Comet.Instance.ServiceLocator.Get<ResManager>();
            _config = Comet.Instance.ServiceLocator.Get<Config>();

            _creation = world.GetWorldComp<GfxCreationWorldComp>();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (_creation.bNeedCreateMap)
            {
                _creation.bNeedCreateMap = false;
                CreateGridMapGfx(_creation.MapRecord);
            }
        }
        
        protected void CreateGridMapGfx(MapRecord mapRecord)
        {
            var resManager = Comet.Instance.ServiceLocator.Get<ResManager>();
            var prefab = resManager.Load<GameObject>("Prefabs/GridMap");
            _gridMap = GameObject.Instantiate(prefab).GetComponent<GridMap>();
            _gridMap.RefreshWithMapRecord(mapRecord,_config.GridSize);
        }
    }
}