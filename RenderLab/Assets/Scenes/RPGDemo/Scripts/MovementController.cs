using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class MovementController
    {
        private Transform _avatar = null;
        private Transform _cameraTransform = null;

        private static float kTurnSpeed = 180.0f;    // degree
        private static float kMoveSpeed = 7.0f;

        private float _radius = 0.5f;
        private Tilemap _tilemap = null;
        private Layer _layer = null;
        
        
        public enum ETryMoveResultType
        {
            OK,
            Block,
        }

        public enum ECtrlDir
        {
            Up,
            Down,
            Left,
            Right,
        }

        public MovementController(Transform avatar)
        {
            _avatar = avatar;
        }

        public void SetTilemapAndLayer(Tilemap tilemap,Layer layer)
        {
            _tilemap = tilemap;
            _layer = layer;
        }

        public void SetPosTileCoord(Vector2Int tileCoord)
        {
            Debug.Assert(_layer != null);
            Vector3 worldPos = Metrics.GetTileWorldPos(_layer,tileCoord.x, tileCoord.y);
            _avatar.transform.position = worldPos;
        }

        public void SetCamera(Transform cameraTransform)
        {
            _cameraTransform = cameraTransform;
        }

        


        public Vector3 GetMovementDirByCtrlDir(ECtrlDir ctrlDir)
        {
            Vector3 result = Vector3.zero;
            switch (ctrlDir)
            {
                case ECtrlDir.Up:
                    result = _avatar.transform.position - _cameraTransform.position;
                    break;
                case ECtrlDir.Down:
                    result = -(_avatar.transform.position - _cameraTransform.position);
                    break;
                case ECtrlDir.Left:
                {
                    Vector3 playerToCamera = _cameraTransform.position - _avatar.position;
                    Vector3 forward = _avatar.transform.position - _cameraTransform.position;
                    forward.y = 0;
                    result = Vector3.Cross(forward, playerToCamera);
                }
                    break;
                case ECtrlDir.Right:
                {
                    Vector3 playerToCamera = _cameraTransform.position - _avatar.position;                    
                    Vector3 forward = _avatar.transform.position - _cameraTransform.position;
                    forward.y = 0;
                    result = -Vector3.Cross(forward, playerToCamera);
                }
                    break;
            }
            result.y = 0;
            return Vector3.Normalize(result);
        }

        public void TurnLeft(float deltaTime)
        {
            Quaternion rot = Quaternion.Euler(0,-kTurnSpeed * deltaTime,0) * _avatar.transform.rotation;
            _avatar.transform.rotation = rot;
        }

        public void TurnRight(float deltaTime)
        {
            Quaternion rot = Quaternion.Euler(0,kTurnSpeed * deltaTime,0) * _avatar.transform.rotation;
            _avatar.transform.rotation = rot;
        }
        
        public void MoveForward(float deltaTime)
        {
            Vector3 forward = _avatar.transform.forward;
            forward.y = 0;
            forward = Vector3.Normalize(forward);
            MoveByDirInOneFrame(forward, deltaTime);
        }
        
        
        public void MoveBackward(float deltaTime)
        {
            Vector3 backward = -_avatar.transform.forward;
            backward.y = 0;
            backward = Vector3.Normalize(backward);
            
            MoveByDirInOneFrame(backward, deltaTime,false);
        }
        
        
        public void MoveLeft(float deltaTime)
        {
            Vector3 playerToCamera = _cameraTransform.position - _avatar.position;
            
            Vector3 forward = _avatar.transform.position - _cameraTransform.position;
            forward.y = 0;
            
            Vector3 dir = Vector3.Cross(forward, playerToCamera);
            MoveByDirInOneFrame(dir,deltaTime);
        }
        
        public void MoveRight(float deltaTime)
        {
            Vector3 forward = _avatar.transform.position - _cameraTransform.position;
            forward.y = 0;
            
            Vector3 playerToCamera = _cameraTransform.position - _avatar.position;
            Vector3 dir = -Vector3.Cross(forward, playerToCamera);
            MoveByDirInOneFrame(dir,deltaTime);
        }

        public void MoveByDirInOneFrame(Vector3 dir,float deltaTime,bool changeDirWhenOK = true)
        {
            dir = Vector3.Normalize(dir);
                
            Vector3 offset = dir * kMoveSpeed * deltaTime;
            
            ETryMoveResultType result = TryMoveMultipleCheck(_avatar.transform.position,dir,_radius,offset);
            
            
            if (result == ETryMoveResultType.OK)
            {
                _avatar.transform.position += offset;
                if (changeDirWhenOK)
                {
                    _avatar.transform.forward = dir;                    
                }
            }
            else if (result == ETryMoveResultType.Block)
            {
                Vector3? correctDir = null; 
                
                // Try other directions ,increase/decrease 3.0 degrees each time
                for (float deg = 3.0f;deg < 80.0f;deg += 3.0f)
                {
                    // add degrees
                    Vector3 nextDir = Quaternion.Euler(0, deg, 0) * dir;
                    Vector3 nextOffset = nextDir * kMoveSpeed * deltaTime;
                    Vector3 nextDest = _avatar.transform.position + nextDir * _radius + nextOffset;

                    var tryAgainResult = TryMoveMultipleCheck(_avatar.transform.position, nextDir, _radius, nextOffset);
                    if (tryAgainResult == ETryMoveResultType.OK)
                    {
                        _avatar.transform.position += nextOffset;
                        correctDir = nextDir;
                        break;
                    }
                    
                    // dec degress
                    nextDir = Quaternion.Euler(0, -deg, 0) * dir;
                    nextOffset = nextDir * kMoveSpeed * deltaTime;
                    nextDest = _avatar.transform.position + nextDir * _radius + nextOffset;
                    tryAgainResult = TryMoveMultipleCheck(_avatar.transform.position,nextDir,_radius,nextOffset);
                    if (tryAgainResult == ETryMoveResultType.OK)
                    {
                        _avatar.transform.position += nextOffset;
                        correctDir = nextDir;
                        break;
                    }
                }

                if (correctDir != null)
                {
                    _avatar.transform.forward = correctDir.Value;
                }
            }
        }

        ETryMoveResultType TryMove(Vector3 destPos)
        {
            ETryMoveResultType result = ETryMoveResultType.Block;
            Vector2Int tileCoord = Metrics.WorldPosToTileCoord(_layer, destPos);
            
            Tile destAtTile = _layer.GetTileAt(tileCoord.x, tileCoord.y);
            if (destAtTile != null && destAtTile.tileType == ETileType.Walkable)
            {
                result = ETryMoveResultType.OK;
            }
            return result;
        }
        
        ETryMoveResultType TryMoveMultipleCheck(Vector3 from,Vector3 dir,float radius,Vector3 offset)
        {
            Vector3 sideDir = Vector3.Normalize(Vector3.Cross(Vector3.up, dir));

            Vector3 leftOrigin = from + sideDir * radius;
            Vector3 rightOrigin = from - sideDir * radius;
            
            //Vector3 centerDest = from + dir * radius + offset;
            //Vector3 leftDest = leftOrigin + dir * radius + offset;
            Vector3 centerDest = from + dir * radius + offset;
            Vector3 leftDest = leftOrigin + dir + offset;            
            Vector3 rightDest = rightOrigin + dir + offset;

            var result = ETryMoveResultType.Block;
            if (TryMove(centerDest) == ETryMoveResultType.OK 
                && TryMove(leftDest) == ETryMoveResultType.OK 
                && TryMove(rightDest) == ETryMoveResultType.OK)
            {
                result = ETryMoveResultType.OK;
            }

            return result;
        }
    }
    
}

