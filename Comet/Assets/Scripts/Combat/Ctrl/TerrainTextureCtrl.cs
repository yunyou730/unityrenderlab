using System;
using comet.input;
using comet.res;
using UnityEngine;

namespace comet.combat
{
    public class TerrainTextureCtrl : ITerrainModifier,IDisposable
    {
        public ETerrainTextureType _selectedTerrainTexture = ETerrainTextureType.None;

        private CombatManager _combat = null;
        InputManager _input = null;
        private ResManager _res = null;

        private Camera _mainCamera = null;
        private MapRecord _mapRecord = null;

        private CmdComp _cmdComp = null;

        private TerrainCrossPointSelector _crossPointSelector = null;

        public ETerrainTextureType SelectedTerrainTexture
        {
            get => _selectedTerrainTexture;
            set => _selectedTerrainTexture = value;
        }

        // private GameObject _terrainGridSelector = null;
        
        public TerrainTextureCtrl(CombatManager combat)
        {
            _combat = combat;
            _input = Comet.Instance.ServiceLocator.Get<InputManager>();
            _res = Comet.Instance.ServiceLocator.Get<ResManager>();
            
            _cmdComp = _combat.World.GetWorldComp<CmdComp>();
        }

        public void Init(Camera mainCamera,MapRecord mapRecord,TerrainCrossPointSelector crossPointSelector)
        {
            _mainCamera = mainCamera;
            _mapRecord = mapRecord;
            _crossPointSelector = crossPointSelector;
        }

        public bool ShouldWorking()
        {
            return _selectedTerrainTexture != ETerrainTextureType.None;
        }

        public void DoJob(int crossPointX,int crossPointY)
        {
            MarkPointTerrainTexture(crossPointX,crossPointY);
        }

        public ETerrainModifyCtrlType GetCtrlType()
        {
            return ETerrainModifyCtrlType.Press;
        }

        public void Dispose()
        {
            
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