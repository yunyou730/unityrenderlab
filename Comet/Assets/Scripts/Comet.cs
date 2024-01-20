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
        }

        void Start()
        {
            _serviceLocator = new ServiceLocator();
            _serviceLocator.Register(_config = new Config());
            _serviceLocator.Register(_resManager = new ResManager());
            _serviceLocator.Register(_inputManager = new InputManager()).Init();
            _serviceLocator.Register(_combatManager = new CombatManager()).Init(_mainCamera);
            _serviceLocator.Register(_uiManager = new UIManager()).Init();

            _combatManager.Start();
        }
        
        // Update is called once per frame
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
