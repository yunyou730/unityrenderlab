using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class TilemapManager
    {
        private Tilemap _tilemap = null;
        private GameObject _tilemapRoot = null;
        private GameObject _obstacleTilePrefab = null;
        private GameObject _walkableTilePrefab = null;
        
        public void InitTilemapAndCreateTileObjects(
            ref Vector2Int playerSpawnCoord,
            GameObject obstacleTilePrefab, 
            GameObject walkableTilePrefab)
        {
            _obstacleTilePrefab = obstacleTilePrefab;
            _walkableTilePrefab = walkableTilePrefab;
            _tilemap = TilemapMock.Mock(ref playerSpawnCoord);
            CreateTilemapGameObjects();   
        }

        public void Dispose()
        {
            
        }

        public Tilemap GetTileMap()
        {
            return _tilemap;
        }

        public Vector3 GetAABBMax()
        {
            return _tilemap.GetAABBMax();
        }

        public Vector3 GetAABBMin()
        {
            return _tilemap.GetAABBMin();
        }
        
        void CreateTilemapGameObjects()
        {
            _tilemapRoot = new GameObject("[tilemap root]");
            for (int layerIdx = 0;layerIdx < _tilemap.LayerCount();layerIdx++)
            {
                Layer layer = _tilemap.GetLayer(layerIdx);
                GameObject layerGameObject = new GameObject("[layer][" + layerIdx + "]");
                layerGameObject.transform.SetParent(_tilemapRoot.transform);

                CreateTileGameObjectsInLayer(layer,layerGameObject.transform);
            }
        }
        
        void CreateTileGameObjectsInLayer(Layer layer,Transform parent)
        {
            for (int x = 0;x < layer.width;x++)
            {
                for (int y = 0;y < layer.height;y++)
                {
                    CreateTileGameObject(layer, x, y,parent);
                }
            }
        }

        void CreateTileGameObject(Layer layer,int x,int y,Transform parent)
        {
            Tile tile = layer.GetTileAt(x, y);

            GameObject prefab = null;
            switch (tile.tileType)
            {
                case ETileType.Obstacle:
                    prefab = _obstacleTilePrefab;
                    break;
                case ETileType.Walkable:
                    prefab = _walkableTilePrefab;
                    break;
            }

            Debug.Assert(prefab != null);
            
            Vector3 pos = Metrics.GetTileWorldPos(layer,x, y);
            var tileGameObject = GameObject.Instantiate(prefab);
            tileGameObject.name = "[tile][" + x + "," + y + "]";
            tileGameObject.transform.SetParent(parent);
            tileGameObject.transform.localScale = new Vector3(Metrics.kTileSize,Metrics.kTileSize,Metrics.kTileSize);
            tileGameObject.transform.position = pos;
            
        }

        
    }
    
}
