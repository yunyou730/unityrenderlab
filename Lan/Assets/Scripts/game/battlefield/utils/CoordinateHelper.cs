using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace lan.game.battlefield
{
    public class CoordinateHelper
    {
        public static float kCellSize = 1;
        
        public static Vector3 GetPosAtCell(int atRow,int atCol)
        {
            float x = atCol * kCellSize;
            float z = atRow * kCellSize;
            float y = 0;    // how to get terrain height?
            
            return new Vector3(x,y,z);
        }
    }

}
