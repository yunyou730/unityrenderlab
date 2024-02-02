using System;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class TerrainGridSelector : IDisposable
    {
        private GameObject _gameObject = null;
        private ResManager _res = null;
        private MapRecord _mapRecord = null;

        public TerrainGridSelector(MapRecord mapRecord)
        {
            _res = Comet.Instance.ServiceLocator.Get<ResManager>();
            _mapRecord = mapRecord;
        }

        public void Init(MapRecord mapRecord)
        {
            float gridScale = mapRecord.GridSize * 2.0f;
            GameObject prefab = _res.Load<GameObject>("Prefabs/TerrainGridSelector");
            _gameObject = GameObject.Instantiate(prefab);
            _gameObject.transform.localScale = new Vector3(gridScale,gridScale,1);
        }

        public void OnUpdate(float deltaTime)
        {
            // MoveGameObject();
        }

        public void Dispose()
        {
            GameObject.Destroy(_gameObject);
        }
        
        private Vector2Int MoveGameObject(Vector3 point)
        {
            Vector3 pos = point;
            
            // Move select GameObject 
            int gridX = (int)(pos.x/_mapRecord.GridSize);
            int gridY = (int)(pos.z / _mapRecord.GridSize);
            
            float x = pos.x - gridX * _mapRecord.GridSize;
            float y = pos.z - gridY * _mapRecord.GridSize;
            
            
            gridX = x > 0.5 ? gridX + 1 : gridX;
            gridY = y > 0.5 ? gridY + 1 : gridY;

            pos = Metrics.GetGridOriginPos(_mapRecord, gridX, gridY);
            pos.y = 0.0001f;
            _gameObject.transform.localPosition = pos;

            Vector2Int result = new Vector2Int(gridX,gridY);
            return result;
        }
    }
}