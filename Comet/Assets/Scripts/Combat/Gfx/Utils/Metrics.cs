using UnityEngine;

namespace comet.combat
{
    public class Metrics
    {
        public static Vector3 GetGridCenterPos(GridMap gridMap, int row, int col)
        {
            float gridSize = gridMap.GridSize;
            float x = gridSize * col + gridSize * 0.5f;
            float z = gridSize * row + gridSize * 0.5f;
            return new Vector3(x, 0, z);
        }
    }
}