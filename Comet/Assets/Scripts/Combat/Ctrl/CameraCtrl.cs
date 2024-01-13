using comet.input;
using UnityEngine;
using UnityEngine.UIElements;

namespace comet.combat
{
    public class CameraCtrl
    {
        private Camera _mainCamera = null;
        private Transform _cameraTransform = null;
        
        private InputManager _inputManager = null;
        private Config _config = null;
        private CombatManager _combat = null;
        

        private Vector2 _cameraMoveDir = Vector2.zero;
        private Vector3 _cameraMoveOffset = Vector3.zero;
        
        public void Init(Camera camera)
        {
            _mainCamera = camera;
            _cameraTransform = _mainCamera.gameObject.transform;
            
            _inputManager = Comet.Instance.ServiceLocator.Get<InputManager>();
            _config = Comet.Instance.ServiceLocator.Get<Config>();
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
            
            InitCameraTransform();
        }
        
        public void OnUpdate(float deltaTime)
        {
            _cameraMoveDir = Vector2.zero;
            if (_inputManager.IsKeyDown(InputManager.EKey.Up))
            {
                _cameraMoveDir += Vector2.up;
            }
            if (_inputManager.IsKeyDown(InputManager.EKey.Down))
            {
                _cameraMoveDir += Vector2.down;
            }
            if (_inputManager.IsKeyDown(InputManager.EKey.Left))
            {
                _cameraMoveDir += Vector2.left;
            }            
            if (_inputManager.IsKeyDown(InputManager.EKey.Right))
            {
                _cameraMoveDir += Vector2.right;
            }

            if (_cameraMoveDir.magnitude > 0)
            {
                Vector2 offset2D = _cameraMoveDir.normalized * deltaTime * _config.CameraMoveSpeed;
                
                _cameraMoveOffset.x = offset2D.x;
                _cameraMoveOffset.y = 0;
                _cameraMoveOffset.z = offset2D.y;
                MoveByOffset(_cameraMoveOffset);
            }
        }

        private void InitCameraTransform()
        {
            _cameraTransform.position = _config.CameraInitPosition;
            _cameraTransform.rotation = Quaternion.Euler(_config.CameraInitEuler);
        }

        private void MoveByOffset(Vector3 offset)
        {
            Vector3 nextPosition = _cameraTransform.position + _cameraMoveOffset;
            
            if (offset.z > 0 && nextPosition.z > MaxZ())
            {
                return;
            }

            if (offset.z < 0 && nextPosition.z < MinZ())
            {
                return;
            }
            
            _mainCamera.transform.position = nextPosition;
        }

        private float MaxZ()
        {
            Rect gridMapBounds = _combat.GridMap.GetBounds();
            float gridMaxZ = gridMapBounds.y + gridMapBounds.height;
            
            Vector3 cameraForward = _mainCamera.transform.forward;
            float cameraHeight = _mainCamera.transform.position.y;  // here assume GricMap.y = 0, should subtract this value
            
            Quaternion rot = Quaternion.AngleAxis(_mainCamera.fieldOfView * 0.5f, Vector3.left);
            Vector3 upperDir = rot * cameraForward;

            float angleInRad = Mathf.Acos(Vector3.Dot(upperDir, Vector3.down));
            float toUpperOffset = Mathf.Tan(angleInRad) * cameraHeight;
            
            return gridMaxZ - toUpperOffset;
        }

        private float MinZ()
        {
            Rect gridMapBounds = _combat.GridMap.GetBounds();
            float gridMinZ = gridMapBounds.y;
            
            Vector3 cameraForward = _mainCamera.transform.forward;
            float cameraHeight = _mainCamera.transform.position.y;  // here assume GricMap.y = 0, should subtract this value
            
            Quaternion rot = Quaternion.AngleAxis(_mainCamera.fieldOfView * 0.5f, Vector3.right);
            Vector3 lowerDir = rot * cameraForward;
            
            float angleInRad = Mathf.Acos(Vector3.Dot(lowerDir, Vector3.down));
            float toLowerOffset = Mathf.Tan(angleInRad) * cameraHeight;
            
            return gridMinZ + toLowerOffset;
        }


    }
}