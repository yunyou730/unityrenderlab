using comet.combat;
using UnityEngine;

namespace comet
{
    public class TerrainHeightEditor
    {
        const int kWidth = 200;
        const int kHeight = 40;
        
        private bool _bShow = true;
        private Rect _expandRect = new Rect(600, 10, 250, 230);
        private Rect _hideRect = new Rect(600, 10, 250, 50);
        private Rect _windowRect;
        
        private CombatManager _combat = null;
        
        public void Init()
        {
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
            _windowRect = _expandRect;
        }
        
        public void OnGUI()
        {
            _windowRect = GUI.Window(2, _windowRect, TextureSelectorWindow, "Terrain Height");
        }
        
        private void TextureSelectorWindow(int windowID)
        {
            _bShow = GUI.Toggle(new Rect(30,0,100,30),_bShow,"");
            if (_bShow)
            {
                _windowRect = _expandRect;
                
                if (GUI.Button(new Rect(30,40,kWidth,kHeight),"None"))
                {

                }
    
                if (GUI.Button(new Rect(30,100,kWidth,kHeight),"Height+"))
                {

                }
                
                if (GUI.Button(new Rect(30,160,kWidth,kHeight),"Height-"))
                {

                }                
            }
            else
            {
                _windowRect = _hideRect;
            }
        }
        
    }
}