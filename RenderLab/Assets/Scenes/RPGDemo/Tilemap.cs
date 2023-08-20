using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace rpg
{
    public class TilemapMock
    {
        public static Tilemap Mock()
        {
            Tilemap tilemap = new Tilemap();
            Layer layer = new Layer(10,10,Vector3.zero);
            tilemap.AddLayer(layer);

            Vector2Int[] obs = new[]
            {
                new Vector2Int(5,2), 
                new Vector2Int(5,3), 
                new Vector2Int(5,5),
                new Vector2Int(5,6),
                new Vector2Int(5,7), 
                
                
                new Vector2Int(0,0), 
                new Vector2Int(1,0), 
                new Vector2Int(2,0), 
                new Vector2Int(3,0), 
                new Vector2Int(4,0), 
                new Vector2Int(5,0), 
                new Vector2Int(6,0), 
                new Vector2Int(7,0), 
                new Vector2Int(8,0), 
                new Vector2Int(9,0), 
                
                
                new Vector2Int(0,9), 
                new Vector2Int(1,9), 
                new Vector2Int(2,9), 
                new Vector2Int(3,9), 
                new Vector2Int(4,9), 
                new Vector2Int(5,9), 
                new Vector2Int(6,9), 
                new Vector2Int(7,9), 
                new Vector2Int(8,9), 
                new Vector2Int(9,9), 
                
                
            };

            MarkTilesAsObstacle(layer,obs);

            return tilemap;
        }

        protected static void MarkTilesAsObstacle(Layer layer,Vector2Int[] coords)
        {
            foreach (Vector2Int coord in coords)
            {
                Tile tile = layer.GetTileAt(coord.x,coord.y);
                tile.tileType = ETileType.Obstacle;
            }
        }
    }

    public class Tilemap
    {
        private List<Layer> _layers = new List<Layer>();
        
        public Tilemap()
        {
            
        }
        
        public void AddLayer(Layer layer)
        {
            _layers.Add(layer);
        }

        public Layer GetLayer(int idx)
        {
            return _layers[idx];
        }

        public int LayerCount()
        {
            return _layers.Count;
        }
    }

    public class Layer
    {
        public int width;
        public int height;
        public Tile[] tiles = null;
        public Vector3 basePos = Vector3.zero; 

        public Layer(int width,int height,Vector3 basePos)
        {
            this.width = width;
            this.height = height;
            this.basePos = Vector3.zero;
            
            
            int tileCnt = width * height;
            tiles = new Tile[tileCnt];

            for (int i = 0;i < tileCnt;i++)
            {
                tiles[i] = new Tile();
            }
        }

        public Tile GetTileAt(int x,int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return null;
            int idx = GetTileIndex(x, y);
            return tiles[idx];
        }

        public int GetTileIndex(int x,int y)
        {
            return y * width + x;
        }
    }
    
    public class Tile
    {
        public ETileType tileType;
        public int posY;

        public Tile()
        {
            tileType = ETileType.Walkable;
            posY = 0;
        }
    }

    public enum ETileType
    {
        Walkable,
        Obstacle
    }
}

