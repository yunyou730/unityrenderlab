using UnityEngine;
using UnityEngine.XR;

namespace comet.combat
{
    public class CmdSys : BaseSys, ITickSys
    {
        private CmdComp _cmd = null;
        private UserCtrlComp _userCtrl = null;
        private MapComp _map = null;
        
        public CmdSys(World world) : base(world)
        {
            _cmd = _world.GetWorldComp<CmdComp>();
            _userCtrl = _world.GetWorldComp<UserCtrlComp>();
            _map = _world.GetWorldComp<MapComp>();
        }

        public void OnTick()
        {
            for(int i = 0;i < _cmd.cmds.Count;i++)
            {
                HandleCmd(_cmd.cmds[i]);
            }
            _cmd.ClearCmd();
        }

        private void HandleCmd(Cmd cmd)
        {
            switch (cmd.CmdType)
            {
                case ECmd.ActorSelection:
                {
                    SelectActor(cmd);
                }
                    break;
                case ECmd.ActorMoveToGrid:
                {
                    var p = (ActorMoveToGridParam)cmd.Param;
                    for (int i = 0;i < _userCtrl.SelectedActors.Count;i++)
                    {
                        var uuid = _userCtrl.SelectedActors[i];
                        var entity = _world.GetEntity(uuid);
                        var moveableComp = entity.GetComp<MoveableComp>();
                        if (moveableComp != null)
                        {
                            HandlePlanRoute(entity,p.GridX,p.GridY);
                        }
                    }
                }
                    break;
                case ECmd.SetGridType:
                {
                    var p = (SetGridTypeParam)cmd.Param;
                    GridRecord gridRecord = _map.MapRecord.GetGridAt(p.GridY,p.GridX);
                    if (gridRecord != null)
                    {
                        gridRecord.SetGridType(p.GridType);
                    }
                }
                    break;
                case ECmd.SetGridTexture:
                {
                    var p = (SetGridTextureTypeParam)cmd.Param;
                    GridRecord gridRecord = _map.MapRecord.GetGridAt(p.GridY,p.GridX);
                    if (gridRecord != null)
                    {
                        // Set Target Layer 
                        gridRecord.SetTextureType((int)p.TextureLayer,p.GridTextureType);

                        // When we are setting base layer ,will clear other layer's data
                        if (p.TextureLayer == ETerrainTextureLayer.BaseLayer)
                        {
                            for (int i = 1;i < (int)ETerrainTextureLayer.Max;i++)
                            {
                                gridRecord.SetTextureType((int)i,EGridTextureType.None);       
                            }
                        }
                    }
                }
                    break;
                default:
                    break;
            }
        }
        
        private void HandlePlanRoute(Entity entity,int gridX,int gridY)
        {
            var posComp = entity.GetComp<PositionComp>();
            var routeComp = entity.GetComp<RoutePlanComp>();
            routeComp.bNeedPlan = true;
            routeComp.FromGrid = new Vector2Int(posComp.GridX,posComp.GridY);
            routeComp.ToGrid = new Vector2Int(gridX,gridY);
            
            // MoveableComp moveableComp = entity.GetComp<MoveableComp>();
            // moveableComp.StopMove();
            // moveableComp.StartMove(gridX,gridY);
        }


        private void SelectActor(Cmd cmd)
        {
            // remove selected
            _userCtrl.ClearSelectActors();
            
            // select new 
            var p = (ActorSelectionParam)cmd.Param;
            _userCtrl.AddSelectActors(p.UUIDs);
        }
        
        
    }
}