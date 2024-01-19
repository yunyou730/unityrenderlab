using System;
using UnityEngine;

namespace comet.combat
{
    public class Metrics
    {
        public static void GetGridCenterPos(MapRecord mapRecord, 
                int gridX, int gridY,
                out float x,out float y,out float z)
        {
            float gridSize = mapRecord.GridSize;
            x = gridSize * gridX + gridSize * 0.5f;
            z = gridSize * gridY + gridSize * 0.5f;
            y = 0;
        }
        
        public static void PosToGrid(MapRecord mapRecord,float x,float y,out int gridX,out int gridY)
        {
            gridX = (int)(x / mapRecord.GridSize);
            gridY = (int)(y / mapRecord.GridSize);
        }

        public static bool IsNear(Vector3 p1,Vector3 p2)
        {
            return (p1 - p2).magnitude <= Double.Epsilon;
        }
    }
}