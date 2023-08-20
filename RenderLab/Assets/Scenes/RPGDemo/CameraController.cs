using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rpg
{
    public class CameraController
    {
        private GameObject _cameraGameObject = null;
        private Camera _ctrlCamera = null;
        private Transform _cameraTransform = null;
        private Transform _lookingTarget = null;

        public Vector3 targetOffset = new Vector3(5,5,5);

        public CameraController()
        {
            
        }

        public void SetCamera(GameObject cameraGameObject)
        {
            _cameraGameObject = cameraGameObject;
            _cameraTransform = _cameraGameObject.transform;
            _ctrlCamera = _cameraGameObject.GetComponent<Camera>();
        }

        public void SetLookTarget(Transform lookTarget)
        {
            _lookingTarget = lookTarget;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_cameraTransform != null & _lookingTarget != null)
            {
                FollowTarget();
            }
        }

        private void FollowTarget()
        {
            _cameraTransform.position = _lookingTarget.position + targetOffset;
            _cameraTransform.LookAt(_lookingTarget);
        }
    }
}

