using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace rpg
{
    public class TilemapMock
    {
        public static Tilemap Mock(ref Vector2Int notObsTile)
        {
            Tilemap tilemap = new Tilemap();

            const int kWidth = 30;
            const int kHeight = 30;
            Layer layer = new Layer(kWidth,kHeight,Vector3.zero);
            tilemap.AddLayer(layer);

            List<Vector2Int> obs = new List<Vector2Int>();
            float obstacleRate = 0.2f;
            for (int x = 0;x < kWidth;x++)
            {
                for (int y = 0;y < kHeight;y++)
                {
                    if (!(notObsTile.x == x && notObsTile.y == y))
                    {
                        int r = Random.Range(0, 100);
                        if (r <= (int)(obstacleRate * 100.0f))
                        {
                            obs.Add(new Vector2Int(x,y));
                        }    
                    }
                }
            }
            MarkTilesAsObstacle(layer,obs.ToArray());

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
        
        public Vector3 GetAABBMin()
        {
            if (_layers.Count > 0)
            {
                var layer = _layers[0];
                return layer.basePos;
            }
            return Vector3.zero;
        }

        public Vector3 GetAABBMax()
        {
            if (_layers.Count > 0)
            {
                var layer = _layers[0];
                return layer.basePos + new Vector3(layer.width * Metrics.kTileSize,0,layer.height * Metrics.kTileSize);
            }
            return Vector3.zero;   
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

