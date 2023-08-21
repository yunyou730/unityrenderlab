using System;
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

        public Vector3 _targetOffset = new Vector3(4,15,4);

        private static float kRotSpeed = 1440;

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
            FollowTarget();
            HandleRotateByTarget(deltaTime);
        }

        private void FollowTarget()
        {
            if (_cameraTransform != null & _lookingTarget != null)
            {
                _cameraTransform.position = _lookingTarget.position + _targetOffset;
                _cameraTransform.LookAt(_lookingTarget);
            }
        }
        
        private void HandleRotateByTarget(float deltaTime)
        {
            if (Mathf.Abs(Input.mouseScrollDelta.y) > Double.Epsilon)
            {
                float scrollValue = Input.mouseScrollDelta.y > 0 ? kRotSpeed : -kRotSpeed;
                scrollValue *= deltaTime;
                Vector3 current = (_cameraTransform.position - _lookingTarget.position);
                Vector3 next = Quaternion.Euler(0, scrollValue, 0) * current;
                _targetOffset = next;

            }

        }
    }
}

