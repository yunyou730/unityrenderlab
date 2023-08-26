using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class Metrics
    {
        public static float kTileSize = 1.5f;
        
        public static Vector3 GetTileWorldPos(Layer layer,int x,int y)
        {
            float offsetX = x * kTileSize;
            float offsetZ = y * kTileSize;

            Tile tile = layer.GetTileAt(x, y);
            float worldY = tile.posY;
            float worldX = layer.basePos.x + offsetX;
            float worldZ = layer.basePos.z + offsetZ;
            return new Vector3(worldX,worldY,worldZ);
        }
        
        public static Vector2Int WorldPosToTileCoord(Layer layer,Vector3 worldPos)
        {
            float x = worldPos.x;
            float z = worldPos.z;

            float offsetX = x - (layer.basePos.x - Metrics.kTileSize * 0.5f);
            float offsetY = z - (layer.basePos.z - Metrics.kTileSize * 0.5f);
            
            Vector2Int tileCoord = Vector2Int.zero;
            
            tileCoord.x = (int)Mathf.Floor(offsetX / Metrics.kTileSize);
            tileCoord.y = (int)Mathf.Floor(offsetY / Metrics.kTileSize);
            
            return tileCoord;
        }

    }
   
}
