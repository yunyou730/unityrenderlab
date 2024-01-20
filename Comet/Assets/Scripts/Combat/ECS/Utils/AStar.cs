using System;
using System.Collections.Generic;
using UnityEngine;

namespace comet.combat
{
    public class AStar
    {
        public static List<Vector2Int> FindPath(MapRecord mapRecord,Vector2Int from,Vector2Int to)
        {
            List<Vector2Int> result = new List<Vector2Int>();

            Vector2Int next = from;
            result.Add(next);

            while (next.x != to.x)
            {
                int delta = (to.x - next.x);
                
                int x = delta > 0 ? next.x + 1 : next.x - 1;
                next.x = x;
                result.Add(next);
            }

            while (next.y != to.y)
            {
                int delta = (to.y - next.y);
                
                int y = delta > 0 ? next.y + 1 : next.y - 1;
                next.y = y;
                result.Add(next);
            }
            
            return result;
        }
    }
}