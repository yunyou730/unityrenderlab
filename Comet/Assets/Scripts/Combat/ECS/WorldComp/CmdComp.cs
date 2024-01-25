using System;
using System.Collections.Generic;

namespace comet.combat
{
    public class CmdComp : BaseWorldComp
    {
        public List<Cmd> cmds = new List<Cmd>();
        
        public void AddCmd(Cmd cmd)
        {
            cmds.Add(cmd);
        }

        public void ClearCmd()
        {
            cmds.Clear();
        }
    }
    

    public enum ECmd
    {
        ActorSelection,
        ActorMoveToGrid,
        SetGridType,
        SetGridTexture,
        
        None,
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
    
    public struct ActorMoveToGridParam
    {
        public int GridX;
        public int GridY;
    }

    public struct SetGridTypeParam
    {
        public int GridX;
        public int GridY;
        public GridRecord.EGridType GridType;
    }
}