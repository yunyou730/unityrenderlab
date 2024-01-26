using System;
using comet.core;
using comet.input;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class TerrainTextureCtrl : IDisposable
    {
        public EGridTextureType _selectedTerrainTexture = EGridTextureType.None;

        private CombatManager _combat = null;
        InputManager _input = null;
        private ResManager _res = null;

        private Camera _mainCamera = null;
        private MapRecord _mapRecord = null;

        private CmdComp _cmdComp = null;

        public EGridTextureType SelectedTerrainTexture
        {
            get => _selectedTerrainTexture;
            set => _selectedTerrainTexture = value;
        }

        private GameObject _terrainGridSelectorGameObject = null;
        
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
            _terrainGridSelectorGameObject = GameObject.Instantiate(prefab);
            _terrainGridSelectorGameObject.transform.localScale = new Vector3(gridScale,gridScale,1);
        }

        public void OnUpdate()
        {
            if (_selectedTerrainTexture == EGridTextureType.None)
            {
                _terrainGridSelectorGameObject.SetActive(false);
                return;
            }
            _terrainGridSelectorGameObject.SetActive(true);

            Ray ray = _mainCamera.ScreenPointToRay(_input.MousePosition());
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit))
            {
                GfxGridMap gfxMap = hit.transform.GetComponent<GfxGridMap>();
                if (gfxMap != null)
                {
                    Vector2Int atGrid = MoveGameObject(hit.point);
                    if (_input.IsMouseButtonPressing(InputManager.EMouseBtn.Left))
                    {
                        MarkGridsTerrainTexture(atGrid.x,atGrid.y);
                    }
                }
            }
        }

        public void Dispose()
        {
            GameObject.Destroy(_terrainGridSelectorGameObject);
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
            _terrainGridSelectorGameObject.transform.localPosition = pos;

            Vector2Int result = new Vector2Int(gridX,gridY);
            return result;
        }

        private void MarkGridsTerrainTexture(int gridX,int gridY)
        {
            Vector2Int[] gridCoords = new Vector2Int[4]; 
            gridCoords[0] = new Vector2Int(gridX,gridY);
            gridCoords[1] = new Vector2Int(gridX - 1,gridY);
            gridCoords[2] = new Vector2Int(gridX,gridY - 1);
            gridCoords[3] = new Vector2Int(gridX - 1,gridY - 1);
            
            for (int i = 0;i < gridCoords.Length;i++)
            {
                // var param = new SetGridTypeParam();
                // param.GridX = gridCoords[i].x;
                // param.GridY = gridCoords[i].y;
                // param.GridType = EGridType.Wall;
                //
                // Cmd cmd = new Cmd(ECmd.SetGridType,param);
                // _cmdComp.AddCmd(cmd);

                var param = new SetGridTextureTypeParam();
                param.GridX = gridCoords[i].x;
                param.GridY = gridCoords[i].y;
                param.TextureLayer = 1;
                param.GridTextureType = _selectedTerrainTexture;
                
                Cmd cmd = new Cmd(ECmd.SetGridTexture,param);
                _cmdComp.AddCmd(cmd);
            }
        }
    }
}