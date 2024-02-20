using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using comet.combat;
using comet.core;
using comet.input;
using comet.res;

namespace comet
{
    public class Comet : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera = null;
        [SerializeField] private UnityEngine.UI.Image _miniMapImage = null;
        
        private ServiceLocator _serviceLocator = null;
        public ServiceLocator ServiceLocator
        {
            get { return _serviceLocator; }
        }

        private Config _config = null;
        private ResManager _resManager = null;
        private InputManager _inputManager = null;
        private CombatManager _combatManager = null;
        private UIManager _uiManager = null;
        
        private static Comet _sInstance = null;
        public static Comet Instance { get { return _sInstance; } }
        
        private void Awake()
        {
            _sInstance = this;
            
            _serviceLocator = new ServiceLocator();
            _serviceLocator.Register(_config = new Config());
            _serviceLocator.Register(_resManager = new ResManager());
            _serviceLocator.Register(_inputManager = new InputManager());
            _serviceLocator.Register(_combatManager = new CombatManager());
            _serviceLocator.Register(_uiManager = new UIManager());
        }

        void Start()
        {
            _inputManager.Init();
            _combatManager.Init(_mainCamera,_miniMapImage);
            _uiManager.Init();

            _combatManager.Start();
        }
        
        void Update()
        {
            _combatManager?.OnUpdate(Time.deltaTime);
        }

        private void OnGUI()
        {
            _uiManager?.OnGUI();
        }

        private void OnDestroy()
        {
            _inputManager.Dispose();
            _combatManager.Dispose();
            _resManager.Dispose();
        }
    }
    
}
