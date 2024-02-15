using comet.combat;
using UnityEngine;

namespace comet
{
    public class TerrainTextureSelector
    {
        const int kWidth = 200;
        const int kHeight = 40;

        private CombatManager _combat = null;
        
        
        private bool _bShow = true;
        private Rect _expandRect = new Rect(300, 10, 250, 350);
        private Rect _hideRect = new Rect(300, 10, 250, 50);
        private Rect _windowRect;
        
        public void Init()
        {
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
            _windowRect = _expandRect;
        }
        
        public void OnGUI()
        {
            _windowRect = GUI.Window(1, _windowRect, TextureSelectorWindow, "Terrain Texture");
        }
        
        private void TextureSelectorWindow(int windowID)
        {
            _bShow = GUI.Toggle(new Rect(30,0,100,30),_bShow,"");
            if (_bShow)
            {
                _windowRect = _expandRect;
                
                if (GUI.Button(new Rect(30,40,kWidth,kHeight),"None"))
                {
                    _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.None;
                }
    
                if (GUI.Button(new Rect(30,100,kWidth,kHeight),"Ground"))
                {
                    _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.Ground;
                }
            
                if (GUI.Button(new Rect(30,160,kWidth,kHeight),"Grass"))
                {
                    _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.Grass;
                }
            
                if (GUI.Button(new Rect(30,220,kWidth,kHeight),"Dirt-Rough"))
                {
                    _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.DirtRough;
                }
            
                if (GUI.Button(new Rect(30,280,kWidth,kHeight),"Blight"))
                {
                    _combat.TerrainTextureCtrl.SelectedTerrainTexture = ETerrainTextureType.Blight;
                }
            }
            else
            {
                _windowRect = _hideRect;
            }
        }
    }
}