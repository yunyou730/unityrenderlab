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
            CreateActorAtCell("Assets/AssetLib/Prefabs/actors/genjin_liulangzhe/liulangzhe.prefab",0, 0);
            CreateActorAtCell("Assets/AssetLib/Prefabs/actors/genjin_falushan/falushan.prefab",-3, 1);
            
        }
        
        protected void CreateChessboardEntity()
        {
            GameObject go = Entry.Res().GetPrefab("Assets/AssetLib/Prefabs/chessboard/Chessboard.prefab");
            go = GameObject.Instantiate(go);

            Chessboard chessboard = go.GetComponent<Chessboard>();
            chessboard.SetRowColCnt(_settings._rowCnt,_settings._colCnt);
            chessboard.Refresh();
        }
        
        protected void CreateActorAtCell(string prefabPath,int row,int col)
        {
            GameObject go = Entry.Res().GetPrefab(prefabPath);
            go = GameObject.Instantiate(go);
            
            Vector3 pos = CoordinateHelper.GetPosAtCell(row, col);
            go.transform.position = pos;
        }

    }
}
