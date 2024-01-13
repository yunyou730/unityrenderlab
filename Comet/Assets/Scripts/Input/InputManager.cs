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

        private Dictionary<EKey, KeyCode> _keyMapping = null;

        public void Init()
        {
            _keyMapping = new Dictionary<EKey, KeyCode>();
            _keyMapping.Add(EKey.Up,KeyCode.UpArrow);
            _keyMapping.Add(EKey.Down,KeyCode.DownArrow);
            _keyMapping.Add(EKey.Left,KeyCode.LeftArrow);
            _keyMapping.Add(EKey.Right,KeyCode.RightArrow);
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
        
        public void Test()
        {
            //var v = new Comet();
            var resManager = Comet.Instance.ServiceLocator.Get<ResManager>();

        }

        public void Dispose()
        {
            _keyMapping = null;
        }
    }
}