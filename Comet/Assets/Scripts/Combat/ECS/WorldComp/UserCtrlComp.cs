using System;
using System.Collections.Generic;

namespace comet.combat
{
    public class UserCtrlComp : BaseWorldComp
    {
        public List<int> SelectedActors = new List<int>();

        public void AddSelectActors(int[] uuids)
        {
            for (int i = 0;i < uuids.Length;i++)
            {
                SelectedActors.Add(uuids[i]);
            }
        }
        
        public void ClearSelectActors()
        {
            SelectedActors.Clear();
        }
    }
}