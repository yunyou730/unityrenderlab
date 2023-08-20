using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class MovementController
    {
        private Transform _avatar = null;

        private static float kTurnSpeed = 3.14f;
        private static float kMoveSpeed = 0.5f;

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
            else if (Input.GetKey(KeyCode.LeftArrow))
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
            Debug.Log("Turn Left");   
        }

        private void TurnRight(float deltaTime)
        {
            Debug.Log("Turn Right");   
        }

        private void MoveForward(float deltaTime)
        {
            Debug.Log("Move Forward");   
        }

        private void MoveBackward(float deltaTime)
        {
            Debug.Log("Move Backward");
        }
    }
    
}

