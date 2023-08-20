using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class Metrics
    {
        public static float kTileSize = 2;
        
        public static Vector3 GetTileWorldPos(Layer layer,int x,int y)
        {
            Vector3 worldPos = Vector3.zero;

            float offsetX = x * kTileSize;
            float offsetZ = y * kTileSize;

            Tile tile = layer.GetTileAt(x, y);
            return worldPos + new Vector3(layer.basePos.x + offsetX,tile.posY,layer.basePos.z + offsetZ);
        }


        public static Vector2Int WorldPosToTileCoord(float x,float z)
        {
            Vector2Int tileCoord = Vector2Int.zero;
            
            
            return tileCoord;
        }

    }
   
}
