using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace comet.input
{
    public class CombatCameraController
    {
        private Camera _mainCamera = null;

        public void Init(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        void Update()
        {
            //if (Input.mousePosition)
            Debug.Log(Input.mousePosition);
            Debug.Log("screen width:" + Screen.width + ", height:" + Screen.height);
        }
    }
}

