using System;
using UnityEngine;

namespace comet.combat
{
    public class Metrics
    {
        public static Vector3 GetGridOriginPos(MapRecord mapRecord,int gridX,int gridY)
        {
            float gridSize = mapRecord.GridSize;
            Vector3 result = new Vector3(
                gridSize * gridX,
                0,
                gridSize * gridY);
            return result;
        }

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

        public static bool IsNear(Vector2 p1,Vector2 p2)
        {
            return (p1 - p2).magnitude <= 0.01f; //Double.Epsilon * 10.0f;
        }
    }
}