using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class InputManager
    {
        private MovementController _moveCtrl = null;
        private CameraController _cameraCtrl = null;
        
        private GameObject _playerGameObject = null;
        private GameObject _cameraGameObject = null;

        private enum EPlayerMoveType
        {
            TankType,
            RPGType,
            
            Max
        }

        private EPlayerMoveType _moveType = EPlayerMoveType.TankType;

        public InputManager(GameObject playerGameObject,
                        GameObject cameraGameObject,
                        Tilemap tilemap,
                        Vector2Int playerSpawnCoord)
        {
            _playerGameObject = playerGameObject;
            _cameraGameObject = cameraGameObject;
            
            // Player movement controller
            _moveCtrl = new MovementController(_playerGameObject.transform);
            _moveCtrl.SetTilemapAndLayer(tilemap,tilemap.GetLayer(0));
            _moveCtrl.SetPosTileCoord(playerSpawnCoord);
            _moveCtrl.SetCamera(_cameraGameObject.transform);

            // Camera controller
            _cameraCtrl = new CameraController();
            _cameraCtrl.SetCamera(_cameraGameObject);
            _cameraCtrl.SetLookTarget(_playerGameObject.transform);
        }

        public void Update(float deltaTime)
        {
            UpdateForPlayerMovement(deltaTime);
            _cameraCtrl.OnUpdate(Time.deltaTime);
        }

        private void UpdateForPlayerMovement(float deltaTime)
        {
            switch (_moveType)
            {
                case EPlayerMoveType.TankType:
                    UpdateMovementTank(deltaTime);
                    break;
                case EPlayerMoveType.RPGType:
                    UpdateMovementRPG(deltaTime);
                    break;
            }
        }

        private void UpdateMovementTank(float deltaTime)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _moveCtrl.TurnLeft(deltaTime);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _moveCtrl.TurnRight(deltaTime);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                _moveCtrl.MoveForward(deltaTime);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                _moveCtrl.MoveBackward(deltaTime);
            }
        }   

        private void UpdateMovementRPG(float deltaTime)
        {
            Vector3 moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveDir += _moveCtrl.GetMovementDirByCtrlDir(MovementController.ECtrlDir.Up);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveDir += _moveCtrl.GetMovementDirByCtrlDir(MovementController.ECtrlDir.Down);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDir += _moveCtrl.GetMovementDirByCtrlDir(MovementController.ECtrlDir.Left);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDir += _moveCtrl.GetMovementDirByCtrlDir(MovementController.ECtrlDir.Right);
            }

            if (moveDir.magnitude > Mathf.Epsilon)
            {
                _moveCtrl.MoveByDirInOneFrame(moveDir,deltaTime);                
            }
        }

    }
}
