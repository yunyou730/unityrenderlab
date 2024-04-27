using System;
using System.Collections.Generic;
using comet.combat;
using UnityEngine;
using UnityEngine.UI;

namespace comet
{
    public class GridDebug
    {
        private CombatManager _combat = null;
        
        private bool _bShow = true;
        private Rect _expandRect = new Rect(20, 10, 250, 400);
        private Rect _hideRect = new Rect(20, 10, 250, 50);
        private Rect _windowRect;
        
        public void Init()
        {
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
            _windowRect = _expandRect;
        }
        
        public void OnGUI()
        {
            _windowRect = GUI.Window(0, _windowRect, ModeSelection, "Grid Mode");
        }

        private void ModeSelection(int windowID)
        {
            const int kWidth = 200;
            const int kHeight = 40;
            // const int kGapOnY = 20;
            //
            // const int kBaseButtonX = 20;
            // const int kBaseButtonY = 40;
            
            // GUI.Box(new Rect(10,10,250,410),"Grid Mode");
            
            _bShow = GUI.Toggle(new Rect(30,0,100,30),_bShow,"");
            if (_bShow)
            {
                _windowRect = _expandRect;
                
                if (GUI.Button(new Rect(30,40,kWidth,kHeight),"Passable"))
                {
                    Debug.Log("Passable");
                }
    
                if (GUI.Button(new Rect(30,100,kWidth,kHeight),"Heat Map"))
                {
                    Debug.Log("Heat Map");
                }
            
                if (GUI.Button(new Rect(30,160,kWidth,kHeight),"Flow Field"))
                {
                    Debug.Log("Flow Field");
                }

                if (GUI.Button(new Rect(30,220,kWidth,kHeight),"Toggle Show Blocker"))
                {
                    _combat.GetGfxGridMap().ToggleShowBlock();
                }
            
                if (GUI.Button(new Rect(30,280,kWidth,kHeight),"Toggle Show Grid"))
                {
                    _combat.GetGfxGridMap().ToggleShowGrid();
                }

                if (GUI.Button(new Rect(30, 340, kWidth, kHeight), "Toggle MiniMap"))
                {
                    if (_combat.World.MiniMap.IsShowing())
                    {
                        _combat.World.MiniMap.Hide();
                    }
                    else
                    {
                        _combat.World.MiniMap.Show();
                    }
                }
            }
            else
            {
                _windowRect = _hideRect;
            }
        }
    }
}