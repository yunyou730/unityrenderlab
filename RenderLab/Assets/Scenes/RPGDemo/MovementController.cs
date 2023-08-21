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

        public void OnUpdate(float deltaTime)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                MoveForward(deltaTime);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                MoveBackward(deltaTime);
            }
            
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MoveLeft(deltaTime);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                MoveRight(deltaTime);
            }
        }

        private void TurnLeft(float deltaTime)
        {
            Quaternion rot = Quaternion.Euler(0,-kTurnSpeed * deltaTime,0) * _avatar.transform.rotation;
            _avatar.transform.rotation = rot;
        }

        private void TurnRight(float deltaTime)
        {
            Quaternion rot = Quaternion.Euler(0,kTurnSpeed * deltaTime,0) * _avatar.transform.rotation;
            _avatar.transform.rotation = rot;
        }
        
        private void MoveForward(float deltaTime)
        {
            Vector3 forward = _avatar.transform.position - _cameraTransform.position;
            forward.y = 0;
            MoveByDirInOneFrame(forward, deltaTime);
        }
        

        private void MoveBackward(float deltaTime)
        {
            Vector3 backward = -(_avatar.transform.position - _cameraTransform.position);
            backward.y = 0;
            MoveByDirInOneFrame(backward, deltaTime);
        }
        
        
        private void MoveLeft(float deltaTime)
        {
            Vector3 playerToCamera = _cameraTransform.position - _avatar.position;
            
            Vector3 forward = _avatar.transform.position - _cameraTransform.position;
            forward.y = 0;
            
            Vector3 dir = Vector3.Cross(forward, playerToCamera);
            MoveByDirInOneFrame(dir,deltaTime);
        }

        private void MoveRight(float deltaTime)
        {
            Vector3 forward = _avatar.transform.position - _cameraTransform.position;
            forward.y = 0;
            
            Vector3 playerToCamera = _cameraTransform.position - _avatar.position;
            Vector3 dir = -Vector3.Cross(forward, playerToCamera);
            MoveByDirInOneFrame(dir,deltaTime);
        }

        private void MoveByDirInOneFrame(Vector3 dir,float deltaTime)
        {
            dir = Vector3.Normalize(dir);
                
            Vector3 offset = dir * kMoveSpeed * deltaTime;
            Vector3 dest = _avatar.transform.position + dir * _radius + offset;
            
            ETryMoveResultType result = TryMove(dest);
            if (result == ETryMoveResultType.OK)
            {
                _avatar.transform.position += offset;
                _avatar.transform.forward = dir;
            }
            else if (result == ETryMoveResultType.Block)
            {
                Vector3? correctDir = null; 
                
                // Try other directions ,increase 30 degrees each time
                for (float deg = 3.0f;deg < 80.0f;deg += 3.0f)
                {
                    // add degrees
                    Vector3 nextDir = Quaternion.Euler(0, deg, 0) * dir;
                    Vector3 nextOffset = nextDir * kMoveSpeed * deltaTime;
                    Vector3 nextDest = _avatar.transform.position + nextDir * _radius + nextOffset;
                    if (TryMove(nextDest) == ETryMoveResultType.OK)
                    {
                        _avatar.transform.position += nextOffset;
                        correctDir = nextDir;
                        break;
                    }
                    
                    // dec degress
                    nextDir = Quaternion.Euler(0, -deg, 0) * dir;
                    nextOffset = nextDir * kMoveSpeed * deltaTime;
                    nextDest = _avatar.transform.position + nextDir * _radius + nextOffset;
                    if (TryMove(nextDest) == ETryMoveResultType.OK)
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
    }
    
}

