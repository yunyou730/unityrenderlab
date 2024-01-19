using UnityEngine;
namespace comet.combat
{
    public class GfxWorld : World
    {
        private GridMap _gridMap = null;
        
        public GfxWorld(MapRecord mapRecord) : base(mapRecord)
        {
            
        }

        protected override void RegisterWorldComps()
        {
            base.RegisterWorldComps();
            
            var gfxWorldCreation = RegisterWorldComp<GfxCreationWorldComp>(new GfxCreationWorldComp());
            gfxWorldCreation.MapRecord = _mapRecord;
            gfxWorldCreation.bNeedCreateMap = true;
        }

        protected override void RegisterSystems()
        {
            base.RegisterSystems();
            RegisterSys(new GfxCreationSys(this));
            RegisterSys(new GfxSyncPosSys(this));
        }
        
        public GridMap GridMap
        {
            get => _gridMap;
            set => _gridMap = value;
        }
        
    }
}