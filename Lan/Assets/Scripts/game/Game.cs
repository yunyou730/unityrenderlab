using System.Collections;
using System.Collections.Generic;
using lan.game.battlefield;
using UnityEngine;

namespace lan.game
{
    public class Game
    {
        public enum GameState
        {
            None,
            Battlefield,
            
        }
        
        protected GameState _state = GameState.None;

        protected BattleField _battleField = null;
        

        public void EnterBattleField()
        {
            _state = GameState.Battlefield;

            BattleFieldSettings settings = new BattleFieldSettings();
            settings.InitWithFakeData();
            
            _battleField = new BattleField(settings);
            _battleField.Init();
        }
    }
}

