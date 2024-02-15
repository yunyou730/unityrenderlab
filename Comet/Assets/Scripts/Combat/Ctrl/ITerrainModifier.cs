using UnityEngine;
namespace comet.combat
{
    public interface ITerrainModifier
    {
        public bool ShouldWorking();
        public void DoJob(int crossPointX,int crossPointY);
    }
}