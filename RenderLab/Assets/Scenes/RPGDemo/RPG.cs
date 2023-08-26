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
        
        private TilemapManager _tilemapManager = null;
        private VisionManager _visionManager = null;

        // Player
        private GameObject _playerGameObject = null;
        private MovementController _moveCtrl = null;
        
        // Camera
        private CameraController _cameraCtrl = null;

        
        void Start()
        {
            var playerSpawnCoord = new Vector2Int(4,5);
            
            // Create tilemap
            _tilemapManager = new TilemapManager();
            _tilemapManager.InitTilemapAndCreateTileObjects(ref playerSpawnCoord,_obstacleTilePrefab,_walkableTilePrefab);
            
            // Create Vision & Vision Obstacles
            _visionManager = new VisionManager(_visionObsBoxPrefab,_visionObsCylinderPrefab);
            _visionManager.CreateObstacleGameObjects(_tilemapManager.GetAABBMin(),_tilemapManager.GetAABBMax());

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
