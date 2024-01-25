using System;
using comet.combat;

namespace comet
{
    public class UIManager : IDisposable
    {
        private CombatManager _combat = null;
        
        private GridDebug _gridDebug = null;
        private TerrainTextureSelector _terrainTexture = null;

        public void Init()
        {
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
            
            _gridDebug = new GridDebug();
            _terrainTexture = new TerrainTextureSelector();
            
            _gridDebug.Init();
            _terrainTexture.Init();
        }

        public void OnGUI()
        {
            _gridDebug?.OnGUI();
            _terrainTexture?.OnGUI();
        }

        public void Dispose()
        {
            _gridDebug = null;
        }
    }
}