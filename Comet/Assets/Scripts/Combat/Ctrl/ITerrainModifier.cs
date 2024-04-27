using UnityEngine;
namespace comet.combat
{
    public enum ETerrainModifyCtrlType
    {
        Click,
        Press,
    }

    public interface ITerrainModifier
    {
        public bool ShouldWorking();
        public void DoJob(int crossPointX,int crossPointY);
        public ETerrainModifyCtrlType GetCtrlType();
    }
}