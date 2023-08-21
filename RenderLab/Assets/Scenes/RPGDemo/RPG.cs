using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class RPG : MonoBehaviour
    {
        [SerializeField]
        GameObject _walkableTilePrefab = null;
        
        [SerializeField]
        GameObject _obstacleTilePrefab = null;

        [SerializeField]
        GameObject _playerPrefab = null;
        
        [SerializeField]
        GameObject _cameraGameObject = null;
        
        

        // Tilemap
        private GameObject _tilemapRoot = null;
        private Tilemap _tilemap = null;
        
        // Player
        private GameObject _playerGameObject = null;
        private MovementController _moveCtrl = null;
        
        // Camera
        private CameraController _cameraCtrl = null;

        
        void Start()
        {
            // Create tilemap
            _tilemap = TilemapMock.Mock();
            CreateTilemapGameObjects();
            
            // Create player
            _playerGameObject = CreatePlayerGameObject();
            Vector2Int createCoord = new Vector2Int(4,5);
            _moveCtrl = new MovementController(_playerGameObject.transform);
            _moveCtrl.SetTilemapAndLayer(_tilemap,_tilemap.GetLayer(0));
            _moveCtrl.SetPosTileCoord(createCoord);
            _moveCtrl.SetCamera(_cameraGameObject.transform);
            
            // Camera controller
            _cameraCtrl = new CameraController();
            _cameraCtrl.SetCamera(_cameraGameObject);
            _cameraCtrl.SetLookTarget(_playerGameObject.transform);
        }
        
        void Update()
        {
            if (_moveCtrl != null)
            {
                _moveCtrl.OnUpdate(Time.deltaTime);
            }

            if (_cameraCtrl != null)
            {
                //_cameraCtrl.targetOffset = _cameraLookTargetOffset; 
                _cameraCtrl.OnUpdate(Time.deltaTime);
            }
        }

        GameObject CreatePlayerGameObject()
        {
            GameObject result = GameObject.Instantiate(_playerPrefab);
            return result;
        }

        void CreateTilemapGameObjects()
        {
            _tilemapRoot = new GameObject("[root]");
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

            if (prefab != null)
            {
                Vector3 pos = Metrics.GetTileWorldPos(layer,x, y);
                var tileGameObject = GameObject.Instantiate(prefab);
                tileGameObject.name = "[tile][" + x + "," + y + "]";
                tileGameObject.transform.SetParent(parent);
                tileGameObject.transform.localScale = new Vector3(Metrics.kTileSize,Metrics.kTileSize,Metrics.kTileSize);
                tileGameObject.transform.position = pos;
            }
        }


    }
    
}
