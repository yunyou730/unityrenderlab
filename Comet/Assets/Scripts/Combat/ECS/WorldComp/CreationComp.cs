using System;
using System.Collections.Generic;

namespace comet.combat
{
    public enum ECreationType
    {
        Actor,
    }
    
    public class CreationComp : BaseWorldComp
    {
        public List<SpawnInfo> SpawnInfo = new List<SpawnInfo>();
        
        public void AddCreationItem(ECreationType creationType,int row,int col)
        {
            SpawnInfo.Add(new SpawnInfo
            {
                CreationType = creationType,
                Row = row,
                Col = col,
            });
        }
    }

    public struct SpawnInfo
    {
        public ECreationType CreationType;
        public int Row;
        public int Col;
    }

}