using UnityEngine;
namespace comet.combat
{
    public class GfxWorld : World
    {
        private MapRecord _mapRecord = null;
        public GfxWorld(MapRecord mapRecord)
        {
            _mapRecord = mapRecord;
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
            _allSystems.Add(new GfxCreationSys(this));
            
            Debug.Log("GfxWorld::RegisterSystems()");
        }
    }
}