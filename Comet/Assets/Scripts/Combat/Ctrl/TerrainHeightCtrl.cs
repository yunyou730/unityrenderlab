using System;
using UnityEngine;

namespace comet.combat
{
    public class TerrainHeightCtrl : ITerrainModifier,IDisposable
    {
        public ETerrainHeightCtrlType _terrainHeightCtrlType = ETerrainHeightCtrlType.None;
        
        private GameObject _selector = null;
        
        private CombatManager _combat = null;

        private Camera _camera = null;
        private MapRecord _mapRecord = null;
        
        private CmdComp _cmdComp = null;

        public TerrainHeightCtrl(CombatManager combat)
        {
            _combat = combat;
            _cmdComp = _combat.World.GetWorldComp<CmdComp>();
        }

        public void Init(Camera mainCamera, MapRecord mapRecord)
        {
            _camera = mainCamera;
            _mapRecord = mapRecord;
        }

        public bool ShouldWorking()
        {
            return _terrainHeightCtrlType != ETerrainHeightCtrlType.None;
        }

        public void DoJob(int crossPointX, int crossPointY)
        {
            switch (_terrainHeightCtrlType)
            {
                case ETerrainHeightCtrlType.Higher:
                {
                    var param = new ModifyPointHeightParam();
                    param.PointX = crossPointX;
                    param.PointY = crossPointY;
                    param.DeltaValue = 0.1f;    // @temp hardcode
                    
                    Cmd cmd = new Cmd(ECmd.ModifyPointHeight,param);
                    _cmdComp.AddCmd(cmd);
                }
                    break;
                case ETerrainHeightCtrlType.Lower:
                {
                    var param = new ModifyPointHeightParam();
                    param.PointX = crossPointX;
                    param.PointY = crossPointY;
                    param.DeltaValue = -0.1f;    // @temp hardcode
                    
                    Cmd cmd = new Cmd(ECmd.ModifyPointHeight,param);
                    _cmdComp.AddCmd(cmd);                    
                }
                    break;
                default:
                    break;
            }
        }

        public ETerrainModifyCtrlType GetCtrlType()
        {
            //return ETerrainModifyCtrlType.Click;
            return ETerrainModifyCtrlType.Press;
        }

        public void Dispose()
        {
            
        }
    }
}