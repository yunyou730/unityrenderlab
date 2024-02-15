using System;
using comet.input;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class TerrainTextureCtrl : IDisposable
    {
        public ETerrainTextureType _selectedTerrainTexture = ETerrainTextureType.None;

        private CombatManager _combat = null;
        InputManager _input = null;
        private ResManager _res = null;

        private Camera _mainCamera = null;
        private MapRecord _mapRecord = null;

        private CmdComp _cmdComp = null;

        public ETerrainTextureType SelectedTerrainTexture
        {
            get => _selectedTerrainTexture;
            set => _selectedTerrainTexture = value;
        }

        private GameObject _terrainGridSelector = null;
        
        public TerrainTextureCtrl(CombatManager combat)
        {
            _combat = combat;
            _input = Comet.Instance.ServiceLocator.Get<InputManager>();
            _res = Comet.Instance.ServiceLocator.Get<ResManager>();
            
            _cmdComp = _combat.World.GetWorldComp<CmdComp>();
        }

        public void Init(Camera mainCamera,MapRecord mapRecord)
        {
            _mainCamera = mainCamera;
            _mapRecord = mapRecord;

            float gridScale = _mapRecord.GridSize * 2.0f;
            GameObject prefab = _res.Load<GameObject>("Prefabs/TerrainGridSelector");
            _terrainGridSelector = GameObject.Instantiate(prefab);
            _terrainGridSelector.transform.localScale = new Vector3(gridScale,gridScale,1);
        }

        public void OnUpdate()
        {
            // Check shall we enable Brush Feature 
            if (_selectedTerrainTexture == ETerrainTextureType.None)
            {
                _terrainGridSelector.SetActive(false);
                return;
            }
            _terrainGridSelector.SetActive(true);
            
            
            // Brush Terrain Texture on Grids
            Ray ray = _mainCamera.ScreenPointToRay(_input.MousePosition());
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit))
            {
                Vector2Int pointCoord = Vector2Int.zero;
                MoveGridSelector(hit.point,ref pointCoord);
                
                GfxGridMap gfxMap = hit.transform.GetComponent<GfxGridMap>();
                if (gfxMap != null && _input.IsMouseButtonPressing(InputManager.EMouseBtn.Left))
                {
                    MarkPointTerrainTexture(pointCoord.x,pointCoord.y);
                }
            }
        }

        public void Dispose()
        {
            GameObject.Destroy(_terrainGridSelector);
        }

        private void MoveGridSelector(Vector3 point,ref Vector2Int atPoint)
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
            _terrainGridSelector.transform.localPosition = pos;
            
            // Pass point coordinate result 
            atPoint.x = pointX;
            atPoint.y = pointY;
        }

        private void MarkPointTerrainTexture(int pointX,int pointY)
        {
            PointRecord pointRecord = _mapRecord.GetPointAt(pointY, pointX);
            if (pointRecord != null)
            {
                var param = new SpecifyPointTextureTypeParam();
                param.PointX = pointX;
                param.PointY = pointY;
                param.TextureLayer = GetLayerIndex();
                param.PointTextureType = _selectedTerrainTexture;
                
                Cmd cmd = new Cmd(ECmd.SpecifyPointTexture,param);
                _cmdComp.AddCmd(cmd);
            }
        }
        
        private ETerrainTextureLayer GetLayerIndex()
        {
            switch (_selectedTerrainTexture)
            {
                case ETerrainTextureType.Ground:
                    return ETerrainTextureLayer.BaseLayer;
                case ETerrainTextureType.Grass:
                    return ETerrainTextureLayer.DecoratorLayer;
                default:
                    return ETerrainTextureLayer.Max;
            }
        }
    }
}