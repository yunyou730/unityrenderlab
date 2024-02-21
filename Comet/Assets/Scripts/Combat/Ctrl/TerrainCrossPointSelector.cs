using System;
using comet.input;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class TerrainCrossPointSelector : IDisposable
    {
        private GameObject _selectorGameObject = null;
        private Material _selectorGameObjectMaterial = null;

        private InputManager _input = null;
        private ResManager _res = null;
        
        private MapRecord _mapRecord = null;

        private Camera _mainCamera = null;
        private TerrainHeightCtrl _terrainHeightCtrl = null;
        private TerrainTextureCtrl _terrainTextureCtrl = null;

        private TerrainDepthTextureProvider _terrainDepthTextureProvider = null;

        public TerrainCrossPointSelector(MapRecord mapRecord,
                                        TerrainTextureCtrl terrainTextureCtrl,
                                        TerrainHeightCtrl terrainHeightCtrl,
                                        TerrainDepthTextureProvider terrainDepthTextureProvider)
        {
            _res = Comet.Instance.ServiceLocator.Get<ResManager>();
            _input = Comet.Instance.ServiceLocator.Get<InputManager>();
            
            _mapRecord = mapRecord;
            
            _terrainTextureCtrl = terrainTextureCtrl;
            _terrainHeightCtrl = terrainHeightCtrl;

            _terrainDepthTextureProvider = terrainDepthTextureProvider;
        }

        public void Init(Camera mainCamera)
        {
            _mainCamera = mainCamera;
            
            // Create Selector GameObject
            float gridScale = _mapRecord.GridSize * 2.0f;
            GameObject prefab = _res.Load<GameObject>("Prefabs/TerrainGridSelector");
            _selectorGameObject = GameObject.Instantiate(prefab);
            // z large enough ,for present decal on higher terrain grid
            _selectorGameObject.transform.localScale = new Vector3(gridScale,gridScale,5);
            
            // Assign terrain depth texture to material
            _selectorGameObjectMaterial = _selectorGameObject.GetComponent<MeshRenderer>().material;
            _selectorGameObjectMaterial.SetTexture(Shader.PropertyToID("_TerrainDepthTexture"),_terrainDepthTextureProvider.GetTerrainDepthTexture());
            
        }
        
        public void Dispose()
        {
            GameObject.Destroy(_selectorGameObject);
            _selectorGameObject = null;
        }

        public void SetActive(bool bActive)
        {
            _selectorGameObject.SetActive(bActive);
        }

        public void OnUpdate()
        {
            // Shall we show the selector 
            var terrainModifier = GetWorkingTerrainModifier();
            if (terrainModifier == null)
            {
                SetActive(false);
                return;
            }
            SetActive(true);

            // If show, check which points & grids are we selected, and modify properties on them.
            Ray ray = _mainCamera.ScreenPointToRay(_input.MousePosition());
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit))
            {
                Vector2Int pointCoord = Vector2Int.zero;
                MoveGridSelector(hit.point, ref pointCoord);
                
                GfxGridMap gfxMap = hit.transform.GetComponent<GfxGridMap>();
                if (gfxMap)
                {
                    // 2 kind of ctrl type 
                    bool bShallDoJob = terrainModifier.GetCtrlType() == ETerrainModifyCtrlType.Press
                                       && _input.IsMouseButtonPressing(InputManager.EMouseBtn.Left);
                    if (!bShallDoJob)
                    {
                        bShallDoJob = terrainModifier.GetCtrlType() == ETerrainModifyCtrlType.Click
                                      && _input.IsMouseButtonDown(InputManager.EMouseBtn.Left);
                    }

                    // Do job 
                    if (bShallDoJob)
                    {
                        terrainModifier.DoJob(pointCoord.x,pointCoord.y); 
                    }

                }
            }
        }

        private ITerrainModifier GetWorkingTerrainModifier()
        {
            ITerrainModifier terrainModifier = null;

            if (_terrainTextureCtrl.ShouldWorking())
            {
                terrainModifier = _terrainTextureCtrl;
            }
            else if (_terrainHeightCtrl.ShouldWorking())
            {
                terrainModifier = _terrainHeightCtrl;
            }

            return terrainModifier;
        }

        public void MoveGridSelector(Vector3 point,ref Vector2Int atPoint)
        {
            // Calc point coordinate 
            Vector3 pos = point;
            
            int gridX = (int)(pos.x / _mapRecord.GridSize);
            int gridY = (int)(pos.z / _mapRecord.GridSize);
            
            float xPct = (pos.x - gridX * _mapRecord.GridSize)/_mapRecord.GridSize;
            float yPct = (pos.z - gridY * _mapRecord.GridSize)/_mapRecord.GridSize;
            
            int pointX = xPct < 0.5 ? gridX : gridX + 1;
            int pointY = yPct < 0.5 ? gridY : gridY + 1;
            
            // Do move by point coordinate 
            pos = Metrics.GetPosByPointCoordinate(_mapRecord,pointX,pointY);
            pos.y = 0.0001f;
            _selectorGameObject.transform.localPosition = pos;
            
            // Pass point coordinate result 
            atPoint.x = pointX;
            atPoint.y = pointY;
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
            _selectorGameObject.transform.localPosition = pos;

            Vector2Int result = new Vector2Int(gridX,gridY);
            return result;
        }
    }
}