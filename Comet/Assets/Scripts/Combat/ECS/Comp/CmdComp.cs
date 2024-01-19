using Unity.VisualScripting;

namespace comet.combat
{
    public enum ECmd
    {
        ActorSelection,
        ActorMoveToTarget,
        ActorMoveByDirection,
        
        None,
    }
    
    public class CmdComp
    {
        
    }
    
    public struct Cmd
    {
        public ECmd CmdType;
        public object Param; 
        
        public Cmd(ECmd cmd,object extParam)
        {
            CmdType = cmd;
            Param = extParam;
        }
    }
    
    public struct ActorSelectionParam
    {
        public int[] UUIDs;
    }
    
    public struct ActorMoveToTargetParam
    {
        public int TargetGridX;
        public int TargetGridY;
    }
}