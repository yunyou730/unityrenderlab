using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace comet
{
    public class MapRecord : IDisposable
    {
        public int Rows => _rows;
        public int Cols => _cols;

        private int _rows;
        private int _cols;

        private GridRecord[] _grids = null;
        
        public MapRecord(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            _grids = new GridRecord[rows * cols];
        }

        public GridRecord GetGridAt(int row,int col)
        {
            return _grids[row * _cols + col];
        }
        
        public void Dispose()
        {
            _grids = null;
        }
    }
}

