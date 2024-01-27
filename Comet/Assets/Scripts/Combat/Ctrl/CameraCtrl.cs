using comet.input;
using UnityEngine;
using UnityEngine.UIElements;

namespace comet.combat
{
    public class CameraCtrl
    {
        private Camera _mainCamera = null;
        private Transform _cameraTransform = null;
        
        private InputManager _input = null;
        private Config _config = null;
        private CombatManager _combat = null;
        
        private Vector2 _cameraMoveDir = Vector2.zero;
        private Vector3 _cameraMoveOffset = Vector3.zero;
        
        
        private const float kHorizonOffsetBuffer = 0.03f;
        private const float kVerticalOffsetBuffer = 0.03f;
        
        private Vector3? _prevFrameMousePos = null;
        private const float kMouseMoveCameraRatio = -0.02f;
        
        public void Init(Camera camera)
        {
            _mainCamera = camera;
            _cameraTransform = _mainCamera.gameObject.transform;
            
            _input = Comet.Instance.ServiceLocator.Get<InputManager>();
            _config = Comet.Instance.ServiceLocator.Get<Config>();
            _combat = Comet.Instance.ServiceLocator.Get<CombatManager>();
            
            InitCameraTransform();
        }
        
        public void OnUpdate(float deltaTime)
        {
            _cameraMoveDir = Vector2.zero;
            KeyboardControl();
            MouseControl();
            CheckAndMove(deltaTime);
        }

        private void KeyboardControl()
        {
            if (_input.IsKeyDown(InputManager.EKey.Up))
            {
                _cameraMoveDir += Vector2.up;
            }
            if (_input.IsKeyDown(InputManager.EKey.Down))
            {
                _cameraMoveDir += Vector2.down;
            }
            if (_input.IsKeyDown(InputManager.EKey.Left))
            {
                _cameraMoveDir += Vector2.left;
            }            
            if (_input.IsKeyDown(InputManager.EKey.Right))
            {
                _cameraMoveDir += Vector2.right;
            }
        }

        private void MouseControl()
        {
            if (_input.IsMouseButtonPressing(InputManager.EMouseBtn.Middle) && _prevFrameMousePos != null)
            {
                Vector3 mousePosThisFrame = _input.MousePosition();
                Vector3 offset = mousePosThisFrame - _prevFrameMousePos.Value;
                offset *= kMouseMoveCameraRatio;
                MoveByOffset(new Vector3(offset.x,0,offset.y));
            }
            _prevFrameMousePos = _input.MousePosition();
        }

        private void CheckAndMove(float deltaTime)
        {
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
            Vector3 nextPosition = _cameraTransform.position + offset;
            
            if ((offset.z > 0 && nextPosition.z > MaxZ())
                ||(offset.z < 0 && nextPosition.z < MinZ()))
            {
                offset.z = 0;
                nextPosition = _cameraTransform.position + offset;
            }

            if ((offset.x < 0 && nextPosition.x < MinX())
                ||(offset.x > 0 && nextPosition.x > MaxX()))
            {
                offset.x = 0;
                nextPosition = _cameraTransform.position + offset;
            }

            _mainCamera.transform.position = nextPosition;
        }

        private float MaxZ()
        {
            Rect gridMapBounds = _combat.GetGfxGridMap().GetBounds();
            float gridMaxZ = gridMapBounds.y + gridMapBounds.height;
            
            Vector3 cameraForward = _mainCamera.transform.forward;
            float cameraHeight = _mainCamera.transform.position.y;  // here assume GricMap.y = 0, should subtract this value
            
            Quaternion rot = Quaternion.AngleAxis(_mainCamera.fieldOfView * (0.5f - kVerticalOffsetBuffer), Vector3.left);
            Vector3 upperDir = rot * cameraForward;

            float angleInRad = Mathf.Acos(Vector3.Dot(upperDir, Vector3.down));
            float toUpperOffset = Mathf.Tan(angleInRad) * cameraHeight;
            
            return gridMaxZ - toUpperOffset;
        }

        private float MinZ()
        {
            Rect gridMapBounds = _combat.GetGfxGridMap().GetBounds();
            float gridMinZ = gridMapBounds.y;
            
            Vector3 cameraForward = _mainCamera.transform.forward;
            float cameraHeight = _mainCamera.transform.position.y;  // here assume GricMap.y = 0, should subtract this value
            
            Quaternion rot = Quaternion.AngleAxis(_mainCamera.fieldOfView * (0.5f - kVerticalOffsetBuffer), Vector3.right);
            Vector3 lowerDir = rot * cameraForward;
            
            float angleInRad = Mathf.Acos(Vector3.Dot(lowerDir, Vector3.down));
            float toLowerOffset = Mathf.Tan(angleInRad) * cameraHeight;
            
            return gridMinZ + toLowerOffset;
        }

        private float MinX()
        {
            Rect gridMapBounds = _combat.GetGfxGridMap().GetBounds();
            float gridMinX = gridMapBounds.x;
            
            float fovX = Camera.VerticalToHorizontalFieldOfView(_mainCamera.fieldOfView,_mainCamera.aspect) * Mathf.Deg2Rad;

            float cameraHeight = _mainCamera.transform.position.y;  // assume y = 0;
            
            Vector3 cameraDir = _mainCamera.transform.forward;
            float cameraDirToGridMapDistance = cameraHeight * Vector3.Dot(cameraDir, Vector3.down);
            float horizonOffset = Mathf.Tan(fovX * (0.5f - kHorizonOffsetBuffer)) * cameraDirToGridMapDistance;
            
            return gridMinX + horizonOffset;
        }

        private float MaxX()
        {
            Rect gridMapBounds = _combat.GetGfxGridMap().GetBounds();
            float gridMaxX = gridMapBounds.x + gridMapBounds.width;
            
            float fovX = Camera.VerticalToHorizontalFieldOfView(_mainCamera.fieldOfView,_mainCamera.aspect) * Mathf.Deg2Rad;

            float cameraHeight = _mainCamera.transform.position.y;  // assume y = 0;
            
            Vector3 cameraDir = _mainCamera.transform.forward;
            float cameraDirToGridMapDistance = cameraHeight * Vector3.Dot(cameraDir, Vector3.down);
            float horizonOffset = Mathf.Tan(fovX * (0.5f - kHorizonOffsetBuffer)) * cameraDirToGridMapDistance;
            
            return gridMaxX - horizonOffset;
        }

    }
}