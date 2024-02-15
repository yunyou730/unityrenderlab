using System;
using comet.combat;

namespace comet
{
    public class UIManager : IDisposable
    {
        private CombatManager _combat = null;
        
        private GridDebug _gridDebug = null;
        private TerrainTextureSelector _terrainTexture = null;
        private TerrainHeightEditor _terrainHeight = null;

        public void Init()
        {
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
            
            _gridDebug = new GridDebug();
            _terrainTexture = new TerrainTextureSelector();
            _terrainHeight = new TerrainHeightEditor();
            
            _gridDebug.Init();
            _terrainTexture.Init();
            _terrainHeight.Init();
        }

        public void OnGUI()
        {
            _gridDebug?.OnGUI();
            _terrainTexture?.OnGUI();
            _terrainHeight?.OnGUI();
        }

        public void Dispose()
        {
            _gridDebug = null;
            _terrainTexture = null;
            _terrainHeight = null;
        }
    }
}