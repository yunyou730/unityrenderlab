using UnityEngine;

namespace ayy.jy
{
    /// <summary>
    /// Keyboard controller for a top-down / isometric perspective camera.
    /// WASD moves the camera on the ground plane, Q zooms in and E zooms out.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class JYTopDownCameraController : MonoBehaviour
    {
        public float _moveSpeed = 300.0f;
        public float _zoomSpeed = 300.0f;
        public float _minZoomDistance = 30.0f;
        public float _maxZoomDistance = 3000.0f;
        public float _groundHeight = 0.0f;

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

            Vector3 focusPoint;
            if (!TryGetGroundFocusPoint(out focusPoint))
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, focusPoint);
            float targetDistance = Mathf.Clamp(
                distance + zoomInput * _zoomSpeed * deltaTime,
                _minZoomDistance,
                _maxZoomDistance);

            // Keep the same view direction and ground focus point. This is a dolly
            // zoom, so perspective distortion stays constant while zooming.
            transform.position += transform.forward * (distance - targetDistance);
        }

        private bool TryGetGroundFocusPoint(out Vector3 focusPoint)
        {
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0.0f, _groundHeight, 0.0f));
            Ray viewRay = new Ray(transform.position, transform.forward);

            float enter;
            if (groundPlane.Raycast(viewRay, out enter))
            {
                focusPoint = viewRay.GetPoint(enter);
                return true;
            }

            focusPoint = new Vector3(transform.position.x, _groundHeight, transform.position.z);
            return false;
        }
    }
}
