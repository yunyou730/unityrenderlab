using UnityEngine;

namespace ayy.jy
{
    /// <summary>
    /// Keyboard controller for a top-down / isometric orthographic camera.
    /// WASD moves the camera on the ground plane, Q shrinks the orthographic
    /// size (zoom in) and E grows it (zoom out).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class JYTopDownCameraController : MonoBehaviour
    {
        public float _moveSpeed = 300.0f;
        public float _zoomSpeed = 100.0f;
        public float _minOrthoSize = 10.0f;
        public float _maxOrthoSize = 500.0f;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            MoveCamera(deltaTime);
            ZoomCamera(deltaTime);
        }

        private void MoveCamera(float deltaTime)
        {
            // Screen-up on the ground plane. For a straight top-down camera,
            // transform.forward is almost parallel to the plane, so transform.up
            // is the stable basis for W/S.
            Vector3 forward = Vector3.ProjectOnPlane(transform.up, Vector3.up);
            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            }

            Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up);
            forward.Normalize();
            right.Normalize();

            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) move += forward;
            if (Input.GetKey(KeyCode.S)) move -= forward;
            if (Input.GetKey(KeyCode.D)) move += right;
            if (Input.GetKey(KeyCode.A)) move -= right;

            if (move.sqrMagnitude > 1.0f)
            {
                move.Normalize();
            }

            transform.position += move * (_moveSpeed * deltaTime);
        }

        private void ZoomCamera(float deltaTime)
        {
            float zoomInput = 0.0f;
            if (Input.GetKey(KeyCode.Q)) zoomInput -= 1.0f;
            if (Input.GetKey(KeyCode.E)) zoomInput += 1.0f;
            if (Mathf.Approximately(zoomInput, 0.0f))
            {
                return;
            }

            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            // The orthographic size only defines the visible area for an
            // orthographic camera, so zooming is a no-op in perspective mode.
            if (_camera == null || !_camera.orthographic)
            {
                return;
            }

            // The camera stays where it is, so the visible ground region grows
            // and shrinks around the point under the screen center.
            _camera.orthographicSize = Mathf.Clamp(
                _camera.orthographicSize + zoomInput * _zoomSpeed * deltaTime,
                _minOrthoSize,
                _maxOrthoSize);
        }
    }
}
