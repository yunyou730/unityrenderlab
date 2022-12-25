using System;
using System.Collections;
using System.Collections.Generic;
using lan.game;
using lan.managers;
using Unity.VisualScripting;
using UnityEngine;

namespace lan
{
    public class Entry : MonoBehaviour
    {
        private static Entry s_instance = null;
        
        private Game   _game = null;
        private ResourceManager _resourceManager = null;
        private ABManager _abManager = null;

        private Transform _entry = null;
        private Transform _uiRoot = null;
        
        public static Entry Instance()
        {
            return s_instance;
        }

        public static Game GetGame()
        {
            return s_instance._game;
        }

        public static ResourceManager Res()
        {
            return s_instance._resourceManager;
        }
        
        public static ABManager AB()
        {
            return s_instance._abManager;
        }

        private void Awake()
        {
            Debug.Assert(s_instance == null,"More than one entry instance!");
            s_instance = this;
            
            _entry = transform.Find("[lan]Entry");
            _uiRoot = transform.Find("[lan]UIRoot");
        }

        private void Start()
        {
            ShowDebugMenu();
            
            _abManager = new ABManager(Application.streamingAssetsPath);
            _resourceManager = new ResourceManager();
            
            _game = new Game();
            _game.EnterBattleField();
        }
        
        public void Update()
        {
            
        }

        private void ShowDebugMenu()
        {
            GameObject go = Resources.Load<GameObject>("menu/Menu_DebugInfo");
            Instantiate(go, _uiRoot, true);
        }
    }
}


