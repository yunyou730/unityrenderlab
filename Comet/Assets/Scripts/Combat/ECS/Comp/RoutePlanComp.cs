using System;
using System.Collections.Generic;
using UnityEngine;

namespace comet.combat
{
    public class RoutePlanComp : BaseComp
    {
        public int DoneIndex = 0;
        public List<Vector2Int> Grids = null;

        public bool bNeedPlan = false;
        public Vector2Int? FromGrid = null;
        public Vector2Int? ToGrid = null;

        public void DontNeedPlan()
        {
            bNeedPlan = false;
            FromGrid = null;
            ToGrid = null;
        }

        public void ResetPath(List<Vector2Int> path)
        {
            Grids = path;
            DoneIndex = 0;
        }

        public bool ShouldMove()
        {
            if (Grids == null || Grids.Count == 0)
            {
                return false;
            }
            return DoneIndex < Grids.Count;
        }

        // public Vector2Int? NextTarget()
        // {
        //     if (HasFinish())
        //     {
        //         return null;                
        //     }
        //     return Grids[DoneIndex];
        // }

        public void Clear()
        {
            Grids.Clear();
        }
    }
}