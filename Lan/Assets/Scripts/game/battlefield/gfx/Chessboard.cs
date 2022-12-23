using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lan.game.battlefield
{
    public class Chessboard : MonoBehaviour
    {
        [SerializeField]
        protected int _colCnt = 1;
    
        [SerializeField]
        protected int _rowCnt = 1;

        private Material _material = null;

        private void Awake()
        {
            _material = GetComponent<MeshRenderer>().sharedMaterial;
        }
        
        public void SetRowColCnt(int rowCnt,int colCnt)
        {
            _colCnt = colCnt;
            _rowCnt = rowCnt;
        }
        
        public void Refresh()
        {
            transform.localScale = new Vector3(_colCnt,_rowCnt,1);
            _material.SetFloat("_ColCnt",_colCnt);
            _material.SetFloat("_RowCnt",_rowCnt);
        }
    }
}

