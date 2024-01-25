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
            GUI.Box(new Rect(300,10,250,250),"Terrain Texture");
            
            if (GUI.Button(new Rect(330,40,kWidth,kHeight),"None"))
            {
                _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTexture.None;
            }
    
            if (GUI.Button(new Rect(330,100,kWidth,kHeight),"Ground"))
            {
                _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTexture.Ground;
            }
            
            if (GUI.Button(new Rect(330,160,kWidth,kHeight),"Grass"))
            {
                _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTexture.Grass;
            }
            
        }
    }
}