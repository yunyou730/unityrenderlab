using System;
using System.Collections.Generic;
using comet.res;
using UnityEngine;

namespace comet.input
{
    public class InputManager :IDisposable
    {
        public enum EKey
        {
            Up,
            Down,
            Left,
            Right
        }

        public enum EMouseBtn
        {
            Left,
            Right,
            Middle,
        }

        private Dictionary<EKey, KeyCode> _keyMapping = null;
        private Dictionary<EMouseBtn, int> _mouseBtnMapping = null;

        public void Init()
        {
            // key mapping
            _keyMapping = new Dictionary<EKey, KeyCode>();
            _keyMapping.Add(EKey.Up,KeyCode.UpArrow);
            _keyMapping.Add(EKey.Down,KeyCode.DownArrow);
            _keyMapping.Add(EKey.Left,KeyCode.LeftArrow);
            _keyMapping.Add(EKey.Right,KeyCode.RightArrow);
            
            // mouse button mapping
            _mouseBtnMapping = new Dictionary<EMouseBtn, int>();
            _mouseBtnMapping.Add(EMouseBtn.Left,0);
            _mouseBtnMapping.Add(EMouseBtn.Right,1);
            _mouseBtnMapping.Add(EMouseBtn.Middle,2);
        }

        public bool IsKeyDown(EKey key)
        {
            if (_keyMapping.ContainsKey(key))
            {
                var targetKeyCode = _keyMapping[key];
                return Input.GetKey(targetKeyCode);
            }
            return false;
        }

        public bool IsMouseButtonPressing(EMouseBtn mouseBtn)
        {
            if (_mouseBtnMapping.ContainsKey(mouseBtn))
            {
                int mouseBtnCode = _mouseBtnMapping[mouseBtn];
                return Input.GetMouseButton(mouseBtnCode);
            }
            return false;
        }

        public bool IsMouseButtonDown(EMouseBtn mouseBtn)
        {
            if (_mouseBtnMapping.ContainsKey(mouseBtn))
            {
                int mouseBtnCode = _mouseBtnMapping[mouseBtn];
                return Input.GetMouseButtonDown(mouseBtnCode);
            }
            return false;
        }
        
        public bool IsMouseButtonUp(EMouseBtn mouseBtn)
        {
            if (_mouseBtnMapping.ContainsKey(mouseBtn))
            {
                int mouseBtnCode = _mouseBtnMapping[mouseBtn];
                return Input.GetMouseButtonUp(mouseBtnCode);
            }
            return false;
        }

        public Vector3 MousePosition()
        {
            return Input.mousePosition;
        }

        public void Dispose()
        {
            _keyMapping = null;
        }
    }
}