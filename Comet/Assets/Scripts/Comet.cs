using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
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

        private ResManager _resManager = null;
        private InputManager _inputManager = null;
        private CombatManager _combatManager = null;

        
        private static Comet _sInstance = null;
        public static Comet Instance { get { return _sInstance; } }
        
        private void Awake()
        {
            _sInstance = this;
        }

        void Start()
        {
            _serviceLocator = new ServiceLocator();
            
            _resManager = _serviceLocator.Register<ResManager>(new ResManager());
            _inputManager = _serviceLocator.Register<InputManager>(new InputManager());
            _combatManager = _serviceLocator.Register<CombatManager>(new CombatManager());

            EnterCombat();
        }
        
        // Update is called once per frame
        void Update()
        {
            // if (_combatCameraCtrl != null)
            // {
            //     
            // }
        }

        private void OnDestroy()
        {
            
        }

        private void EnterCombat()
        {
            _combatManager.CreateMapRecord();
            _combatManager.CreateGridMapGfx();
        }
    }
    
}
