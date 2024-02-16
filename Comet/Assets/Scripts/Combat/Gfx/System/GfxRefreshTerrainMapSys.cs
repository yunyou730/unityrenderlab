using Unity.VisualScripting;

namespace comet.combat
{
    public class GfxRefreshTerrainMapSys:BaseSys,IUpdateSys
    {
        private GfxTerrainMeshComp _terrainMeshComp = null;
        private GfxGridMap _gfxGridMap = null;
        
        public GfxRefreshTerrainMapSys(World world) : base(world)
        {
            _terrainMeshComp = world.GetWorldComp<GfxTerrainMeshComp>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_gfxGridMap == null)
            {
                GfxWorld gfxWorld = (GfxWorld)_world;
                _gfxGridMap = gfxWorld.GfxGridMap;
            }
            
            if (_terrainMeshComp.IsDirty)
            {
                HandleTerrainMeshRebuild();
                _terrainMeshComp.IsDirty = false;
            }
        }
        
        private void HandleTerrainMeshRebuild()
        {
            _gfxGridMap.RebuildMesh();
        }
    }
}