using System;
using comet.combat;

using UnityEngine;

namespace comet.combat
{
    public class MovementSys : BaseSys ,IUpdateSys
    {
        private Type[] _comps = {typeof(PositionComp),typeof(MoveableComp)};
        private MapComp _mapComp = null;

        private float kSpeed = 5.0f;
        private float kHeightFixValue = 1.0f;
        
        public MovementSys(World world) : base(world)
        {
            _mapComp = _world.GetWorldComp<MapComp>();
        }

        public void OnUpdate(float deltaTime)
        {
            var entityList = _world.GetEntities(_comps);
            foreach (var entity in entityList)
            {
                var moveable = entity.GetComp<MoveableComp>();
                if (moveable.IsMoving)
                {
                    HandleMoving(entity.GetComp<PositionComp>(),moveable,deltaTime);
                }
            }
        }

        private void HandleMoving(PositionComp posComp,MoveableComp moveable,float deltaTime)
        {
            float x, y, z;
            Metrics.GetGridCenterPos(_mapComp.MapRecord,moveable.TargetGridX,moveable.TargetGridY,out x,out y,out z);
            
            Vector3 from = new Vector3(posComp.X, posComp.Y, posComp.Z);
            Vector3 to = new Vector3(x, y, z);
            
            if (Metrics.IsNear(new Vector2(from.x,from.z),new Vector2(to.x,to.z)))
            {   
                StopMoving(ref to,moveable,posComp);
            }
            else
            {
                KeepMoving(ref from,ref to,posComp,deltaTime);
            }

            SyncPosYByGrid(posComp,posComp.GridX,posComp.GridY);
        }

        private void StopMoving(ref Vector3 destPos,MoveableComp moveable,PositionComp posComp)
        {
            moveable.IsMoving = false;
            posComp.SetPos(_mapComp.MapRecord,destPos.x,destPos.z);
        }

        private void KeepMoving(ref Vector3 from ,ref Vector3 to,PositionComp posComp,float deltaTime)
        {
            Vector3 dir = (to - from);
            dir.y = 0;
            dir = dir.normalized;
            
            Vector3 nextPos = from + dir * kSpeed * deltaTime;
            posComp.SetPos(_mapComp.MapRecord,nextPos.x,nextPos.z);
        }

        private void SyncPosYByGrid(PositionComp posComp,int gridX,int gridY)
        {
            GridRecord gridRecord = _mapComp.MapRecord.GetGridAt(gridY, gridX);
            posComp.Y = gridRecord.Height + kHeightFixValue;
        }
    }
}