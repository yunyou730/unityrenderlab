using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class MovementController
    {
        private Transform _avatar = null;

        private static float kTurnSpeed = 180.0f;    // degree
        private static float kMoveSpeed = 7.0f;

        public MovementController(Transform avatar)
        {
            _avatar = avatar;
        }

        public void SetPosByLayerAndTileCoord(Layer layer,Vector2Int tileCoord)
        {
            Vector3 worldPos = Metrics.GetTileWorldPos(layer,tileCoord.x, tileCoord.y);
            _avatar.transform.position = worldPos;
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
                TurnLeft(deltaTime);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                TurnRight(deltaTime);
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
            Vector3 dir = _avatar.transform.forward;
            Vector3 offset = dir * kMoveSpeed * deltaTime;
            _avatar.transform.position += offset;
        }

        private void MoveBackward(float deltaTime)
        {
            Vector3 dir = -_avatar.transform.forward;
            Vector3 offset = dir * kMoveSpeed * deltaTime;
            _avatar.transform.position += offset;            
        }
    }
    
}

