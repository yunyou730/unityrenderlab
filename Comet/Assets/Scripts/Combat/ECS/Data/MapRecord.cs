using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace comet.combat
{
    public class MapRecord : IDisposable
    {
        public int GridRows => _rows;
        public int GridCols => _cols;
        public float GridSize => _gridSize;
        
        public int PointsInRow { get { return _rows + 1; } }
        public int PointsInCol { get { return _cols + 1; } }

        private int _rows;
        private int _cols;
        private float _gridSize;

        private GridRecord[] _grids = null;
        private PointRecord[] _points = null;
        
        // private const int kBlockerRate = 0;
        
        
        public MapRecord(int rows, int cols,float gridSize)
        {
            _rows = rows;
            _cols = cols;
            _gridSize = gridSize;
            _grids = new GridRecord[rows * cols];
            _points = new PointRecord[(rows + 1) * (cols + 1)];
        }
        
        public void Generate(float minHeight,float maxHeight)
        {
            // Grids
            for(int gridY = 0;gridY < _rows;gridY++)
            {
                for (int gridX = 0;gridX < _cols;gridX++)
                {
                    GridRecord gridRecord = new GridRecord();
                    gridRecord.SetHeight(0);
                    gridRecord.SetGridType(EGridType.Ground);
                    _grids[gridY * _cols + gridX] = gridRecord;
                }
            }
            
            // Points
            for (int pointY = 0;pointY < _rows + 1;pointY++)
            {
                for (int pointX = 0;pointX < _cols + 1;pointX++)
                {
                    PointRecord pointRecord = new PointRecord();
                    pointRecord.CliffLevel = 0;
                    pointRecord.TerrainHeight = 0.0f;
                    pointRecord.TerrainTextureType = ETerrainTextureType.Ground;
                    _points[pointY * (_cols + 1) + pointX] = pointRecord;
                    
                    // // @miao @debug
                    // if (pointX >= 5 && pointX <= 10)// && pointY == 5)
                    // {
                    //     pointRecord.TerrainHeight = 0.4f;
                    // }
                }
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

        // row [0,_rows + 1) , col [0,_cols + 1) 
        public PointRecord GetPointAt(int row,int col)
        {
            if (row < 0 || row >= _rows + 1 || col < 0 || col >= _cols + 1)
            {
                return null;
            }
            return _points[row * (_cols + 1) + col];
        }

        // public void RandomizeAllGridsType()
        // {
        //     var rand = new System.Random();
        //     for (int i = 0; i < _grids.Length; i++)
        //     {
        //         int randValue = rand.Next(100);
        //         EGridType gridType =
        //             randValue < kBlockerRate ? EGridType.Wall : EGridType.Ground;
        //         _grids[i].SetGridType(gridType);
        //     }
        // }

        public void Dispose()
        {
            _grids = null;
            _points = null;
        }
    }
}

