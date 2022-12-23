using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace lan.game.battlefield
{
    public class BattleField
    {
        protected BattleFieldSettings _settings = null;
        
        

        public BattleField(BattleFieldSettings settings)
        {
            _settings = settings;
        }

        public void Init()
        {
            CreateChessboardEntity();
        }
        
        protected void CreateChessboardEntity()
        {
            GameObject go = Entry.Res().GetPrefab("Assets/AssetLib/Prefabs/chessboard/Chessboard.prefab");
            go = GameObject.Instantiate(go);

            Chessboard chessboard = go.GetComponent<Chessboard>();
            chessboard.SetRowColCnt(7,10);
            chessboard.Refresh();
        }
    }
}
