using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace rpg
{
    public class ObstacleManager
    {
        private Vector3 _boundingBoxMin;
        private Vector3 _boundingBoxMax;
        private GameObject _root = null;
        private int _objstacleCount = 50;
        
        private enum EVisionObsType
        {
            Box,
            Cylinder,
            Max
        }
        
        private GameObject _boxPrefab = null;
        private GameObject _cylinderPrefab = null;

        public ObstacleManager(GameObject boxPrefab,GameObject cylinderPrefab)
        {
            _boxPrefab = boxPrefab;
            _cylinderPrefab = cylinderPrefab;
        }

        public void CreateObstacleGameObjects(Vector3 min,Vector3 max)
        {
            _root = new GameObject("[vision root]");
            _boundingBoxMin = min;
            _boundingBoxMax = max;
            
            for (int i = 0 ;i < _objstacleCount;i++)
            {
                DoCreateOneObstacle();
            }
        }

        private void DoCreateOneObstacle()
        {
            float x = UnityEngine.Random.Range(_boundingBoxMin.x, _boundingBoxMax.x);
            float z = UnityEngine.Random.Range(_boundingBoxMin.z, _boundingBoxMax.z);

            EVisionObsType theType = (EVisionObsType)UnityEngine.Random.Range(0, (int)EVisionObsType.Max);

            // theType = EVisionObsType.Box;  // @miao @temp
            GameObject go = null;

            Vector3 randScale = Vector3.one;
            switch (theType)
            {
                case EVisionObsType.Box:
                    go = GameObject.Instantiate(_boxPrefab);
                    randScale = new Vector3(UnityEngine.Random.Range(1,3f),1,UnityEngine.Random.Range(1,3));
                    break;
                case EVisionObsType.Cylinder:
                    go = GameObject.Instantiate(_cylinderPrefab);
                    float r = UnityEngine.Random.Range(1, 3f);
                    randScale = new Vector3(r, 1, r);
                    break;
            }
                
            go.transform.SetParent(_root.transform);
            go.transform.localPosition = new Vector3(x, 0, z);
            go.transform.localRotation = Quaternion.Euler(0f,UnityEngine.Random.Range(0,360f),0f);
            go.transform.localScale = randScale;
        }

        public void Dispose()
        {
            
        }
    }
    
}
