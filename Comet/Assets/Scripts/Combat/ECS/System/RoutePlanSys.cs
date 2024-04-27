using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace comet.combat
{
    public class RoutePlanSys : BaseSys,ITickSys
    {
        Type[] _types = { typeof(RoutePlanComp)};
        
        private MapComp _mapComp = null;

        private AStar _astar = null;
        
        public RoutePlanSys(World world) : base(world)
        {
            _mapComp = _world.GetWorldComp<MapComp>();
            _astar = new AStar(_mapComp.MapRecord);
        }

        public void OnTick()
        {
            List<Entity> entities = this._world.GetEntities(_types);

            foreach (var entity in entities)
            {
                var routeComp = entity.GetComp<RoutePlanComp>();
                if (routeComp.bNeedPlan)
                {
                    PlanRoute(routeComp);
                }

                if (routeComp.ShouldMove())
                {
                    HandleMoving(entity);
                }
            }
        }

        private void PlanRoute(RoutePlanComp routeComp)
        {
            // Debug.Log("[plan_route]" 
            //           + routeComp.FromGrid.Value.x + "," 
            //           + routeComp.FromGrid.Value.y +
            //           "[to]"
            //           + routeComp.ToGrid.Value.x + ","
            //           + routeComp.ToGrid.Value.y);

            /*
            List<Vector2Int> path = SimplePathFinding.FindPath(_mapComp.MapRecord,routeComp.FromGrid.Value,routeComp.ToGrid.Value);
            routeComp.ResetPath(path);
            routeComp.DontNeedPlan();
            */
            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<Vector2Int> path = _astar.FindPath(routeComp.FromGrid.Value, routeComp.ToGrid.Value);  ;
            watch.Stop();
            Debug.Log("AStar.FindPath() Cost:" + watch.Elapsed);
            
            routeComp.ResetPath(path);
            routeComp.DontNeedPlan();
        

            // Debug.Log("[route]" + path);
            // foreach (var grid in path)
            // {
            //     Debug.Log("[" + grid.x + "," + grid.y + "]");
            // }
        }

        private void HandleMoving(Entity entity)
        {
            var routeComp = entity.GetComp<RoutePlanComp>();
            var posComp = entity.GetComp<PositionComp>();
            var moveableComp = entity.GetComp<MoveableComp>();
            
            Vector2Int nextTarget = routeComp.Grids[routeComp.DoneIndex];
            if (posComp.GridX == nextTarget.x && posComp.GridY == nextTarget.y)
            {
                routeComp.DoneIndex++;
            }
            else
            {
                moveableComp.MoveTo(nextTarget.x,nextTarget.y);
            }
        }

    }
}