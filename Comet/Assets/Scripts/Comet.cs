using System.Collections;
using System.Collections.Generic;
using comet.combat;
using comet.core;
using comet.input;
using Unity.VisualScripting;
using UnityEngine;

namespace comet
{
    public class Comet : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera = null;

        public ServiceLocator _serviceLocator = null;

        private CombatCameraController _combatCameraCtrl = null;
        
        void Start()
        {
            _serviceLocator = new ServiceLocator();
            
            _combatCameraCtrl = _serviceLocator.Register<CombatCameraController>(new CombatCameraController());
            _combatCameraCtrl.Init(_mainCamera);
            
            CreateGridMap();
        }
        
        void CreateGridMap()
        {
            var t = Resources.Load<GameObject>("Prefabs/GridMap");
            var gridMap = GameObject.Instantiate(t).GetComponent<GridMap>();
            
            MapRecord mapRecord = new MapRecord(100,100);
            mapRecord.GenerateGrids(0,0);
            gridMap.RefreshMap(mapRecord,0.2f);
        }

        // Update is called once per frame
        void Update()
        {
            if (_combatCameraCtrl != null)
            {
                
            }
        }
    }
    
}
