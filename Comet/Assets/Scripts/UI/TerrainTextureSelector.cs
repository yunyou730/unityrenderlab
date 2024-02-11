using comet.combat;
using UnityEngine;

namespace comet
{
    public class TerrainTextureSelector
    {
        const int kWidth = 200;
        const int kHeight = 40;

        private CombatManager _combat = null;
        
        public void Init()
        {
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
        }
        
        public void OnGUI()
        {
            GUI.Box(new Rect(300,10,250,350),"Terrain Texture");
            
            if (GUI.Button(new Rect(330,40,kWidth,kHeight),"None"))
            {
                _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.None;
            }
    
            if (GUI.Button(new Rect(330,100,kWidth,kHeight),"Ground"))
            {
                _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.Ground;
            }
            
            if (GUI.Button(new Rect(330,160,kWidth,kHeight),"Grass"))
            {
                _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.Grass;
            }
            
            if (GUI.Button(new Rect(330,220,kWidth,kHeight),"Dirt-Rough"))
            {
                _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.DirtRough;
            }
            
            if (GUI.Button(new Rect(330,280,kWidth,kHeight),"Blight"))
            {
                _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.Blight;
            }
            
            
        }
    }
}