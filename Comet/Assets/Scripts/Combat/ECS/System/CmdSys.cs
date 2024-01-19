using UnityEngine.XR;

namespace comet.combat
{
    public class CmdSys : BaseSys, ITickSys
    {
        private CmdComp _cmd = null;
        private UserCtrlComp _userCtrl = null;
        public CmdSys(World world) : base(world)
        {
            _cmd = _world.GetWorldComp<CmdComp>();
            _userCtrl = _world.GetWorldComp<UserCtrlComp>();
        }

        public void OnTick()
        {
            for(int i = 0;i < _cmd.cmds.Count;i++)
            {
                HandleCmd(_cmd.cmds[i]);
            }
        }

        private void HandleCmd(Cmd cmd)
        {
            switch (cmd.CmdType)
            {
                case ECmd.ActorSelection:
                {
                    // remove selected
                    _userCtrl.ClearSelectActors();
                    
                    // select new
                    var p = (ActorSelectionParam)cmd.Param;
                    _userCtrl.AddSelectActors(p.UUIDs);
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
                            moveableComp.StopMove();
                            moveableComp.StartMove(p.GridX,p.GridY); 
                        }
                    }
                }
                    break;
                default:
                    break;
            }
        }
    }
}