using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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
        
        public void GenerateGrids(float minHeight,float maxHeight)
        {
            var rand = new System.Random();
            for(int i = 0;i < _rows * _cols;i++)
            {
                var gridRecord = new GridRecord();
                
                // height
                var height = rand.NextDouble() * (maxHeight - minHeight) + minHeight;
                gridRecord.SetHeight((float)height);
                
                // grid type
                //int gridType = rand.Next((int)GridRecord.EGridType.Max);
                int gridType = (int)GridRecord.EGridType.Plane;
                gridRecord.SetGridType((GridRecord.EGridType)gridType);
                
                // hold GridRecord
                _grids[i] = gridRecord;
            }
        }

        public GridRecord GetGridAt(int row,int col)
        {
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
            {
                return null;
            }
            return _grids[row * _cols + col];
        }

        public void RandomizeAllGridsType()
        {
            var rand = new System.Random();
            for (int i = 0; i < _grids.Length; i++)
            {
                int gridType = rand.Next((int)GridRecord.EGridType.Max);
                _grids[i].SetGridType((GridRecord.EGridType)gridType);
            }
        }

        public void Dispose()
        {
            _grids = null;
        }
    }
}

