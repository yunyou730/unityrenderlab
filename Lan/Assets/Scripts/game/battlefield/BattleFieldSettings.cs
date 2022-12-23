using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lan.game.battlefield
{
    public class BattleFieldSettings
    {
        public int _rowCnt;
        public int _colCnt;

        public void InitWithFakeData()
        {
            _rowCnt = 25;
            _colCnt = 30;
        }

        public void InitWithConfig()
        {
            
        }
    }
}

