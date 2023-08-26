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

        // Player
        private GameObject _playerGameObject = null;
        private MovementController _moveCtrl = null;
        
        // Camera
        private CameraController _cameraCtrl = null;

        
        void Start()
        {
            var playerSpawnCoord = new Vector2Int(4,5);
            
            // Managers
            _tilemapManager = new TilemapManager();
            _obstacleManager = new ObstacleManager(_visionObsBoxPrefab,_visionObsCylinderPrefab);
            _visionManager = new VisionManager(_visionRangePrefab);
            
            // Create tilemap
            _tilemapManager.InitTilemapAndCreateTileObjects(ref playerSpawnCoord,_obstacleTilePrefab,_walkableTilePrefab);
            
            // Create Obstacles
            _obstacleManager.CreateObstacleGameObjects(_tilemapManager.GetAABBMin(),_tilemapManager.GetAABBMax());

            // Create player
            _playerGameObject = CreatePlayerGameObject();
            _moveCtrl = new MovementController(_playerGameObject.transform);
            _moveCtrl.SetTilemapAndLayer(_tilemapManager.GetTileMap(),_tilemapManager.GetTileMap().GetLayer(0));
            _moveCtrl.SetPosTileCoord(playerSpawnCoord);
            _moveCtrl.SetCamera(_cameraGameObject.transform);
            
            // Camera controller
            _cameraCtrl = new CameraController();
            _cameraCtrl.SetCamera(_cameraGameObject);
            _cameraCtrl.SetLookTarget(_playerGameObject.transform);
            
            // Create visions
            _visionManager.RegisterVision(_playerGameObject,10.0f);
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

        private void CreateVisionObstacles()
        {
            
        }


    }
    
}
