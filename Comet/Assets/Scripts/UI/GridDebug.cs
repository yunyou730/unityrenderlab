using System;
using System.Collections.Generic;
using comet.combat;
using UnityEngine;
using UnityEngine.UI;

namespace comet
{
    enum EGridMode
    {
        CanPass,
        FlowField,
        
        Max
    }

    public class GridDebug
    {
        private string[] _modes = new string[]
        {
            "mode1", "mode2"
        };

        private CombatManager _combat = null;
        
        public void Init()
        {
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
        }
        
        public void OnGUI()
        {
            ModeSelection();
        }

        private void ModeSelection()
        {
            const int kWidth = 200;
            const int kHeight = 40;
            const int kGapOnY = 20;

            const int kBaseButtonX = 20;
            const int kBaseButtonY = 40;
            
            GUI.Box(new Rect(10,10,250,350),"Grid Mode");
            if (GUI.Button(new Rect(40,40,kWidth,kHeight),"Passable"))
            {
                Debug.Log("Passable");
            }
    
            if (GUI.Button(new Rect(40,100,kWidth,kHeight),"Heat Map"))
            {
                Debug.Log("Heat Map");
            }
            
            if (GUI.Button(new Rect(40,160,kWidth,kHeight),"Flow Field"))
            {
                Debug.Log("Flow Field");
            }

            if (GUI.Button(new Rect(40,220,kWidth,kHeight),"Toggle Show Blocker"))
            {
                _combat.GetGfxGridMap().ToggleShowBlock();
            }
            
            if (GUI.Button(new Rect(40,280,kWidth,kHeight),"Toggle Show Grid"))
            {
                _combat.GetGfxGridMap().ToggleShowGrid();
            }
        }
    }
}