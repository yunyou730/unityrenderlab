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
        Game   _game = null;

        private ResourceManager _resourceManager = null;
        


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

        private void Awake()
        {
            Debug.Assert(s_instance == null,"More than one entry instance!");
            s_instance = this;
        }

        private void Start()
        {
            _resourceManager = new ResourceManager();
            
            _game = new Game();
            _game.EnterBattleField();
        }

        public void Update()
        {
            
        }
    }
}


