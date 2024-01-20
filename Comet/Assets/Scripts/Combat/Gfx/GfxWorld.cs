using UnityEngine;
namespace comet.combat
{
    public class GfxWorld : World
    {
        private GfxGridMap _gfxGridMap = null;
        
        private Texture2D _walkableBuffer = null;
        
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
        
        public GfxGridMap GfxGridMap
        {
            get => _gfxGridMap;
            set => _gfxGridMap = value;
        }
        
    }
}