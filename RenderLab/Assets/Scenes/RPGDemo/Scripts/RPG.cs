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

        [SerializeField] private GameObject _visionObsBoxPrefab = null;
        [SerializeField] private GameObject _visionObsCylinderPrefab = null;

        [SerializeField] private GameObject _visionRangePrefab = null;
        
        private TilemapManager _tilemapManager = null;
        private ObstacleManager _obstacleManager = null;
        private VisionManager _visionManager = null;
        private InputManager _inputManager = null;

        // Player
        private GameObject _playerGameObject = null;

        void Start()
        {
            var playerSpawnCoord = new Vector2Int(4,5);

            // Create tilemap
            _tilemapManager = new TilemapManager();
            _tilemapManager.InitTilemapAndCreateTileObjects(ref playerSpawnCoord,_obstacleTilePrefab,_walkableTilePrefab);
            
            // Create Obstacles
            _obstacleManager = new ObstacleManager(_visionObsBoxPrefab,_visionObsCylinderPrefab);
            _obstacleManager.CreateObstacleGameObjects(_tilemapManager.GetAABBMin(),_tilemapManager.GetAABBMax());

            // Create player
            _playerGameObject = CreatePlayerGameObject();
            
            // Input Manager
            _inputManager = new InputManager(_playerGameObject, _cameraGameObject,_tilemapManager.GetTileMap(),playerSpawnCoord);
            
            // Create visions
            _visionManager = new VisionManager(_visionRangePrefab);            
            _visionManager.RegisterVision(_playerGameObject,10.0f);
        }
        
        void Update()
        {
            if (_inputManager != null)
            {
                _inputManager.Update(Time.deltaTime);   
            }

            if (_visionManager != null)
            {
                _visionManager.Update();
            }
        }

        GameObject CreatePlayerGameObject()
        {
            GameObject result = GameObject.Instantiate(_playerPrefab);
            return result;
        }
    }
    
}
